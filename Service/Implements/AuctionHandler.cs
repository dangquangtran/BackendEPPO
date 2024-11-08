using BusinessObjects.Models;
using Microsoft.EntityFrameworkCore;
using Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Service.Implements
{
    public class AuctionHandler
    {
        private readonly Dictionary<int, List<WebSocket>> _roomConnections;
        private readonly IUnitOfWork _unitOfWork;

        public AuctionHandler (IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _roomConnections = new Dictionary<int, List<WebSocket>>();
        }

        public async Task HandleAsync(WebSocket webSocket)
        {
            var buffer = new byte[1024 * 4];
            WebSocketReceiveResult result;

            while (webSocket.State == WebSocketState.Open)
            {
                result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                var message = Encoding.UTF8.GetString(buffer, 0, result.Count);

                // Deserialize message to get user and room information
                var bidRequest = JsonSerializer.Deserialize<BidRequest>(message);
                if (bidRequest == null) continue;

                // Kiểm tra xem user có trong phòng đấu giá không
                var userRoom = await _unitOfWork.UserRoomRepository
                    .GetFirstOrDefaultAsync(ur => ur.UserId == bidRequest.UserId && ur.RoomId == bidRequest.RoomId && ur.IsActive == true);

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
                    UserId = bidRequest.UserId,
                    RoomId = bidRequest.RoomId,
                    BidAmount = bidRequest.BidAmount,
                    BidTime = DateTime.UtcNow,
                    CreatedDate = DateTime.UtcNow,
                    IsActive = true,
                    Status = 1 // Bạn có thể thay đổi trạng thái tùy theo logic
                };

                //_context.HistoryBids.Add(historyBid);
                //await _context.SaveChangesAsync();

                // Phát broadcast cho tất cả người dùng trong phòng (nếu cần)
                if (_roomConnections.TryGetValue(bidRequest.RoomId, out var connections))
                {
                    foreach (var connection in connections)
                    {
                        var broadcastMessage = new { Message = "New bid placed", historyBid };
                        var broadcastBytes = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(broadcastMessage));
                        await connection.SendAsync(new ArraySegment<byte>(broadcastBytes), WebSocketMessageType.Text, true, CancellationToken.None);
                    }
                }
            }
        }
    }

    // Model cho request đấu giá
    public class BidRequest
    {
        public int UserId { get; set; }
        public int RoomId { get; set; }
        public double BidAmount { get; set; }
    }
}

