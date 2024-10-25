using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOs.OrderDetail
{
    public class CreateOrderDetailDTO
    {
        public int? Quantity { get; set; }
        public double? TotalPrice { get; set; }
    }
}
