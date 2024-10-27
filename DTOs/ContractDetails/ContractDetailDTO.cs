using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOs.ContractDetails
{
    public class ContractDetailDTO
    {
        public int ContractDetailId { get; set; }
        public int? ContractId { get; set; }
        public int? PlantId { get; set; }
        public int? Quantity { get; set; }
        public double? TotalPrice { get; set; }
        public bool? IsActive { get; set; }
        public int? Status { get; set; }
    }
    public class CreateContractDetailDTO
    {
        public int ContractDetailId { get; set; }
        public int? ContractId { get; set; }
        public int? PlantId { get; set; }
        public int? Quantity { get; set; }
        public double? TotalPrice { get; set; }
        public bool? IsActive { get; set; }
        public int? Status { get; set; }
    }
    public class UpdateContractDetailDTO
    {
        public int ContractDetailId { get; set; }
        public int? ContractId { get; set; }
        public int? PlantId { get; set; }
        public int? Quantity { get; set; }
        public double? TotalPrice { get; set; }
        public bool? IsActive { get; set; }
        public int? Status { get; set; }
    }
}
