using BusinessObjects.Models;
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

                    // Lưu lịch sử đấu giá
                    var historyBid = new HistoryBid
                    {
                        UserId = userId.Value,
                        RoomId = bidRequest.RoomId,
                        BidAmount = bidRequest.BidAmount,
                        BidTime = DateTime.Now,
                        IsActive = true,
                        Status = 1
                    };

                    _unitOfWork.HistoryBidRepository.Insert(historyBid);
                    await _unitOfWork.SaveAsync();

                    // Phát broadcast cho các người dùng trong phòng
                    if (_roomConnections.TryGetValue(bidRequest.RoomId, out var connections))
                    {
                        foreach (var connection in connections)
                        {
                            var broadcastMessage = new
                            {
                                Message = "Có lượt đấu giá mới",
                                HistoryBid = historyBid
                            };
                            var broadcastBytes = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(broadcastMessage));
                            await connection.SendAsync(new ArraySegment<byte>(broadcastBytes), WebSocketMessageType.Text, true, CancellationToken.None);
                        }
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
