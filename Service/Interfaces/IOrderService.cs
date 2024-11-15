﻿using BusinessObjects.Models;
using DTOs.Order;
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
        void CancelOrder(int orderId);
    }
}
