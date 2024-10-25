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
        public int? Quantity { get; set; }
        public double? TotalPrice { get; set; }
    }
}
