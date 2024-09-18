using System;
using System.Collections.Generic;

namespace BusinessObjects.Models
{
    public partial class ImagePlant
    {
        public int ImagePlantId { get; set; }
        public int? PlantId { get; set; }
        public string ImageUrl { get; set; }
        public int? Status { get; set; }

        public virtual Plant Plant { get; set; }
    }
}
