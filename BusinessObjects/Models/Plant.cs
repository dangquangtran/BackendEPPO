using System;
using System.Collections.Generic;

namespace BusinessObjects.Models
{
    public partial class Plant
    {
        public Plant()
        {
            Blogs = new HashSet<Blog>();
            ContractDetails = new HashSet<ContractDetail>();
            ImagePlants = new HashSet<ImagePlant>();
            PlantVouchers = new HashSet<PlantVoucher>();
            Rooms = new HashSet<Room>();
            SubFeedbacks = new HashSet<SubFeedback>();
            SubOrderDetails = new HashSet<SubOrderDetail>();
        }

        public int PlantId { get; set; }
        public string PlantName { get; set; }
        public string Description { get; set; }
        public double Quantity { get; set; }
        public double Price { get; set; }
        public double Discounts { get; set; }
        public double FinalPrice { get; set; }
        public int? CategoryId { get; set; }
        public int? TypeEcommerceId { get; set; }
        public int? Status { get; set; }
        public bool? IsActive { get; set; }
        public DateTime? CreationDate { get; set; }
        public DateTime? ModificationDate { get; set; }
        public int? ModificationBy { get; set; }
        public string Code { get; set; }

        public virtual Category Category { get; set; }
        public virtual User ModificationByNavigation { get; set; }
        public virtual TypeEcommerce TypeEcommerce { get; set; }
        public virtual ICollection<Blog> Blogs { get; set; }
        public virtual ICollection<ContractDetail> ContractDetails { get; set; }
        public virtual ICollection<ImagePlant> ImagePlants { get; set; }
        public virtual ICollection<PlantVoucher> PlantVouchers { get; set; }
        public virtual ICollection<Room> Rooms { get; set; }
        public virtual ICollection<SubFeedback> SubFeedbacks { get; set; }
        public virtual ICollection<SubOrderDetail> SubOrderDetails { get; set; }
    }
}
