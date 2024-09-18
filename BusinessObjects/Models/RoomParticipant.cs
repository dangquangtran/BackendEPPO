using System;
using System.Collections.Generic;

namespace BusinessObjects.Models
{
    public partial class RoomParticipant
    {
        public int RoomParticipantId { get; set; }
        public int? RoomId { get; set; }
        public int? UserId { get; set; }
        public DateTime? JoinDate { get; set; }
        public bool? IsActive { get; set; }
        public int? Status { get; set; }

        public virtual Room Room { get; set; }
        public virtual User User { get; set; }
    }
}
