using System;
using System.Collections.Generic;

namespace BusinessObjects.Models
{
    public partial class OrderDetail
    {
        public OrderDetail()
        {
            SubOrderDetails = new HashSet<SubOrderDetail>();
        }

        public int OrderDetailId { get; set; }
        public int? OrderId { get; set; }
        public int? Quantity { get; set; }
        public double? TotalPrice { get; set; }
        public int? Status { get; set; }
        public DateTime? CreationDate { get; set; }
        public DateTime? ModificationDate { get; set; }
        public int? ModificationBy { get; set; }
        public string Code { get; set; }

        public virtual User ModificationByNavigation { get; set; }
        public virtual Order Order { get; set; }
        public virtual ICollection<SubOrderDetail> SubOrderDetails { get; set; }
    }
}
