﻿using BusinessObjects.Models;
using DTOs.OrderDetail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOs.Order
{
    public class CreateOrderDTO
    {
        public string Description { get; set; }
        public double? TotalPrice { get; set; }
        public double? DeliveryFee { get; set; }
        public string DeliveryAddress { get; set; }
        public int? PaymentId { get; set; }
        public string PaymentStatus { get; set; }
        public List<CreateOrderDetailDTO> OrderDetails { get; set; }
    }
}