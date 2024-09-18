using System;
using System.Collections.Generic;

namespace BusinessObjects.Models
{
    public partial class PlantVoucher
    {
        public PlantVoucher()
        {
            Orders = new HashSet<Order>();
        }

        public int PlantVoucherId { get; set; }
        public int? PlantId { get; set; }
        public DateTime? CreationDate { get; set; }
        public DateTime? EndDate { get; set; }
        public double? TotalPrice { get; set; }
        public string Description { get; set; }
        public bool? IsOrderVoucher { get; set; }
        public int? Status { get; set; }

        public virtual Plant Plant { get; set; }
        public virtual ICollection<Order> Orders { get; set; }
    }
}
