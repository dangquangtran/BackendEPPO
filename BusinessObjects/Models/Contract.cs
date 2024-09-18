using System;
using System.Collections.Generic;

namespace BusinessObjects.Models
{
    public partial class Contract
    {
        public Contract()
        {
            ContractDetails = new HashSet<ContractDetail>();
        }

        public int ContractId { get; set; }
        public int? UserId { get; set; }
        public int? ContractNumber { get; set; }
        public DateTime? CreationContractDate { get; set; }
        public DateTime? EndContractDate { get; set; }
        public decimal? TotalAmount { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public ulong? IsActive { get; set; }
        public int? Status { get; set; }

        public virtual User User { get; set; }
        public virtual ICollection<ContractDetail> ContractDetails { get; set; }
    }
}
