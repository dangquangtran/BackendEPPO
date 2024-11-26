using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Repository.Interfaces;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Service.Implements
{
    public class OrderCancellationService : BackgroundService
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public OrderCancellationService(IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Console.WriteLine("OrderCancellationService is starting.");

            while (!stoppingToken.IsCancellationRequested)
            {
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var _unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

                    // Lấy danh sách đơn hàng chưa thanh toán quá 30 phút
                    var overdueOrders = await _unitOfWork.OrderRepository.GetAsync(order =>
                        order.PaymentId == 2 &&
                        order.PaymentStatus == "Chưa thanh toán" &&
                        order.CreationDate.HasValue &&
                        order.CreationDate.Value.AddMinutes(30) <= DateTime.UtcNow.AddHours(7));

                    foreach (var order in overdueOrders)
                    {
                        // Hủy đơn hàng
                        order.Status = 5; // Trạng thái "Đã hủy"
                        order.ModificationDate = DateTime.UtcNow.AddHours(7);

                        // Lấy các cây liên quan trong
                        var orderDetails = await _unitOfWork.OrderDetailRepository
                            .GetAsync(sod => sod.OrderId == order.OrderId);

                        foreach (var orderDetail in orderDetails)
                        {
                            if (orderDetail.PlantId.HasValue)
                            {
                                // Cập nhật trạng thái cây
                                var plant = await _unitOfWork.PlantRepository.GetByIDAsync(orderDetail.PlantId.Value);
                                if (plant != null)
                                {
                                    plant.IsActive = true;
                                    _unitOfWork.PlantRepository.Update(plant);
                                }
                            }
                        }

                        // Cập nhật trạng thái và lưu thay đổi
                        _unitOfWork.OrderRepository.Update(order);
                        await _unitOfWork.SaveAsync();
                    }
                }

                // Đợi 1 phút trước khi kiểm tra lại
                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
            }

            Console.WriteLine("OrderCancellationService is stopping.");
        }
    }
}
