using System;
using System.Collections.Generic;

namespace BusinessObjects.Models
{
    public partial class OrderDetail
    {
        public int OrderDetailId { get; set; }
        public int? OrderId { get; set; }
        public int? PlantId { get; set; }
        public DateTime? RentalStartDate { get; set; }
        public DateTime? RentalEndDate { get; set; }
        public double? NumberMonth { get; set; }
        public virtual Order Order { get; set; }
        public virtual Plant Plant { get; set; }
    }
}
