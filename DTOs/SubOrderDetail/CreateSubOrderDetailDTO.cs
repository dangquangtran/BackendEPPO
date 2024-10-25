using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOs.SubOrderDetail
{
    public class CreateSubOrderDetailDTO
    {
        public int? PlantId { get; set; }
        public int? AccessoryId { get; set; }
        public int? ServiceId { get; set; }
        public int? HistoryBidId { get; set; }
    }
}
