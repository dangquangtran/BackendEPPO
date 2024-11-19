using AutoMapper;
using BusinessObjects.Models;
using DTOs.HistoryBid;
using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.IdentityModel.Tokens;
    using Repository.Interfaces;
    using System;
    using System.Collections.Generic;
    using System.IdentityModel.Tokens.Jwt;
    using System.Linq;
    using System.Net.WebSockets;
    using System.Text;
    using System.Text.Json;
    using System.Threading;
    using System.Threading.Tasks;

    namespace Service.Implements
    {
        public class AuctionHandler
        {
            private readonly Dictionary<int, List<WebSocket>> _roomConnections;
            private readonly IServiceScopeFactory _serviceScopeFactory;
            private readonly IConfiguration _configuration;

        public AuctionHandler(IServiceScopeFactory serviceScopeFactory, IConfiguration configuration)
            {
                _serviceScopeFactory = serviceScopeFactory;
                _configuration = configuration;
                _roomConnections = new Dictionary<int, List<WebSocket>>();
            }

        // Thay đổi các thông báo trong mã nguồn sang tiếng Việt
        public async Task HandleAsync(WebSocket webSocket)
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var _unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
                var _mapper = scope.ServiceProvider.GetRequiredService<IMapper>();
                var buffer = new byte[1024 * 4];
                WebSocketReceiveResult result;

                int? userId = null;

                // Nhận và giải mã token để lấy userId
                result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                var initialMessage = Encoding.UTF8.GetString(buffer, 0, result.Count);

                var authMessage = JsonSerializer.Deserialize<AuthMessageDTO>(initialMessage);
                if (authMessage != null && !string.IsNullOrEmpty(authMessage.Token))
                {
                    var userIdClaim = DecodeJwtToken(authMessage.Token, "userId");

                    if (!string.IsNullOrEmpty(userIdClaim))
                    {
                        userId = int.Parse(userIdClaim);
                        var successResponse = new { Message = "Token is valid, you have been successfully authenticated." };
                        var successResponseBytes = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(successResponse));
                        await webSocket.SendAsync(new ArraySegment<byte>(successResponseBytes), WebSocketMessageType.Text, true, CancellationToken.None);
                        await JoinRoomAsync(authMessage.RoomId, webSocket);
                    }
                    else
                    {
                        await webSocket.CloseAsync(WebSocketCloseStatus.PolicyViolation, "Invalid token", CancellationToken.None);
                        return;
                    }
                }
                else
                {
                    await webSocket.CloseAsync(WebSocketCloseStatus.PolicyViolation, "Invalid authentication", CancellationToken.None);
                    return;
                }

                while (webSocket.State == WebSocketState.Open)
                {
                    try
                    {
                        result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                        var message = Encoding.UTF8.GetString(buffer, 0, result.Count);

                        var bidRequest = JsonSerializer.Deserialize<BidRequest>(message);
                        if (bidRequest == null || userId == null) continue;

                        // Kiểm tra xem user có trong phòng đấu giá không
                        var userRoom = await _unitOfWork.UserRoomRepository
                            .GetFirstOrDefaultAsync(ur => ur.UserId == userId.Value && ur.RoomId == bidRequest.RoomId && ur.IsActive == true);

                        if (userRoom == null)
                        {
                            var response = new { Message = "User is not allowed to bid in this room." };
                            var responseBytes = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(response));
                            await webSocket.SendAsync(new ArraySegment<byte>(responseBytes), WebSocketMessageType.Text, true, CancellationToken.None);
                            continue;
                        }
                        var user = await _unitOfWork.UserRepository.GetByIDAsync(userId.Value);

                        if (user == null || user.IsActive == false || user.WalletId == null)
                        {
                            var response = new { Message = "User or wallet not found." };
                            var responseBytes = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(response));
                            await webSocket.SendAsync(new ArraySegment<byte>(responseBytes), WebSocketMessageType.Text, true, CancellationToken.None);
                            continue;
                        }

                        // Check if the wallet has sufficient balance
                        var userWallet = await _unitOfWork.WalletRepository.GetByIDAsync(user.WalletId.Value);

                        if (userWallet == null || userWallet.NumberBalance < bidRequest.BidAmount)
                        {
                            var response = new { Message = "Insufficient wallet balance to place a bid." };
                            var responseBytes = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(response));
                            await webSocket.SendAsync(new ArraySegment<byte>(responseBytes), WebSocketMessageType.Text, true, CancellationToken.None);
                            continue;
                        }

                        // Lấy thông tin phòng đấu giá từ cơ sở dữ liệu
                        var room = await _unitOfWork.RoomRepository.GetByIDAsync(bidRequest.RoomId, includeProperties: "Plant");
                        if (room == null)
                        {
                            var response = new { Message = "Room not found." };
                            var responseBytes = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(response));
                            await webSocket.SendAsync(new ArraySegment<byte>(responseBytes), WebSocketMessageType.Text, true, CancellationToken.None);
                            continue;
                        }

                        // Lấy giá cây của phòng đấu giá từ đối tượng Plant
                        double plantPrice = 0;
                        if (room.Plant != null)
                        {
                            plantPrice = room.Plant.FinalPrice; // Lấy giá của cây, mặc định 0 nếu không có giá
                        }
                        else
                        {
                            var response = new { Message = "No plant found for this room." };
                            var responseBytes = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(response));
                            await webSocket.SendAsync(new ArraySegment<byte>(responseBytes), WebSocketMessageType.Text, true, CancellationToken.None);
                            continue;
                        }

                        // Lấy priceStep của phòng đấu giá
                        var priceStep = room.PriceStep ?? 0; // Lấy PriceStep, mặc định 0 nếu không có giá trị

                        // Lấy thông tin đấu giá gần nhất trong phòng này
                        var lastBid = await _unitOfWork.HistoryBidRepository.GetQueryable(
                            b => b.RoomId == bidRequest.RoomId && b.IsActive == true,
                            b => b.OrderByDescending(bid => bid.BidTime))  // Sắp xếp theo thời gian giảm dần
                            .FirstOrDefaultAsync(); // Lấy lần đấu giá đầu tiên

                        if (lastBid != null)
                        {
                            // Kiểm tra nếu BidAmount của lần đấu giá mới nhỏ hơn PriceAuctionNext của lần đấu giá trước đó
                            if (bidRequest.BidAmount < lastBid.PriceAuctionNext)
                            {
                                var response = new { Message = $"Bid amount must be greater than or equal to the next auction price: {lastBid.PriceAuctionNext}" };
                                var responseBytes = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(response));
                                await webSocket.SendAsync(new ArraySegment<byte>(responseBytes), WebSocketMessageType.Text, true, CancellationToken.None);
                                continue;
                            }
                        }
                        else
                        {
                            // Nếu chưa có lượt đấu giá nào, kiểm tra nếu BidAmount lớn hơn giá cây trong phòng cộng với PriceStep
                            if (bidRequest.BidAmount < plantPrice + priceStep)
                            {
                                var response = new { Message = $"Bid amount must be greater than or equal to the plant price plus price step: {plantPrice + priceStep}" };
                                var responseBytes = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(response));
                                await webSocket.SendAsync(new ArraySegment<byte>(responseBytes), WebSocketMessageType.Text, true, CancellationToken.None);
                                continue;
                            }
                        }

                        // Tính toán PriceAuctionNext cho lần đấu giá hiện tại
                        var priceAuctionNext = bidRequest.BidAmount + priceStep;

                        // Lưu thông tin đấu giá vào lịch sử
                        var historyBid = new HistoryBid
                        {
                            UserId = userId.Value,
                            RoomId = bidRequest.RoomId,
                            BidAmount = bidRequest.BidAmount,
                            BidTime = DateTime.Now,
                            PriceAuctionNext = priceAuctionNext,
                            IsActive = true,
                            Status = 1
                        };

                        _unitOfWork.HistoryBidRepository.Insert(historyBid);
                        await _unitOfWork.SaveAsync();
                        var historyBidVM = _mapper.Map<HistoryBidVM>(historyBid);
                        // Phát broadcast cho các người dùng trong phòng
                        if (_roomConnections.TryGetValue(bidRequest.RoomId, out var connections))
                        {
                            foreach (var connection in connections)
                            {
                                var broadcastMessage = new
                                {
                                    Message = "Have a new auction",
                                    HistoryBid = historyBidVM
                                };
                                var broadcastBytes = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(broadcastMessage));
                                await connection.SendAsync(new ArraySegment<byte>(broadcastBytes), WebSocketMessageType.Text, true, CancellationToken.None);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("An error occurred: " + ex.Message);
                        Console.WriteLine("Stack Trace: " + ex.StackTrace);
                    }
                }
                }
        }

            private async Task JoinRoomAsync(int roomId, WebSocket webSocket)
            {
                // Kiểm tra nếu phòng chưa có trong từ điển, thêm mới
                if (!_roomConnections.ContainsKey(roomId))
                {
                    _roomConnections[roomId] = new List<WebSocket>();
                }

                // Thêm kết nối WebSocket vào danh sách phòng
                _roomConnections[roomId].Add(webSocket);
            }
            private string DecodeJwtToken(string token, string claimType)
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"]);

                try
                {
                    var claimsPrincipal = tokenHandler.ValidateToken(token, new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(key),
                        ValidateIssuer = false,
                        ValidateAudience = false
                    }, out SecurityToken validatedToken);

                    var jwtToken = (JwtSecurityToken)validatedToken;
                    var claim = jwtToken.Claims.FirstOrDefault(c => c.Type == claimType);

                    return claim?.Value;
                }
                catch
                {
                    return null;
                }
            }
        }

        public class AuthMessageDTO
        {
            public string Token { get; set; }
            public int RoomId { get; set; }
        }

        public class BidRequest
        {
            public int RoomId { get; set; }
            public double BidAmount { get; set; }
        }
    }
