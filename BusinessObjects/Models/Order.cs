using System;
using System.Collections.Generic;

namespace BusinessObjects.Models
{
    public partial class Order
    {
        public Order()
        {
            Deliveries = new HashSet<Delivery>();
            OrderDetails = new HashSet<OrderDetail>();
        }

        public int OrderId { get; set; }
        public int? UserId { get; set; }
        public string Description { get; set; }
        public double? TotalPrice { get; set; }
        public int? UserVoucherId { get; set; }
        public int? PlantVoucherId { get; set; }
        public double? FinalPrice { get; set; }
        public int? TransactionId { get; set; }
        public int? PaymentId { get; set; }
        public int? PaymentStatus { get; set; }
        public int? Status { get; set; }
        public DateTime? CreationDate { get; set; }
        public DateTime? ModificationDate { get; set; }
        public int? ModificationBy { get; set; }
        public string Code { get; set; }

        public virtual User ModificationByNavigation { get; set; }
        public virtual Payment Payment { get; set; }
        public virtual PlantVoucher PlantVoucher { get; set; }
        public virtual Transaction Transaction { get; set; }
        public virtual UserVoucher UserVoucher { get; set; }
        public virtual ICollection<Delivery> Deliveries { get; set; }
        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
    }
}
