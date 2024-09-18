using System;
using System.Collections.Generic;

namespace BusinessObjects.Models
{
    public partial class ImageAccessory
    {
        public int ImageAccessoryId { get; set; }
        public int? AccessoryId { get; set; }
        public string ImageUrl { get; set; }
        public int? Status { get; set; }

        public virtual Accessory Accessory { get; set; }
    }
}
