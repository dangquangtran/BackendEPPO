using System;
using System.Collections.Generic;

namespace BusinessObjects.Models
{
    public partial class Blog
    {
        public int BlogId { get; set; }
        public string Title { get; set; }
        public int? PlantId { get; set; }
        public string Content { get; set; }
        public DateTime? CreationDate { get; set; }
        public DateTime? ModificationDate { get; set; }
        public int? ModificationByUserId { get; set; }
        public string ModificationDescription { get; set; }
        public int? UserId { get; set; }
        public int? Status { get; set; }

        public virtual User ModificationByUser { get; set; }
        public virtual Plant Plant { get; set; }
        public virtual User User { get; set; }
    }
}
