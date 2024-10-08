using System;
using System.Collections.Generic;

namespace BusinessObjects.Models
{
    public partial class SubFeedback
    {
        public int SubFeedbackId { get; set; }
        public int? PlantId { get; set; }
        public int? AccessoryId { get; set; }
        public int? ServiceId { get; set; }

        public virtual Accessory Accessory { get; set; }
        public virtual Plant Plant { get; set; }
        public virtual Epposervice Service { get; set; }
    }
}
