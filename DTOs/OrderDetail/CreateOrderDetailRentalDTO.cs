using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOs.OrderDetail
{
    public class CreateOrderDetailRentalDTO
    {
        public int? PlantId { get; set; }
        public DateTime? RentalStartDate { get; set; }
        public double? NumberMonth { get; set; }
        public double? Deposit { get; set; }
    }
}
