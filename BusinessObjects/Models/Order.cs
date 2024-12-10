using System;
using System.Collections.Generic;

namespace BusinessObjects.Models
{
    public partial class Order
    {
        public Order()
        {
            ImageDeliveryOrders = new HashSet<ImageDeliveryOrder>();
            ImageReturnOrders = new HashSet<ImageReturnOrder>();
            OrderDetails = new HashSet<OrderDetail>();
        }

        public int OrderId { get; set; }
        public int? UserId { get; set; }
        public double? TotalPrice { get; set; }
        public double? DeliveryFee { get; set; }
        public string DeliveryAddress { get; set; }
        public string DeliveryDescription { get; set; }
        public int? NumberOfDeliveries { get; set; }
        public double? FinalPrice { get; set; }
        public int? TypeEcommerceId { get; set; }
        public int? PaymentId { get; set; }
        public string PaymentStatus { get; set; }
        public int? Status { get; set; }
        public DateTime? CreationDate { get; set; }
        public DateTime? ModificationDate { get; set; }
        public int? ModificationBy { get; set; }
        public string Code { get; set; }

        public virtual User ModificationByNavigation { get; set; }
        public virtual Payment Payment { get; set; }
        public virtual TypeEcommerce TypeEcommerce { get; set; }
        public virtual ICollection<ImageDeliveryOrder> ImageDeliveryOrders { get; set; }
        public virtual ICollection<ImageReturnOrder> ImageReturnOrders { get; set; }
        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
    }
}
