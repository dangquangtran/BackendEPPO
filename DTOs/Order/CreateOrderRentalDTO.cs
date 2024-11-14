using DTOs.OrderDetail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOs.Order
{
    public class CreateOrderRentalDTO
    {
        public double TotalPrice { get; set; }
        public double DeliveryFee { get; set; }
        public string DeliveryAddress { get; set; }
        public int? TypeEcommerceId { get; set; }
        public int? PaymentId { get; set; }
        public List<CreateOrderDetailRentalDTO> OrderDetailsRental { get; set; }
    }
}
