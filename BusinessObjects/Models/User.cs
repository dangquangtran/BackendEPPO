using System;
using System.Collections.Generic;

namespace BusinessObjects.Models
{
    public partial class User
    {
        public User()
        {
            Accessories = new HashSet<Accessory>();
            Addresses = new HashSet<Address>();
            BlogModificationByUsers = new HashSet<Blog>();
            BlogUsers = new HashSet<Blog>();
            Categories = new HashSet<Category>();
            Contracts = new HashSet<Contract>();
            ConversationUserOneNavigations = new HashSet<Conversation>();
            ConversationUserTwoNavigations = new HashSet<Conversation>();
            Deliveries = new HashSet<Delivery>();
            Feedbacks = new HashSet<Feedback>();
            HistoryBids = new HashSet<HistoryBid>();
            Messages = new HashSet<Message>();
            Notifications = new HashSet<Notification>();
            OrderDetails = new HashSet<OrderDetail>();
            Orders = new HashSet<Order>();
            Plants = new HashSet<Plant>();
            RoomParticipants = new HashSet<RoomParticipant>();
            Rooms = new HashSet<Room>();
            Services = new HashSet<Service>();
            UserVouchers = new HashSet<UserVoucher>();
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
        public int? RankId { get; set; }
        public bool? IsActive { get; set; }
        public DateTime? CreationDate { get; set; }
        public int? CreationBy { get; set; }
        public DateTime? ModificationDate { get; set; }
        public int? ModificationBy { get; set; }
        public int? Status { get; set; }

        public virtual Rank Rank { get; set; }
        public virtual Role Role { get; set; }
        public virtual Wallet Wallet { get; set; }
        public virtual ICollection<Accessory> Accessories { get; set; }
        public virtual ICollection<Address> Addresses { get; set; }
        public virtual ICollection<Blog> BlogModificationByUsers { get; set; }
        public virtual ICollection<Blog> BlogUsers { get; set; }
        public virtual ICollection<Category> Categories { get; set; }
        public virtual ICollection<Contract> Contracts { get; set; }
        public virtual ICollection<Conversation> ConversationUserOneNavigations { get; set; }
        public virtual ICollection<Conversation> ConversationUserTwoNavigations { get; set; }
        public virtual ICollection<Delivery> Deliveries { get; set; }
        public virtual ICollection<Feedback> Feedbacks { get; set; }
        public virtual ICollection<HistoryBid> HistoryBids { get; set; }
        public virtual ICollection<Message> Messages { get; set; }
        public virtual ICollection<Notification> Notifications { get; set; }
        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
        public virtual ICollection<Order> Orders { get; set; }
        public virtual ICollection<Plant> Plants { get; set; }
        public virtual ICollection<RoomParticipant> RoomParticipants { get; set; }
        public virtual ICollection<Room> Rooms { get; set; }
        public virtual ICollection<Service> Services { get; set; }
        public virtual ICollection<UserVoucher> UserVouchers { get; set; }
    }
}
