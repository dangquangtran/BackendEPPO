using BusinessObjects.Models;
using DTOs.Message;
using Firebase.Storage;
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
using System.Xml.Linq;
using Microsoft.Extensions.Configuration;
using DTOs.Conversation;
using System.Text.Encodings.Web;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;

namespace Service.Implements
{
    public class ChatHandler
    {
        private readonly ConcurrentDictionary<int, WebSocket> _userSockets = new ConcurrentDictionary<int, WebSocket>();
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly IConfiguration _configuration;

        public ChatHandler(IServiceScopeFactory serviceScopeFactory, IConfiguration configuration)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _configuration = configuration;
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
                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    };
                    var authMessage = JsonSerializer.Deserialize<AuthMessageDTO>(initialMessage, options);

                    if (authMessage != null && !string.IsNullOrEmpty(authMessage.Token))
                    {
                        // Giải mã token để lấy userId
                        var userIdClaim = DecodeJwtToken(authMessage.Token, "userId");

                        if (!string.IsNullOrEmpty(userIdClaim))
                        {
                            userId = int.Parse(userIdClaim);
                            _userSockets.TryAdd(userId.Value, socket);
                        }
                        else
                        {
                            await socket.CloseAsync(WebSocketCloseStatus.PolicyViolation, "Invalid token", CancellationToken.None);
                            return;
                        }
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
                            string imageUrl = null;

                            // Nếu tin nhắn có hình ảnh, tải lên Firebase
                            if (!string.IsNullOrEmpty(chatMessage.ImageLink))
                            {
                                imageUrl = await UploadImageToFirebase(chatMessage.ImageLink, userId.Value, chatMessage.ConversationId);
                            }
                            // Tạo một phạm vi mới để sử dụng IUnitOfWork
                            using (var scope = _serviceScopeFactory.CreateScope())
                            {
                                var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

                                // Tạo và lưu tin nhắn vào cơ sở dữ liệu
                                var message = new Message
                                {
                                    ConversationId = chatMessage.ConversationId,
                                    UserId = userId.Value,
                                    Message1 = chatMessage.Message1,
                                    Type = chatMessage.Type,
                                    ImageLink = imageUrl,
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
                return null; // Trả về null nếu không giải mã được
            }
        }
        private async Task<string> UploadImageToFirebase(string imagePath, int userId, int conversationId)
        {
            var stream = File.Open(imagePath, FileMode.Open);
            var timestamp = DateTime.UtcNow.ToString("yyyyMMdd_HHmmssfff"); // Sử dụng "fff" để có độ chính xác đến mili giây
            var fileName = $"user_{userId}_conv_{conversationId}_{timestamp}.jpg";

            // Tải ảnh lên Firebase
            var task = new FirebaseStorage(
                _configuration["Firebase:Bucket"],
                new FirebaseStorageOptions
                {
                    ThrowOnCancel = true
                })
                .Child("chat_images")
                .Child(fileName)
                .PutAsync(stream);

            // Lấy URL của ảnh đã tải lên
            return await task;
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

            var options = new JsonSerializerOptions
            {
                WriteIndented = true, // Để dễ đọc
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping // Bỏ qua escape cho một số ký tự
            };

            var broadcastJson = JsonSerializer.Serialize(broadcastMessage, options);
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
    public string Token { get; set; }
}
