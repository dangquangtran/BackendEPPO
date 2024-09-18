using System;
using System.Collections.Generic;

namespace BusinessObjects.Models
{
    public partial class Service
    {
        public Service()
        {
            SubFeedbacks = new HashSet<SubFeedback>();
            SubOrderDetails = new HashSet<SubOrderDetail>();
        }

        public int ServiceId { get; set; }
        public string ServiceName { get; set; }
        public string Description { get; set; }
        public double? ServicePrice { get; set; }
        public DateTime? CreationDate { get; set; }
        public DateTime? ModificationDate { get; set; }
        public int? ModificationBy { get; set; }
        public string Code { get; set; }
        public bool? IsActive { get; set; }
        public int? Status { get; set; }

        public virtual User ModificationByNavigation { get; set; }
        public virtual ICollection<SubFeedback> SubFeedbacks { get; set; }
        public virtual ICollection<SubOrderDetail> SubOrderDetails { get; set; }
    }
}
