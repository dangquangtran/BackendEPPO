using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOs.Notification
{
    public class NotificationDTO
    {
        public int NotificationId { get; set; }
        public int? UserId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public bool? IsRead { get; set; }
        public bool? IsNotifications { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public int? Status { get; set; }
    }
    public class CreateNotificationDTO
    {
        public int? UserId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public bool? IsRead { get; set; }
        public bool? IsNotifications { get; set; }
        public int? Status { get; set; }


    }
    public class UpdateNotificationDTO
    {
        public int NotificationId { get; set; }
        public int? UserId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public bool? IsRead { get; set; }
        public bool? IsNotifications { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public int? Status { get; set; }
    }
}
