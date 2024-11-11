using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOs.OrderDetail
{
    public class CreateOrderDetailDTO
    {
        public int? PlantId { get; set; }
        public double? TotalPrice { get; set; }
    }
}
