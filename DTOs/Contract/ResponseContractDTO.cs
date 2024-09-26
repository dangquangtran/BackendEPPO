using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOs.Contract
{
    public class ResponseContractDTO
    {
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

    }
}
