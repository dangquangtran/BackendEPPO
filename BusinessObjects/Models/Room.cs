using System;
using System.Collections.Generic;

namespace BusinessObjects.Models
{
    public partial class Room
    {
        public Room()
        {
            HistoryBids = new HashSet<HistoryBid>();
            UserRooms = new HashSet<UserRoom>();
        }

        public int RoomId { get; set; }
        public int? PlantId { get; set; }
        public DateTime? RegistrationOpenDate { get; set; }
        public DateTime? RegistrationEndDate { get; set; }
        public double? RegistrationFee { get; set; }
        public double? PriceStep { get; set; }
        public DateTime? CreationDate { get; set; }
        public DateTime? ActiveDate { get; set; }
        public DateTime? EndDate { get; set; }
        public DateTime? ModificationDate { get; set; }
        public int? ModificationBy { get; set; }
        public int? Status { get; set; }

        public virtual User ModificationByNavigation { get; set; }
        public virtual Plant Plant { get; set; }
        public virtual ICollection<HistoryBid> HistoryBids { get; set; }
        public virtual ICollection<UserRoom> UserRooms { get; set; }
    }
}
