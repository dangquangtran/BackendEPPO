using System;
using System.Collections.Generic;

namespace BusinessObjects.Models
{
    public partial class ImageDeliveryOrder
    {
        public int ImageDeliveryOrderId { get; set; }
        public int? OrderId { get; set; }
        public string ImageUrl { get; set; }
        public DateTime? UploadDate { get; set; }

        public virtual Order Order { get; set; }
    }
}
