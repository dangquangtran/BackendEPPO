using System;
using System.Collections.Generic;

namespace BusinessObjects.Models
{
    public partial class User
    {
        public User()
        {
            Addresses = new HashSet<Address>();
            Categories = new HashSet<Category>();
            Contracts = new HashSet<Contract>();
            ConversationUserOneNavigations = new HashSet<Conversation>();
            ConversationUserTwoNavigations = new HashSet<Conversation>();
            FeedbackModificationByUsers = new HashSet<Feedback>();
            FeedbackUsers = new HashSet<Feedback>();
            HistoryBids = new HashSet<HistoryBid>();
            Messages = new HashSet<Message>();
            Notifications = new HashSet<Notification>();
            Orders = new HashSet<Order>();
            Plants = new HashSet<Plant>();
            Rooms = new HashSet<Room>();
            UserRooms = new HashSet<UserRoom>();
        }

        public int UserId { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string FullName { get; set; }
        public string Gender { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string ImageUrl { get; set; }
        public int? IdentificationCard { get; set; }
        public int? WalletId { get; set; }
        public int? RoleId { get; set; }
        public string RankLevel { get; set; }
        public bool? IsActive { get; set; }
        public DateTime? CreationDate { get; set; }
        public int? CreationBy { get; set; }
        public DateTime? ModificationDate { get; set; }
        public int? ModificationBy { get; set; }
        public int? Status { get; set; }

        public virtual Role Role { get; set; }
        public virtual Wallet Wallet { get; set; }
        public virtual ICollection<Address> Addresses { get; set; }
        public virtual ICollection<Category> Categories { get; set; }
        public virtual ICollection<Contract> Contracts { get; set; }
        public virtual ICollection<Conversation> ConversationUserOneNavigations { get; set; }
        public virtual ICollection<Conversation> ConversationUserTwoNavigations { get; set; }
        public virtual ICollection<Feedback> FeedbackModificationByUsers { get; set; }
        public virtual ICollection<Feedback> FeedbackUsers { get; set; }
        public virtual ICollection<HistoryBid> HistoryBids { get; set; }
        public virtual ICollection<Message> Messages { get; set; }
        public virtual ICollection<Notification> Notifications { get; set; }
        public virtual ICollection<Order> Orders { get; set; }
        public virtual ICollection<Plant> Plants { get; set; }
        public virtual ICollection<Room> Rooms { get; set; }
        public virtual ICollection<UserRoom> UserRooms { get; set; }
    }
}
