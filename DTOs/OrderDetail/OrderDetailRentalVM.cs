using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOs.OrderDetail
{
    public class OrderDetailRentalVM
    {
        public int OrderDetailId { get; set; }
        public int? OrderId { get; set; }
        public int? PlantId { get; set; }
        public DateTime? RentalStartDate { get; set; }
        public DateTime? RentalEndDate { get; set; }
        public double? NumberMonth { get; set; }
    }
}
