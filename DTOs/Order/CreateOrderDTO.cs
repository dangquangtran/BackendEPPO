using BusinessObjects.Models;
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
        public int? UserVoucherId { get; set; }
        public int? PlantVoucherId { get; set; }
        public double? FinalPrice { get; set; }
        public int? TransactionId { get; set; }
        public int? PaymentId { get; set; }
        public int? PaymentStatus { get; set; }
        public List<CreateOrderDetailDTO> OrderDetails { get; set; }
    }
}
