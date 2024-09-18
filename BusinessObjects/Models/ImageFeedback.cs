using System;
using System.Collections.Generic;

namespace BusinessObjects.Models
{
    public partial class ImageFeedback
    {
        public int ImgageFeedbackId { get; set; }
        public int? FeedbackId { get; set; }
        public string ImageUrl { get; set; }
        public int? Status { get; set; }

        public virtual Feedback Feedback { get; set; }
    }
}
