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
                        var successResponse = "Token";
                        var successResponseBytes = Encoding.UTF8.GetBytes(successResponse);
                        await webSocket.SendAsync(new ArraySegment<byte>(successResponseBytes), WebSocketMessageType.Text, true, CancellationToken.None);
                        await JoinRoomAsync(authMessage.RoomId, webSocket);
                    }
                    else
                    {
                        await webSocket.CloseAsync(WebSocketCloseStatus.PolicyViolation, "Token không hợp lệ", CancellationToken.None);
                        return;
                    }
                }
                else
                {
                    await webSocket.CloseAsync(WebSocketCloseStatus.PolicyViolation, "Xác thực thất bại", CancellationToken.None);
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
                            var response = "Người dùng không được phép đấu giá trong phòng này.";
                            var responseBytes = Encoding.UTF8.GetBytes(response);
                            await webSocket.SendAsync(new ArraySegment<byte>(responseBytes), WebSocketMessageType.Text, true, CancellationToken.None);
                            continue;
                        }
                        var user = await _unitOfWork.UserRepository.GetByIDAsync(userId.Value);

                        if (user == null || user.IsActive == false || user.WalletId == null)
                        {
                            var response = "User hoặc ví không tìm thấy được";
                            var responseBytes = Encoding.UTF8.GetBytes(response);
                            await webSocket.SendAsync(new ArraySegment<byte>(responseBytes), WebSocketMessageType.Text, true, CancellationToken.None);
                            continue;
                        }

                        // Check if the wallet has sufficient balance
                        var userWallet = await _unitOfWork.WalletRepository.GetByIDAsync(user.WalletId.Value);

                        if (userWallet == null || userWallet.NumberBalance < bidRequest.BidAmount)
                        {
                            var response = "Số dư ví không đủ để thực hiện đấu giá.";
                            var responseBytes = Encoding.UTF8.GetBytes(response);
                            await webSocket.SendAsync(new ArraySegment<byte>(responseBytes), WebSocketMessageType.Text, true, CancellationToken.None);
                            continue;
                        }

                        // Lấy thông tin phòng đấu giá từ cơ sở dữ liệu
                        var room = await _unitOfWork.RoomRepository.GetByIDAsync(bidRequest.RoomId, includeProperties: "Plant");
                        if (room == null)
                        {
                            var response = "Không tìm thấy phòng";
                            var responseBytes = Encoding.UTF8.GetBytes(response);
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
                            var response = "Không tìm thấy cây trong phòng";
                            var responseBytes = Encoding.UTF8.GetBytes(response);
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
                                var response = $"Bạn phải ra giá cao hơn giá cao nhât + bước giá: {lastBid.PriceAuctionNext}";
                                var responseBytes = Encoding.UTF8.GetBytes(response);
                                await webSocket.SendAsync(new ArraySegment<byte>(responseBytes), WebSocketMessageType.Text, true, CancellationToken.None);
                                continue;
                            }
                        }
                        else
                        {
                            // Nếu chưa có lượt đấu giá nào, kiểm tra nếu BidAmount lớn hơn giá cây trong phòng cộng với PriceStep
                            if (bidRequest.BidAmount < plantPrice + priceStep)
                            {
                                var response = $"Bạn phải ra giá cao hơn giá khởi điểm + bước giá: {plantPrice + priceStep}";
                                var responseBytes = Encoding.UTF8.GetBytes(response);
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
                            BidTime = DateTime.UtcNow.AddHours(7),
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
                            var invalidConnections = new List<WebSocket>();

                            foreach (var connection in connections)
                            {
                                if (connection.State == WebSocketState.Open)
                                {
                                    var response = "Có lượt đặt cược mới";
                                    var responseBytes = Encoding.UTF8.GetBytes(response);
                                    await connection.SendAsync(new ArraySegment<byte>(responseBytes), WebSocketMessageType.Text, true, CancellationToken.None);
                                }
                                else
                                {
                                    invalidConnections.Add(connection);
                                }
                            }

                            // Loại bỏ các kết nối không hợp lệ
                            foreach (var invalidConnection in invalidConnections)
                            {
                                connections.Remove(invalidConnection);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("An error occurred: " + ex.Message);
                        Console.WriteLine("Stack Trace: " + ex.StackTrace);
                    }
                }
                // Khi kết nối đóng, loại bỏ khỏi danh sách kết nối trong phòng
                await LeaveRoomAsync(authMessage.RoomId, webSocket);
            }
        }

        private async Task JoinRoomAsync(int roomId, WebSocket webSocket)
        {
            if (!_roomConnections.ContainsKey(roomId))
            {
                _roomConnections[roomId] = new List<WebSocket>();
            }

            // Xóa các kết nối đã đóng
            _roomConnections[roomId] = _roomConnections[roomId]
                .Where(socket => socket.State == WebSocketState.Open).ToList();

            // Thêm kết nối mới
            _roomConnections[roomId].Add(webSocket);
        }
        private async Task LeaveRoomAsync(int roomId, WebSocket webSocket)
        {
            if (_roomConnections.ContainsKey(roomId))
            {
                var connections = _roomConnections[roomId];

                // Loại bỏ kết nối khỏi danh sách
                connections.Remove(webSocket);

                // Nếu không còn kết nối nào trong phòng, có thể loại bỏ phòng
                if (connections.Count == 0)
                {
                    _roomConnections.Remove(roomId);
                }
            }
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
