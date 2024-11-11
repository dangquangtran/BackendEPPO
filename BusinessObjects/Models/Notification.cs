using System;
using System.Collections.Generic;

namespace BusinessObjects.Models
{
    public partial class Notification
    {
        public int NotificationId { get; set; }
        public int? UserId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public bool? IsRead { get; set; }
        public bool? IsNotifications { get; set; }
        public int? Status { get; set; }

        public virtual User User { get; set; }
    }
}
