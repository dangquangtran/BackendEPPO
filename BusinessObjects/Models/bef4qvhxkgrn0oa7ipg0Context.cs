using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace BusinessObjects.Models
{
    public partial class bef4qvhxkgrn0oa7ipg0Context : DbContext
    {
        public bef4qvhxkgrn0oa7ipg0Context()
        {
        }

        public bef4qvhxkgrn0oa7ipg0Context(DbContextOptions<bef4qvhxkgrn0oa7ipg0Context> options)
            : base(options)
        {
        }

        public virtual DbSet<Address> Addresses { get; set; }
        public virtual DbSet<Category> Categories { get; set; }
        public virtual DbSet<Contract> Contracts { get; set; }
        public virtual DbSet<ContractDetail> ContractDetails { get; set; }
        public virtual DbSet<Conversation> Conversations { get; set; }
        public virtual DbSet<Feedback> Feedbacks { get; set; }
        public virtual DbSet<HistoryBid> HistoryBids { get; set; }
        public virtual DbSet<ImageFeedback> ImageFeedbacks { get; set; }
        public virtual DbSet<ImagePlant> ImagePlants { get; set; }
        public virtual DbSet<Message> Messages { get; set; }
        public virtual DbSet<Notification> Notifications { get; set; }
        public virtual DbSet<Order> Orders { get; set; }
        public virtual DbSet<OrderDetail> OrderDetails { get; set; }
        public virtual DbSet<Payment> Payments { get; set; }
        public virtual DbSet<Plant> Plants { get; set; }
        public virtual DbSet<Role> Roles { get; set; }
        public virtual DbSet<Room> Rooms { get; set; }
        public virtual DbSet<Transaction> Transactions { get; set; }
        public virtual DbSet<TypeEcommerce> TypeEcommerces { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<UserRoom> UserRooms { get; set; }
        public virtual DbSet<Wallet> Wallets { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseMySQL("Server=bef4qvhxkgrn0oa7ipg0-mysql.services.clever-cloud.com;Uid=us2diblhg4zawg4d\n;Pwd=babZ7q3tl6uyTqzKCRF6;Database=bef4qvhxkgrn0oa7ipg0");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Address>(entity =>
            {
                entity.ToTable("Address");

                entity.HasIndex(e => e.UserId, "UserID");

                entity.Property(e => e.AddressId).HasColumnName("AddressID");

                entity.Property(e => e.CreationDate).HasColumnType("datetime");

                entity.Property(e => e.Description).HasColumnType("text");

                entity.Property(e => e.ModificationDate).HasColumnType("datetime");

                entity.Property(e => e.UserId).HasColumnName("UserID");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Addresses)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("Address_ibfk_1");
            });

            modelBuilder.Entity<Category>(entity =>
            {
                entity.ToTable("Category");

                entity.HasIndex(e => e.ModificationById, "ModificationByID");

                entity.Property(e => e.CategoryId).HasColumnName("CategoryID");

                entity.Property(e => e.CreationDate).HasColumnType("datetime");

                entity.Property(e => e.Description).HasColumnType("text");

                entity.Property(e => e.ModificationById).HasColumnName("ModificationByID");

                entity.Property(e => e.ModificationDate).HasColumnType("datetime");

                entity.Property(e => e.Title).HasMaxLength(255);

                entity.HasOne(d => d.ModificationBy)
                    .WithMany(p => p.Categories)
                    .HasForeignKey(d => d.ModificationById)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("Category_ibfk_1");
            });

            modelBuilder.Entity<Contract>(entity =>
            {
                entity.ToTable("Contract");

                entity.HasIndex(e => e.UserId, "FK_User");

                entity.Property(e => e.ContractId).HasColumnName("ContractID");

                entity.Property(e => e.ContractFileName).HasMaxLength(255);

                entity.Property(e => e.ContractUrl)
                    .HasMaxLength(255)
                    .HasColumnName("ContractURL");

                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

                entity.Property(e => e.CreationContractDate).HasColumnType("datetime");

                entity.Property(e => e.Description).HasMaxLength(255);

                entity.Property(e => e.EndContractDate).HasColumnType("datetime");

                entity.Property(e => e.IsActive).HasColumnType("bit(1)");

                entity.Property(e => e.TypeContract).HasMaxLength(255);

                entity.Property(e => e.UpdatedAt).HasColumnType("datetime");

                entity.Property(e => e.UserId).HasColumnName("UserID");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Contracts)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK_User");
            });

            modelBuilder.Entity<ContractDetail>(entity =>
            {
                entity.ToTable("ContractDetail");

                entity.HasIndex(e => e.ContractId, "ContractID");

                entity.HasIndex(e => e.PlantId, "PlantID");

                entity.Property(e => e.ContractDetailId).HasColumnName("ContractDetailID");

                entity.Property(e => e.ContractId).HasColumnName("ContractID");

                entity.Property(e => e.PlantId).HasColumnName("PlantID");

                entity.HasOne(d => d.Contract)
                    .WithMany(p => p.ContractDetails)
                    .HasForeignKey(d => d.ContractId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("ContractDetail_ibfk_3");

                entity.HasOne(d => d.Plant)
                    .WithMany(p => p.ContractDetails)
                    .HasForeignKey(d => d.PlantId)
                    .HasConstraintName("ContractDetail_ibfk_2");
            });

            modelBuilder.Entity<Conversation>(entity =>
            {
                entity.ToTable("Conversation");

                entity.HasIndex(e => e.UserOne, "UserOne");

                entity.HasIndex(e => e.UserTwo, "UserTwo");

                entity.Property(e => e.ConversationId).HasColumnName("ConversationID");

                entity.Property(e => e.CreationDate).HasColumnType("datetime");

                entity.HasOne(d => d.UserOneNavigation)
                    .WithMany(p => p.ConversationUserOneNavigations)
                    .HasForeignKey(d => d.UserOne)
                    .HasConstraintName("Conversation_ibfk_1");

                entity.HasOne(d => d.UserTwoNavigation)
                    .WithMany(p => p.ConversationUserTwoNavigations)
                    .HasForeignKey(d => d.UserTwo)
                    .HasConstraintName("Conversation_ibfk_2");
            });

            modelBuilder.Entity<Feedback>(entity =>
            {
                entity.ToTable("Feedback");

                entity.HasIndex(e => e.ModificationByUserId, "ModificationByUserID");

                entity.HasIndex(e => e.PlantId, "PlantID");

                entity.HasIndex(e => e.UserId, "UserID");

                entity.Property(e => e.FeedbackId).HasColumnName("FeedbackID");

                entity.Property(e => e.CreationDate).HasColumnType("datetime");

                entity.Property(e => e.Description).HasColumnType("text");

                entity.Property(e => e.ModificationByUserId).HasColumnName("ModificationByUserID");

                entity.Property(e => e.ModificationDate).HasColumnType("datetime");

                entity.Property(e => e.PlantId).HasColumnName("PlantID");

                entity.Property(e => e.Title)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.Property(e => e.UserId).HasColumnName("UserID");

                entity.HasOne(d => d.ModificationByUser)
                    .WithMany(p => p.FeedbackModificationByUsers)
                    .HasForeignKey(d => d.ModificationByUserId)
                    .HasConstraintName("Feedback_ibfk_1");

                entity.HasOne(d => d.Plant)
                    .WithMany(p => p.Feedbacks)
                    .HasForeignKey(d => d.PlantId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("Feedback_ibfk_2");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.FeedbackUsers)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("Feedback_ibfk_3");
            });

            modelBuilder.Entity<HistoryBid>(entity =>
            {
                entity.ToTable("HistoryBid");

                entity.HasIndex(e => e.RoomId, "RoomID");

                entity.HasIndex(e => e.UserId, "UserID");

                entity.Property(e => e.HistoryBidId).HasColumnName("HistoryBidID");

                entity.Property(e => e.BidTime).HasColumnType("datetime");

                entity.Property(e => e.Code).HasMaxLength(255);

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.EndDate).HasColumnType("datetime");

                entity.Property(e => e.RoomId).HasColumnName("RoomID");

                entity.Property(e => e.UserId).HasColumnName("UserID");

                entity.HasOne(d => d.Room)
                    .WithMany(p => p.HistoryBids)
                    .HasForeignKey(d => d.RoomId)
                    .HasConstraintName("HistoryBid_ibfk_2");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.HistoryBids)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("HistoryBid_ibfk_1");
            });

            modelBuilder.Entity<ImageFeedback>(entity =>
            {
                entity.HasKey(e => e.ImgageFeedbackId)
                    .HasName("PRIMARY");

                entity.ToTable("ImageFeedback");

                entity.HasIndex(e => e.FeedbackId, "FeedbackID");

                entity.Property(e => e.ImgageFeedbackId).HasColumnName("ImgageFeedbackID");

                entity.Property(e => e.FeedbackId).HasColumnName("FeedbackID");

                entity.Property(e => e.ImageUrl).HasMaxLength(255);

                entity.HasOne(d => d.Feedback)
                    .WithMany(p => p.ImageFeedbacks)
                    .HasForeignKey(d => d.FeedbackId)
                    .HasConstraintName("ImageFeedback_ibfk_1");
            });

            modelBuilder.Entity<ImagePlant>(entity =>
            {
                entity.ToTable("ImagePlant");

                entity.HasIndex(e => e.PlantId, "PlantID");

                entity.Property(e => e.ImagePlantId).HasColumnName("ImagePlantID");

                entity.Property(e => e.ImageUrl).HasMaxLength(255);

                entity.Property(e => e.PlantId).HasColumnName("PlantID");

                entity.HasOne(d => d.Plant)
                    .WithMany(p => p.ImagePlants)
                    .HasForeignKey(d => d.PlantId)
                    .HasConstraintName("ImagePlant_ibfk_1");
            });

            modelBuilder.Entity<Message>(entity =>
            {
                entity.ToTable("Message");

                entity.HasIndex(e => e.ConversationId, "ConversationID");

                entity.HasIndex(e => e.UserId, "UserID");

                entity.Property(e => e.MessageId).HasColumnName("MessageID");

                entity.Property(e => e.ConversationId).HasColumnName("ConversationID");

                entity.Property(e => e.CreationDate).HasColumnType("datetime");

                entity.Property(e => e.ImageLink).HasMaxLength(255);

                entity.Property(e => e.Message1)
                    .HasColumnType("text")
                    .HasColumnName("Message");

                entity.Property(e => e.Type).HasMaxLength(50);

                entity.Property(e => e.UpdateDate).HasColumnType("datetime");

                entity.Property(e => e.UserId).HasColumnName("UserID");

                entity.HasOne(d => d.Conversation)
                    .WithMany(p => p.Messages)
                    .HasForeignKey(d => d.ConversationId)
                    .HasConstraintName("Message_ibfk_1");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Messages)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("Message_ibfk_2");
            });

            modelBuilder.Entity<Notification>(entity =>
            {
                entity.ToTable("Notification");

                entity.HasIndex(e => e.UserId, "UserID");

                entity.Property(e => e.NotificationId).HasColumnName("NotificationID");

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.Description).HasColumnType("text");

                entity.Property(e => e.Title).HasMaxLength(255);

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

                entity.Property(e => e.UserId).HasColumnName("UserID");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Notifications)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("Notification_ibfk_1");
            });

            modelBuilder.Entity<Order>(entity =>
            {
                entity.ToTable("Order");

                entity.HasIndex(e => e.ModificationBy, "ModificationBy");

                entity.HasIndex(e => e.PaymentId, "PaymentID");

                entity.Property(e => e.OrderId).HasColumnName("OrderID");

                entity.Property(e => e.Code).HasMaxLength(50);

                entity.Property(e => e.CreationDate).HasColumnType("datetime");

                entity.Property(e => e.DeliveryAddress).HasMaxLength(255);

                entity.Property(e => e.Description).HasMaxLength(255);

                entity.Property(e => e.ModificationDate).HasColumnType("datetime");

                entity.Property(e => e.PaymentId).HasColumnName("PaymentID");

                entity.Property(e => e.PaymentStatus).HasMaxLength(255);

                entity.Property(e => e.UserId).HasColumnName("UserID");

                entity.HasOne(d => d.ModificationByNavigation)
                    .WithMany(p => p.Orders)
                    .HasForeignKey(d => d.ModificationBy)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("Order_ibfk_1");

                entity.HasOne(d => d.Payment)
                    .WithMany(p => p.Orders)
                    .HasForeignKey(d => d.PaymentId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("Order_ibfk_2");
            });

            modelBuilder.Entity<OrderDetail>(entity =>
            {
                entity.ToTable("OrderDetail");

                entity.HasIndex(e => e.OrderId, "OrderID");

                entity.HasIndex(e => e.PlantId, "PlantID");

                entity.Property(e => e.OrderDetailId).HasColumnName("OrderDetailID");

                entity.Property(e => e.OrderId).HasColumnName("OrderID");

                entity.Property(e => e.PlantId).HasColumnName("PlantID");

                entity.HasOne(d => d.Order)
                    .WithMany(p => p.OrderDetails)
                    .HasForeignKey(d => d.OrderId)
                    .HasConstraintName("OrderDetail_ibfk_1");

                entity.HasOne(d => d.Plant)
                    .WithMany(p => p.OrderDetails)
                    .HasForeignKey(d => d.PlantId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("OrderDetail_ibfk_2");
            });

            modelBuilder.Entity<Payment>(entity =>
            {
                entity.ToTable("Payment");

                entity.Property(e => e.PaymentId).HasColumnName("PaymentID");

                entity.Property(e => e.Description).HasMaxLength(255);

                entity.Property(e => e.PaymentMethod).HasMaxLength(255);
            });

            modelBuilder.Entity<Plant>(entity =>
            {
                entity.ToTable("Plant");

                entity.HasIndex(e => e.CategoryId, "CategoryID");

                entity.HasIndex(e => e.ModificationBy, "ModificationBy");

                entity.HasIndex(e => e.TypeEcommerceId, "TypeEcommerceID");

                entity.Property(e => e.PlantId).HasColumnName("PlantID");

                entity.Property(e => e.CategoryId).HasColumnName("CategoryID");

                entity.Property(e => e.Code).HasMaxLength(255);

                entity.Property(e => e.CreationDate).HasColumnType("datetime");

                entity.Property(e => e.Description).HasColumnType("text");

                entity.Property(e => e.MainImage).HasMaxLength(255);

                entity.Property(e => e.ModificationDate).HasColumnType("datetime");

                entity.Property(e => e.PlantName)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.Property(e => e.Title).HasMaxLength(255);

                entity.Property(e => e.TypeEcommerceId).HasColumnName("TypeEcommerceID");

                entity.HasOne(d => d.Category)
                    .WithMany(p => p.Plants)
                    .HasForeignKey(d => d.CategoryId)
                    .HasConstraintName("Plant_ibfk_1");

                entity.HasOne(d => d.ModificationByNavigation)
                    .WithMany(p => p.Plants)
                    .HasForeignKey(d => d.ModificationBy)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("Plant_ibfk_3");

                entity.HasOne(d => d.TypeEcommerce)
                    .WithMany(p => p.Plants)
                    .HasForeignKey(d => d.TypeEcommerceId)
                    .HasConstraintName("Plant_ibfk_2");
            });

            modelBuilder.Entity<Role>(entity =>
            {
                entity.ToTable("Role");

                entity.Property(e => e.RoleId).HasColumnName("RoleID");

                entity.Property(e => e.Description).HasMaxLength(255);

                entity.Property(e => e.NameRole)
                    .IsRequired()
                    .HasMaxLength(255);
            });

            modelBuilder.Entity<Room>(entity =>
            {
                entity.ToTable("Room");

                entity.HasIndex(e => e.ModificationBy, "ModificationBy");

                entity.HasIndex(e => e.PlantId, "PlantID");

                entity.Property(e => e.RoomId).HasColumnName("RoomID");

                entity.Property(e => e.ActiveDate).HasColumnType("datetime");

                entity.Property(e => e.CreationDate).HasColumnType("datetime");

                entity.Property(e => e.EndDate).HasColumnType("datetime");

                entity.Property(e => e.ModificationDate).HasColumnType("datetime");

                entity.Property(e => e.PlantId).HasColumnName("PlantID");

                entity.Property(e => e.RegistrationEndDate).HasColumnType("datetime");

                entity.Property(e => e.RegistrationOpenDate).HasColumnType("datetime");

                entity.HasOne(d => d.ModificationByNavigation)
                    .WithMany(p => p.Rooms)
                    .HasForeignKey(d => d.ModificationBy)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("Room_ibfk_2");

                entity.HasOne(d => d.Plant)
                    .WithMany(p => p.Rooms)
                    .HasForeignKey(d => d.PlantId)
                    .HasConstraintName("Room_ibfk_1");
            });

            modelBuilder.Entity<Transaction>(entity =>
            {
                entity.ToTable("Transaction");

                entity.HasIndex(e => e.PaymentId, "PaymentID");

                entity.HasIndex(e => e.WalletId, "WalletID");

                entity.Property(e => e.TransactionId).HasColumnName("TransactionID");

                entity.Property(e => e.CreationDate).HasColumnType("datetime");

                entity.Property(e => e.Description).HasMaxLength(255);

                entity.Property(e => e.PaymentId).HasColumnName("PaymentID");

                entity.Property(e => e.RechargeDate).HasColumnType("datetime");

                entity.Property(e => e.WalletId).HasColumnName("WalletID");

                entity.Property(e => e.WithdrawDate).HasColumnType("datetime");

                entity.HasOne(d => d.Payment)
                    .WithMany(p => p.Transactions)
                    .HasForeignKey(d => d.PaymentId)
                    .HasConstraintName("Transaction_ibfk_2");

                entity.HasOne(d => d.Wallet)
                    .WithMany(p => p.Transactions)
                    .HasForeignKey(d => d.WalletId)
                    .HasConstraintName("Transaction_ibfk_1");
            });

            modelBuilder.Entity<TypeEcommerce>(entity =>
            {
                entity.ToTable("TypeEcommerce");

                entity.Property(e => e.TypeEcommerceId).HasColumnName("TypeEcommerceID");

                entity.Property(e => e.Description).HasColumnType("text");

                entity.Property(e => e.Title).HasMaxLength(255);
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("User");

                entity.HasIndex(e => e.RoleId, "RoleID");

                entity.HasIndex(e => e.WalletId, "WalletID");

                entity.Property(e => e.UserId).HasColumnName("UserID");

                entity.Property(e => e.CreationDate).HasColumnType("datetime");

                entity.Property(e => e.DateOfBirth).HasColumnType("datetime");

                entity.Property(e => e.Email).HasMaxLength(255);

                entity.Property(e => e.FullName).HasMaxLength(255);

                entity.Property(e => e.Gender).HasMaxLength(10);

                entity.Property(e => e.ImageUrl)
                    .HasMaxLength(255)
                    .HasColumnName("ImageURL");

                entity.Property(e => e.ModificationDate).HasColumnType("datetime");

                entity.Property(e => e.Password)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.Property(e => e.PhoneNumber).HasMaxLength(20);

                entity.Property(e => e.RankLevel).HasMaxLength(255);

                entity.Property(e => e.RoleId).HasColumnName("RoleID");

                entity.Property(e => e.UserName)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.Property(e => e.WalletId).HasColumnName("WalletID");

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.Users)
                    .HasForeignKey(d => d.RoleId)
                    .HasConstraintName("User_ibfk_2");

                entity.HasOne(d => d.Wallet)
                    .WithMany(p => p.Users)
                    .HasForeignKey(d => d.WalletId)
                    .HasConstraintName("User_ibfk_1");
            });

            modelBuilder.Entity<UserRoom>(entity =>
            {
                entity.ToTable("UserRoom");

                entity.HasIndex(e => e.RoomId, "RoomID");

                entity.HasIndex(e => e.UserId, "UserID");

                entity.Property(e => e.UserRoomId).HasColumnName("UserRoomID");

                entity.Property(e => e.JoinDate).HasColumnType("datetime");

                entity.Property(e => e.RoomId).HasColumnName("RoomID");

                entity.Property(e => e.UserId).HasColumnName("UserID");

                entity.HasOne(d => d.Room)
                    .WithMany(p => p.UserRooms)
                    .HasForeignKey(d => d.RoomId)
                    .HasConstraintName("UserRoom_ibfk_1");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.UserRooms)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("UserRoom_ibfk_2");
            });

            modelBuilder.Entity<Wallet>(entity =>
            {
                entity.ToTable("Wallet");

                entity.Property(e => e.WalletId).HasColumnName("WalletID");

                entity.Property(e => e.CreationDate).HasColumnType("datetime");

                entity.Property(e => e.ModificationDate).HasColumnType("datetime");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
