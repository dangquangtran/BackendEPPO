using AutoMapper;
using BusinessObjects.Models;
using DTOs.Order;
using Repository.Interfaces;
using Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Implements
{
    public class OrderService : IOrderService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public OrderService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public IEnumerable<OrderVM> GetAllOrders(int pageIndex, int pageSize)
        {
            var orders = _unitOfWork.OrderRepository.Get(filter: c => c.Status != 0, pageIndex: pageIndex, pageSize: pageSize, includeProperties: "OrderDetails");
            return _mapper.Map<IEnumerable<OrderVM>>(orders);
        }

        public Order GetOrderById(int id)
        {
            return _unitOfWork.OrderRepository.GetByID(id);
        }

        public void CreateOrder(CreateOrderDTO createOrder, int userId)
        {
            Order order = _mapper.Map<Order>(createOrder);
            order.CreationDate = DateTime.Now;
            order.Status = 1;
            order.UserId = userId;
            _unitOfWork.OrderRepository.Insert(order);
            _unitOfWork.Save();
        }
        public void UpdateOrder(UpdateOrderDTO updateOrder)
        {
            Order order = _mapper.Map<Order>(updateOrder);
            order.ModificationDate = DateTime.Now;
            _unitOfWork.OrderRepository.Update(order);
            _unitOfWork.Save();
        }
        public IEnumerable<OrderVM> GetOrdersByUserId(int userId, int pageIndex, int pageSize)
        {
            // Lấy danh sách đơn hàng dựa theo userId và có trạng thái khác 0 (đang hoạt động)
            var orders = _unitOfWork.OrderRepository.Get(
                filter: o => o.UserId == userId && o.Status != 0, // Lọc theo userId và trạng thái đơn hàng
                pageIndex: pageIndex,
                pageSize: pageSize,
                includeProperties: "OrderDetails" // Bao gồm thông tin chi tiết đơn hàng
            );

            // Sử dụng AutoMapper để chuyển đổi từ Order sang OrderVM
            return _mapper.Map<IEnumerable<OrderVM>>(orders);
        }
    }
}
