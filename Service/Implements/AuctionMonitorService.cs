﻿using BusinessObjects.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Mysqlx.Crud;
using Org.BouncyCastle.Utilities;
using Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Implements
{
    public class AuctionMonitorService : BackgroundService
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public AuctionMonitorService(IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Console.WriteLine("AuctionMonitorService is starting.");
            while (!stoppingToken.IsCancellationRequested)
            {
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var _unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

                    var rooms = await _unitOfWork.RoomRepository.GetAsync(r =>
                        r.EndDate <= DateTime.UtcNow.AddHours(7) && r.Status == 2);
                    foreach (var room in rooms)
                    {
                        var historyBids = await _unitOfWork.HistoryBidRepository.GetAsync(hb => hb.RoomId == room.RoomId && hb.IsActive == true);
                        var plant = await _unitOfWork.PlantRepository.GetByIDAsync(room.PlantId);

                        if (!historyBids.Any())
                        {
                            // Không có ai đấu giá => Chuyển trạng thái phòng thành 4
                            room.Status = 4; // Trạng thái không có ai đấu giá
                            if (plant != null)
                            {
                                plant.IsActive = true; // Đặt lại trạng thái Plant là active
                                _unitOfWork.PlantRepository.Update(plant);
                            }
                            _unitOfWork.RoomRepository.Update(room);
                            await _unitOfWork.SaveAsync();
                            continue; // Bỏ qua việc tạo Order
                        }
                        // Tìm thông tin đấu giá cao nhất trong phòng
                        var highestBid = await _unitOfWork.HistoryBidRepository
                            .GetQueryable(hb => hb.RoomId == room.RoomId && hb.IsActive == true, hb => hb.OrderByDescending(b => b.BidAmount))
                            .FirstOrDefaultAsync();

                        if (highestBid != null)
                        {
                            // Cập nhật thông tin người thắng đấu giá
                            highestBid.IsWinner = true;
                            room.Status = 3; // Đánh dấu phòng đấu giá đã hoàn thành

                            // Đánh dấu rằng người thắng chưa thanh toán
                            highestBid.IsPayment = true;
                            double length = plant.Length ?? 0;
                            double width = plant.Width ?? 0;
                            double height = plant.Height ?? 0;

                            // Tính thể tích
                            double volume = length + (width + height) + (width + height);


                            // Giá cơ bản cho mỗi đơn vị thể tích (10.000 VNĐ/m³)
                            const double baseRate = 1000;

                            // Tính phí vận chuyển
                            double shippingCost = (volume * baseRate) + (volume * 0.10) + 50000;

                            // Trả về giá trị làm tròn
                            var deliveryFee =(int)Math.Ceiling(shippingCost);
                            var userAddress = await _unitOfWork.AddressRepository
                        .GetFirstOrDefaultAsync(addr => addr.UserId == highestBid.UserId);


                            var userAddressList = await _unitOfWork.AddressRepository.GetAsync(addr => addr.UserId == highestBid.UserId && addr.Status == 1);

                            var selectedAddress = userAddressList.OrderByDescending(addr => addr.ModificationDate).FirstOrDefault();
                            //var selectedAddress = userAddressList.ElementAtOrDefault(1);
                            if (selectedAddress == null)
                            {
                                throw new InvalidOperationException("Người dùng chưa có cập nhật địa chỉ.");
                            }
                         
                            string deliveryAddress = selectedAddress?.Description ?? "Chưa cập nhật";
                         
                            var newOrder = new BusinessObjects.Models.Order
                            {
                                UserId = highestBid.UserId,
                                TotalPrice = highestBid.BidAmount,
                                DeliveryFee = deliveryFee,
                                DeliveryAddress = deliveryAddress,
                                FinalPrice = highestBid.BidAmount + deliveryFee,
                                TypeEcommerceId = 3,
                                PaymentId = 2,
                                PaymentStatus = "Đã thanh toán",
                                Status = 2,
                                CreationDate = DateTime.UtcNow.AddHours(7),
                                OrderDetails = new List<OrderDetail>
                                {
                                    new OrderDetail
                                    {
                                        PlantId = plant.PlantId
                                    }
                                }
                            };

                            var user = _unitOfWork.UserRepository.GetByID(highestBid.UserId);
                            var walletId = user?.WalletId;
                            var wallet = _unitOfWork.WalletRepository.GetByID(walletId);
                            wallet.NumberBalance -= newOrder.FinalPrice;
                            _unitOfWork.WalletRepository.Update(wallet);

                            // Tạo và thêm giao dịch mới
                            Transaction transaction = new Transaction
                            {
                                WalletId = walletId,
                                Description = "Thanh toán đơn hàng",
                                WithdrawNumber = newOrder.FinalPrice,
                                RechargeNumber = null,
                                WithdrawDate = DateTime.UtcNow.AddHours(7),
                                CreationDate = DateTime.UtcNow.AddHours(7),
                                PaymentId = 2,
                                Status = 1,
                                IsActive = true
                            };
                            _unitOfWork.TransactionRepository.Insert(transaction);

                            _unitOfWork.OrderRepository.Insert(newOrder);
                            // Cập nhật trạng thái và lưu thay đổi vào cơ sở dữ liệu
                            _unitOfWork.HistoryBidRepository.Update(highestBid);
                            _unitOfWork.RoomRepository.Update(room);
                            var notification = new Notification
                            {
                                UserId = int.Parse(plant.Code),
                                Title = "Thông báo",
                                Description = "Đơn hàng " + newOrder.OrderId + " đã được tạo thành công",
                                CreatedDate = DateTime.UtcNow.AddHours(7),
                                UpdatedDate = DateTime.UtcNow.AddHours(7),
                                IsRead = false,
                                IsNotifications = false,
                                Status = 1
                            };

                            // Thêm vào cơ sở dữ liệu
                            _unitOfWork.NotificationRepository.Insert(notification);
                            await _unitOfWork.SaveAsync();
                        }
                    }

                    await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken); // Chạy kiểm tra sau mỗi giây
                }
            }
            Console.WriteLine("AuctionMonitorService is stopping.");
        }
    }
}
