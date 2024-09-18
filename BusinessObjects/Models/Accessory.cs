using System;
using System.Collections.Generic;

namespace BusinessObjects.Models
{
    public partial class Accessory
    {
        public Accessory()
        {
            ImageAccessories = new HashSet<ImageAccessory>();
            SubFeedbacks = new HashSet<SubFeedback>();
            SubOrderDetails = new HashSet<SubOrderDetail>();
        }

        public int AccessoryId { get; set; }
        public string AccessoryName { get; set; }
        public string Description { get; set; }
        public double? Price { get; set; }
        public double? Quantity { get; set; }
        public double? Discounts { get; set; }
        public string Title { get; set; }
        public int? Status { get; set; }
        public bool? IsActive { get; set; }
        public DateTime? CreationDate { get; set; }
        public DateTime? ModificationDate { get; set; }
        public int? ModificationBy { get; set; }
        public string Code { get; set; }

        public virtual User ModificationByNavigation { get; set; }
        public virtual ICollection<ImageAccessory> ImageAccessories { get; set; }
        public virtual ICollection<SubFeedback> SubFeedbacks { get; set; }
        public virtual ICollection<SubOrderDetail> SubOrderDetails { get; set; }
    }
}
