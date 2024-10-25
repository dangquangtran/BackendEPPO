using System;
using System.Collections.Generic;

namespace BusinessObjects.Models
{
    public partial class ContractDetail
    {
        public int ContractDetailId { get; set; }
        public int? ContractId { get; set; }
        public int? PlantId { get; set; }
        public int? Quantity { get; set; }
        public double? TotalPrice { get; set; }
        public bool? IsActive { get; set; }
        public int? Status { get; set; }

        public virtual Contract Contract { get; set; }
        public virtual Plant Plant { get; set; }
    }
}
