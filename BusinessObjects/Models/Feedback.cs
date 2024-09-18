using System;
using System.Collections.Generic;

namespace BusinessObjects.Models
{
    public partial class Feedback
    {
        public Feedback()
        {
            ImageFeedbacks = new HashSet<ImageFeedback>();
        }

        public int FeedbackId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime? CreationDate { get; set; }
        public DateTime? ModificationDate { get; set; }
        public int? ModificationByUserId { get; set; }
        public int? Status { get; set; }

        public virtual User ModificationByUser { get; set; }
        public virtual ICollection<ImageFeedback> ImageFeedbacks { get; set; }
    }
}
