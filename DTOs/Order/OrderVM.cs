using BusinessObjects.Models;
using DTOs.OrderDetail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOs.Order
{
    public class OrderVM
    {
        public int OrderId { get; set; }
        public int UserId { get; set; }
        public double? TotalPrice { get; set; }
        public double? DeliveryFee { get; set; }
        public string DeliveryAddress { get; set; }
        public string DeliveryDescription { get; set; }
        public double? FinalPrice { get; set; }
        public int? TypeEcommerceId { get; set; }
        public int? PaymentId { get; set; }
        public string PaymentStatus { get; set; }
        public int? Status { get; set; }
        public DateTime? CreationDate { get; set; }
        public DateTime? ModificationDate { get; set; }
        public virtual ICollection<ImageDeliveryOrder> ImageDeliveryOrders { get; set; }
        public virtual ICollection<ImageReturnOrder> ImageReturnOrders { get; set; }
        public virtual ICollection<OrderDetailVM> OrderDetails { get; set; }
    }
}
