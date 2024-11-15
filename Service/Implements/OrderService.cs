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

            double finalPrice = createOrderDTO.TotalPrice + createOrderDTO.DeliveryFee;

            if (createOrderDTO.PaymentId == 2)
            {
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
            }

            Order order = _mapper.Map<Order>(createOrderDTO);
            order.CreationDate = DateTime.Now;
            order.Status = 1;
            order.UserId = userId;
            order.FinalPrice = order.TotalPrice + order.DeliveryFee;
            order.PaymentStatus = createOrderDTO.PaymentId == 2 ? "Đã thanh toán" : "Chưa thanh toán";
            order.TypeEcommerceId = 1;

            foreach (var orderDetailDTO in createOrderDTO.OrderDetails)
            {
                if (orderDetailDTO.PlantId.HasValue)
                {
                    var plant = _unitOfWork.PlantRepository.GetByID(orderDetailDTO.PlantId.Value);
                    if (plant != null)
                    {
                        // Kiểm tra trạng thái isActive của cây
                        if ((bool)!plant.IsActive)
                        {
                            throw new Exception($"Cây với ID {plant.PlantId} không thể được đặt vì đã ngừng hoạt động.");
                        }

                        // Cập nhật trạng thái của cây
                        plant.IsActive = false;
                        _unitOfWork.PlantRepository.Update(plant);
                    }
                }
            }

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
        public IEnumerable<OrderVM> GetOrdersBuyByUserId(int userId, int pageIndex, int pageSize, int status)
        {
            var orders = _unitOfWork.OrderRepository.Get(
                filter: o => o.UserId == userId && o.Status == status && o.TypeEcommerceId == 1,
                pageIndex: pageIndex,
                pageSize: pageSize,
                includeProperties: "OrderDetails,OrderDetails.Plant" // Bao gồm thông tin chi tiết đơn hàng
            );

            return _mapper.Map<IEnumerable<OrderVM>>(orders);
        }

        public IEnumerable<OrderRentalVM> GetOrdersRentalByUserId(int userId, int pageIndex, int pageSize, int status)
        {
            var orders = _unitOfWork.OrderRepository.Get(
                filter: o => o.UserId == userId && o.Status == status && o.TypeEcommerceId == 2,
                pageIndex: pageIndex,
                pageSize: pageSize,
                includeProperties: "OrderDetails,OrderDetails.Plant" // Bao gồm thông tin chi tiết đơn hàng
            );

            return _mapper.Map<IEnumerable<OrderRentalVM>>(orders);
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
        public OrderVM CreateRentalOrder(CreateOrderRentalDTO createOrderDTO, int userId)
        {
            var unpaidOrdersCount = _unitOfWork.OrderRepository
                .Get(o => o.UserId == userId && o.PaymentStatus == "Chưa thanh toán" && o.PaymentId == 2 && o.Status == 1)
                .Count();

            if (unpaidOrdersCount > 3)
            {
                throw new Exception("Người dùng có hơn 3 đơn hàng thuê chưa thanh toán. Không thể tạo đơn hàng mới.");
            }

          
            // Tạo order mới
            Order order = _mapper.Map<Order>(createOrderDTO);

            order.CreationDate = DateTime.Now;
            order.TypeEcommerceId = 2;
            order.Status = 1;
            order.UserId = userId;

            foreach (var orderDetail in order.OrderDetails)
            {
                if (orderDetail.RentalStartDate.HasValue && orderDetail.NumberMonth.HasValue)
                {
                    // Lấy giá cây từ cơ sở dữ liệu
                    var plant = _unitOfWork.PlantRepository.GetByID(orderDetail.PlantId);
                    if (plant != null)
                    {
                        // Tính giá cho OrderDetail = giá cây * số tháng
                        double priceForOrderDetail = plant.Price * orderDetail.NumberMonth.Value;

                        // Cộng dồn giá này vào tổng giá của Order
                        order.TotalPrice += priceForOrderDetail;

                        // Tính RentalEndDate
                        orderDetail.RentalEndDate = orderDetail.RentalStartDate.Value.AddMonths((int)orderDetail.NumberMonth.Value);
                    }
                    else
                    {
                        throw new Exception($"Không tìm thấy cây với ID {orderDetail.PlantId}.");
                    }
                }
            }

            order.FinalPrice = order.TotalPrice + order.DeliveryFee;
            order.PaymentStatus = "Chưa thanh toán";

            // Cập nhật trạng thái của cây và lưu đơn hàng
            foreach (var orderDetailDTO in createOrderDTO.OrderDetails)
            {
                if (orderDetailDTO.PlantId.HasValue)
                {
                    var plant = _unitOfWork.PlantRepository.GetByID(orderDetailDTO.PlantId.Value);
                    if (plant != null)
                    {
                        if ((bool)!plant.IsActive)
                        {
                            throw new Exception($"Cây với ID {plant.PlantId} không thể được thuê vì đã ngừng hoạt động.");
                        }

                        plant.IsActive = false;
                        _unitOfWork.PlantRepository.Update(plant);
                    }
                }
            }

            _unitOfWork.OrderRepository.Insert(order);
            _unitOfWork.Save();
            return _mapper.Map<OrderVM>(order);
        }



        public void UpdatePaymentOrderRental(int orderId, int contractId, int userId, int paymentId)
        {
            // Lấy thông tin hợp đồng và cập nhật
            var contract = _unitOfWork.ContractRepository.GetByID(contractId);
            if (contract != null)
            {
                contract.IsActive = 1;
                _unitOfWork.ContractRepository.Update(contract);
            }

            // Lấy thông tin người dùng và ví
            var user = _unitOfWork.UserRepository.GetByID(userId);
            var walletId = user?.WalletId;
            var wallet = _unitOfWork.WalletRepository.GetByID(walletId);

            if (wallet == null)
            {
                throw new Exception("Không tìm thấy ví của người dùng.");
            }

            // Lấy thông tin đơn hàng
            var order = _unitOfWork.OrderRepository.GetByID(orderId);
            if (order == null)
            {
                throw new Exception("Không tìm thấy đơn hàng.");
            }

            // Kiểm tra paymentId để quyết định logic cần thực hiện
            if (paymentId == 2)
            {
                // Kiểm tra số dư ví
                if (wallet.NumberBalance < order.FinalPrice)
                {
                    throw new Exception("Số dư trong ví không đủ để thanh toán.");
                }

                // Trừ tiền từ ví và cập nhật ví
                wallet.NumberBalance -= order.FinalPrice;
                _unitOfWork.WalletRepository.Update(wallet);

                // Tạo và thêm giao dịch mới
                Transaction transaction = new Transaction
                {
                    WalletId = walletId,
                    Description = "Thanh toán đơn hàng",
                    WithdrawNumber = order.FinalPrice,
                    RechargeNumber = null,
                    WithdrawDate = DateTime.Now,
                    CreationDate = DateTime.Now,
                    PaymentId = 2,
                    Status = 1,
                    IsActive = true
                };
                _unitOfWork.TransactionRepository.Insert(transaction);

                // Cập nhật trạng thái thanh toán đơn hàng
                order.PaymentStatus = "Đã thanh toán";
            }
           

            order.ModificationDate = DateTime.Now;
            _unitOfWork.OrderRepository.Update(order);
            _unitOfWork.Save();
        }

        public void CancelOrder(int orderId)
        {
            // Lấy thông tin đơn hàng từ cơ sở dữ liệu
            var order = _unitOfWork.OrderRepository.GetByID(orderId, includeProperties: "OrderDetails");
            if (order == null)
            {
                throw new Exception("Không tìm thấy đơn hàng.");
            }
            
            // Kiểm tra trạng thái đơn hàng (chỉ hủy nếu chưa hoàn thành)
            if (order.Status == 4 || order.PaymentStatus == "Đã thanh toán")
            {
                throw new Exception("Đơn hàng đã hoàn thành hoặc đã được thanh toán và không thể hủy.");
            }

            order.Status = 5;
            order.ModificationDate = DateTime.Now;

            foreach (var orderDetail in order.OrderDetails)
            {
                // Cập nhật trạng thái của cây nếu cần
                if (orderDetail.PlantId.HasValue)
                {
                    var plant = _unitOfWork.PlantRepository.GetByID(orderDetail.PlantId.Value);
                    if (plant != null)
                    {
                        plant.IsActive = true; // Kích hoạt lại cây nếu đơn hàng bị hủy
                        _unitOfWork.PlantRepository.Update(plant);
                    }
                }
            }

            // Cập nhật đơn hàng
            _unitOfWork.OrderRepository.Update(order);
            _unitOfWork.Save();
        }

    }

}
