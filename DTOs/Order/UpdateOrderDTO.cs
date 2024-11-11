using BusinessObjects.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOs.Order
{
    public class UpdateOrderDTO
    {
        public int OrderId { get; set; }
        public int? UserId { get; set; }
        public double? TotalPrice { get; set; }
        public double? DeliveryFee { get; set; }
        public string DeliveryAddress { get; set; }
        public double? FinalPrice { get; set; }
        public int? PaymentId { get; set; }
        public string PaymentStatus { get; set; }
        public int? Status { get; set; }
        public DateTime? CreationDate { get; set; }
        public DateTime? ModificationDate { get; set; }

    }
}
