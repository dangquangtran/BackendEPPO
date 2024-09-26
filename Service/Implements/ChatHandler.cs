using BusinessObjects.Models;
using DTOs.Message;
using Microsoft.Extensions.DependencyInjection;
using Repository.Interfaces;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Service.Implements
{
    public class ChatHandler
    {
        private readonly ConcurrentDictionary<int, WebSocket> _userSockets = new ConcurrentDictionary<int, WebSocket>();
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public ChatHandler(IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;
        }

        public async Task HandleAsync(WebSocket socket)
        {
            int? userId = null;
            try
            {
                // Nhận thông tin đăng nhập hoặc xác thực từ client
                var buffer = new byte[1024 * 4];
                var result = await socket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

                if (result.MessageType == WebSocketMessageType.Text)
                {
                    var initialMessage = Encoding.UTF8.GetString(buffer, 0, result.Count);
                    var authMessage = JsonSerializer.Deserialize<AuthMessageDTO>(initialMessage);

                    if (authMessage != null && authMessage.UserId > 0)
                    {
                        userId = authMessage.UserId;
                        _userSockets.TryAdd(userId.Value, socket);
                    }
                    else
                    {
                        // Nếu không hợp lệ, đóng kết nối
                        await socket.CloseAsync(WebSocketCloseStatus.PolicyViolation, "Invalid authentication", CancellationToken.None);
                        return;
                    }
                }

                // Vòng lặp nhận và xử lý tin nhắn
                while (socket.State == WebSocketState.Open)
                {
                    buffer = new byte[1024 * 4];
                    result = await socket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

                    if (result.MessageType == WebSocketMessageType.Close)
                    {
                        break;
                    }
                    else if (result.MessageType == WebSocketMessageType.Text)
                    {
                        var messageJson = Encoding.UTF8.GetString(buffer, 0, result.Count);
                        var chatMessage = JsonSerializer.Deserialize<ChatMessageDTO>(messageJson);

                        if (chatMessage != null)
                        {
                            // Tạo một phạm vi mới để sử dụng IUnitOfWork
                            using (var scope = _serviceScopeFactory.CreateScope())
                            {
                                var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

                                // Tạo và lưu tin nhắn vào cơ sở dữ liệu
                                var message = new Message
                                {
                                    ConversationId = chatMessage.ConversationId,
                                    UserId = chatMessage.UserId,
                                    Message1 = chatMessage.Message1,
                                    Type = chatMessage.Type,
                                    ImageLink = chatMessage.ImageLink,
                                    CreationDate = DateTime.UtcNow,
                                    IsSent = true,
                                    IsSeen = false,
                                    Status = 1 // Ví dụ: 1 = active
                                };

                                unitOfWork.MessageRepository.Insert(message);
                                unitOfWork.Save();

                                // Phát tin nhắn tới các người dùng trong cuộc trò chuyện
                                await BroadcastMessage(chatMessage.ConversationId, message, unitOfWork);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"WebSocket error: {ex.Message}");
            }
            finally
            {
                if (userId.HasValue)
                {
                    _userSockets.TryRemove(userId.Value, out _);
                }

                if (socket.State != WebSocketState.Closed)
                {
                    await socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", CancellationToken.None);
                }

                socket.Dispose();
            }
        }

        private async Task BroadcastMessage(int conversationId, Message message, IUnitOfWork unitOfWork)
        {
            // Lấy thông tin cuộc trò chuyện
            var conversation = unitOfWork.ConversationRepository.Get(
                filter: c => c.ConversationId == conversationId,
                includeProperties: "UserOneNavigation,UserTwoNavigation"
            ).FirstOrDefault();

            if (conversation == null)
                return;

            // Xác định người nhận
            int recipientId = 0;
            if (conversation.UserOne == message.UserId)
            {
                recipientId = conversation.UserTwo ?? 0;
            }
            else if (conversation.UserTwo == message.UserId)
            {
                recipientId = conversation.UserOne ?? 0;
            }

            // Nếu không tìm thấy người nhận, thoát ra
            if (recipientId == 0)
                return;

            // Tạo tin nhắn để phát
            var broadcastMessage = new
            {
                message.MessageId,
                message.ConversationId,
                message.UserId,
                message.Message1,
                message.Type,
                message.ImageLink,
                message.CreationDate,
                message.IsSent,
                message.IsSeen,
                message.Status
            };

            var broadcastJson = JsonSerializer.Serialize(broadcastMessage);
            var broadcastBytes = Encoding.UTF8.GetBytes(broadcastJson);
            var buffer = new ArraySegment<byte>(broadcastBytes);

            // Chỉ gửi tin nhắn tới người nhận
            if (_userSockets.TryGetValue(recipientId, out var recipientSocket) && recipientSocket.State == WebSocketState.Open)
            {
                await recipientSocket.SendAsync(buffer, WebSocketMessageType.Text, true, CancellationToken.None);
            }
        }
    }
}
    public class AuthMessageDTO
{
    public int UserId { get; set; } // ID của người dùng
}
