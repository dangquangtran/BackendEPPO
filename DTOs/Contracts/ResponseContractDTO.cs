using DTOs.ContractDetails;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOs.Contracts
{
    public class ResponseContractDTO
    {
        public int ContractId { get; set; }
        public int? UserId { get; set; }
        public int? ContractNumber { get; set; }
        public string Description { get; set; }
        public DateTime? CreationContractDate { get; set; }
        public DateTime? EndContractDate { get; set; }
        public double? TotalAmount { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string TypeContract { get; set; }
        public string ContractUrl { get; set; }
        public ulong? IsActive { get; set; }
        public int? Status { get; set; }

    }
    public class CreateContractDTO
    {
        public int? UserId { get; set; }
        public int? ContractNumber { get; set; }
        public string? Description { get; set; }
        public DateTime? CreationContractDate { get; set; }
        public DateTime? EndContractDate { get; set; }
        public double? TotalAmount { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? TypeContract { get; set; }
        public string? ContractUrl { get; set; }
        public List<ContractDetailDTO>? ContractDetails { get; set; }


    }
    public class ContractDetailDTO
    {
        public int? PlantId { get; set; }
    
        public double? TotalPrice { get; set; }
       
    }

    public class ContractDetailDTOOwner
    {
   

    }
    public class UpdateContractDTO
        {
            public int ContractId { get; set; }
            public int? UserId { get; set; }
            public int? ContractNumber { get; set; }
            public string? Description { get; set; }
            public DateTime? CreationContractDate { get; set; }
            public DateTime? EndContractDate { get; set; }
            public double? TotalAmount { get; set; }
            public DateTime? CreatedAt { get; set; }
            public DateTime? UpdatedAt { get; set; }
            public string? TypeContract { get; set; }
            public string? ContractUrl { get; set; }
            public ulong? IsActive { get; set; }
            public int? Status { get; set; }

        }



    public class CreateContractPartnershipDTO
    {
        public int? ContractNumber
        {
            get; set;
        } 
        public string? ContractUrl { get; set; }
        public List<ContractDetailDTOOwner>? ContractDetails { get; set; }


    }

}
