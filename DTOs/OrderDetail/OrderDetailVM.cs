using DTOs.Plant;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOs.OrderDetail
{
    public class OrderDetailVM
    {
        public int OrderDetailId { get; set; }
        public int? OrderId { get; set; }
        public int? PlantId { get; set; }
        public DateTime? RentalStartDate { get; set; }
        public DateTime? RentalEndDate { get; set; }
        public double? NumberMonth { get; set; }
        public double? Deposit { get; set; }
        public string DepositDescription { get; set; }
        public double? DepositReturnCustomer { get; set; }
        public double? DepositReturnOwner { get; set; }
        public PlantVM Plant { get; set; }
        public bool? IsReturnSoon { get; set; }
        public string? ReturnDescription { get; set; }
    }
}
