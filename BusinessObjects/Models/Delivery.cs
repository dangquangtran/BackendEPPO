using System;
using System.Collections.Generic;

namespace BusinessObjects.Models
{
    public partial class Delivery
    {
        public int DeliveryId { get; set; }
        public int? OrderId { get; set; }
        public string Description { get; set; }
        public int? Status { get; set; }
        public DateTime? CreationDate { get; set; }
        public DateTime? ModificationDate { get; set; }
        public int? ModificationBy { get; set; }
        public string Code { get; set; }

        public virtual User ModificationByNavigation { get; set; }
        public virtual Order Order { get; set; }
    }
}
