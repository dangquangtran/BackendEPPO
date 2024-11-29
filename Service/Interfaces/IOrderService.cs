using BusinessObjects.Models;
using DTOs.Order;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Interfaces
{
    public interface IOrderService
    {
        IEnumerable<OrderVM> GetAllOrders(int pageIndex, int pageSize);
        OrderVM GetOrderById(int id);
        void CreateOrder(CreateOrderDTO createOrder, int userId);
        void UpdateOrder(UpdateOrderDTO updateOrder);
        IEnumerable<OrderVM> GetOrdersBuyByUserId(int userId, int pageIndex, int pageSize, int status);
        void UpdatePaymentStatus(int orderId, string paymentStatus);
        OrderVM CreateRentalOrder(CreateOrderRentalDTO createOrderDTO, int userId);
        void UpdatePaymentOrderRental(int orderId, int contractId, int userId, int paymentId);
        IEnumerable<OrderRentalVM> GetOrdersRentalByUserId(int userId, int pageIndex, int pageSize, int status);
        void CancelOrder(int orderId, int userId);
        Task<int> CountOrderByStatus(int userId, int status);
        Task<double> CountOrderPrice(int status, int? month = null, int? year = null );
        Task UpdatePreparedOrderSuccess(int orderId, int userId);
        Task UpdateDeliverOrderSuccess(int orderId, List<IFormFile> imageFiles, int userId);

        Task<List<double>> CountOrderPriceForYear(int status, int year);
        Task<List<double>> CountOrderPriceByTypeEcom(int status, int year, int typeEcommerceId);

        Task UpdateDeliverOrderFail(int orderId, List<IFormFile> imageFiles, int userId);
        Task UpdateReturnOrderSuccess(int orderId, List<IFormFile> imageFiles, int userId);
        void UpdateOrderStatus(int orderId, int newStatus, int userId);
        IEnumerable<OrderVM> GetOrdersByOwner(int userId, int pageIndex, int pageSize);
        IEnumerable<OrderVM> GetOrdersByTypeEcommerceId(int typeEcommerceId, DateTime? startDate, DateTime? endDate, int pageIndex, int pageSize);

        Task<int> CountOrderByStatus(int status);
        Task<double> CountOrderPrice(int status);
        Task<double> CountOrderPriceDateNow(int status);
    }
}
