﻿using AutoMapper;
using BusinessObjects.Models;
using DTOs.Order;
using Repository.Implements;
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

        public OrderVM GetOrderById(int id)
        {
            var order =_unitOfWork.OrderRepository.GetByID(id, includeProperties: "OrderDetails");
            return _mapper.Map<OrderVM>(order);
        }

        public void CreateOrder(CreateOrderDTO createOrderDTO, int userId)
        {
            var user = _unitOfWork.UserRepository.GetByID(userId);
            var walletId = user.WalletId;
            var wallet = _unitOfWork.WalletRepository.GetByID(walletId);
            if (wallet == null)
            {
                throw new Exception("Không tìm thấy ví của người dùng.");
            }

            // Tính tổng số tiền cần thanh toán cho order
            double finalPrice = createOrderDTO.TotalPrice + createOrderDTO.DeliveryFee;
            if (wallet.NumberBalance < finalPrice)
            {
                throw new Exception("Số dư trong ví không đủ để thanh toán.");
            }
            wallet.NumberBalance -= finalPrice;
            _unitOfWork.WalletRepository.Update(wallet);

            Transaction transaction = new Transaction
            {
                WalletId = walletId,
                Description = "Thanh toán đơn hàng",
                WithdrawNumber = finalPrice,
                RechargeNumber = null,
                WithdrawDate = DateTime.Now,
                CreationDate = DateTime.Now,
                PaymentId = 2,
                Status = 1,
                IsActive = true
            };
            _unitOfWork.TransactionRepository.Insert(transaction);

            Order order = _mapper.Map<Order>(createOrderDTO);
            order.CreationDate = DateTime.Now;
            order.Status = 1;
            order.UserId = userId;
            order.FinalPrice = order.TotalPrice + order.DeliveryFee;
            order.PaymentStatus = "Đã thanh toán";
            // 2. Tính tổng tiền và giá cuối cùng nếu có voucher
            //order.TotalPrice = CalculateTotalPrice(createOrderDTO);
            //order.FinalPrice = ApplyVoucher(order.TotalPrice, order.UserVoucherId, order.PlantVoucherId);

            //// 3. Thêm các chi tiết đơn hàng (OrderDetails) và sub chi tiết (SubOrderDetails)
            //foreach (var orderDetailDTO in createOrderDTO.OrderDetails)
            //{
            //    OrderDetail orderDetail = _mapper.Map<OrderDetail>(orderDetailDTO);
            //    orderDetail.CreationDate = DateTime.Now;
            //    orderDetail.Status = 1; // Giả sử trạng thái "1" là chi tiết đơn hàng mới
            //    orderDetail.OrderId = order.OrderId; // Gắn OrderId vào OrderDetail
            //    order.OrderDetails.Add(orderDetail);

            //    // 4. Thêm các SubOrderDetails
            //    foreach (var subOrderDetailDTO in orderDetailDTO.SubOrderDetails)
            //    {
            //        SubOrderDetail subOrderDetail = _mapper.Map<SubOrderDetail>(subOrderDetailDTO);
            //        subOrderDetail.OrderDetailId = orderDetail.OrderDetailId; // Gắn OrderDetailId vào SubOrderDetail
            //        orderDetail.SubOrderDetails.Add(subOrderDetail);
            //    }
            //}

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
        public IEnumerable<OrderVM> GetOrdersByUserId(int userId, int pageIndex, int pageSize, int status)
        {
            // Lấy danh sách đơn hàng dựa theo userId và có trạng thái khác 0 (đang hoạt động)
            var orders = _unitOfWork.OrderRepository.Get(
                filter: o => o.UserId == userId && o.Status == status, // Lọc theo userId và trạng thái đơn hàng
                pageIndex: pageIndex,
                pageSize: pageSize,
                includeProperties: "OrderDetails" // Bao gồm thông tin chi tiết đơn hàng
            );

            // Sử dụng AutoMapper để chuyển đổi từ Order sang OrderVM
            return _mapper.Map<IEnumerable<OrderVM>>(orders);
        }

        //private double CalculateTotalPrice(CreateOrderDTO createOrderDTO)
        //{
        //    double totalPrice = 0;

        //    // Tính tổng giá của tất cả các OrderDetails
        //    foreach (var orderDetail in createOrderDTO.OrderDetails)
        //    {
        //        totalPrice += orderDetail.TotalPrice ?? 0;
        //    }

        //    return totalPrice;
        //}

        //private double ApplyVoucher(double? totalPrice, int? userVoucherId, int? plantVoucherId)
        //{
        //    double finalPrice = totalPrice ?? 0;

        //    // Giả sử hàm ApplyUserVoucher và ApplyPlantVoucher sẽ trả về số tiền giảm giá
        //    if (userVoucherId.HasValue)
        //    {
        //        finalPrice -= ApplyUserVoucher(userVoucherId.Value);
        //    }

        //    if (plantVoucherId.HasValue)
        //    {
        //        finalPrice -= ApplyPlantVoucher(plantVoucherId.Value);
        //    }

        //    return finalPrice;
        //}
        public void UpdatePaymentStatus(int orderId, string paymentStatus)
        {
            var order = _unitOfWork.OrderRepository.GetByID(orderId);
            if (order != null)
            {
                order.PaymentStatus = paymentStatus; // Giả sử bạn có thuộc tính PaymentStatus trong Order
                order.ModificationDate = DateTime.Now; // Cập nhật ngày sửa đổi
                _unitOfWork.OrderRepository.Update(order);
                _unitOfWork.Save();
            }
        }
    }
}
