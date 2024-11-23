using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
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

                    var rooms = await _unitOfWork.RoomRepository.GetAsync(r => r.EndDate <= DateTime.UtcNow.AddHours(7));
                    foreach (var room in rooms)
                    {
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
                            highestBid.IsPayment = false;

                            // Cập nhật trạng thái và lưu thay đổi vào cơ sở dữ liệu
                            _unitOfWork.HistoryBidRepository.Update(highestBid);
                            _unitOfWork.RoomRepository.Update(room);
                            await _unitOfWork.SaveAsync();
                        }
                    }

                    await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken); // Chạy kiểm tra sau mỗi giây
                }
            }
            Console.WriteLine("AuctionMonitorService is stopping.");
        }
    }
}
