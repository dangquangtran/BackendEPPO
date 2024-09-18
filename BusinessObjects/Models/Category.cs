using System;
using System.Collections.Generic;

namespace BusinessObjects.Models
{
    public partial class Category
    {
        public Category()
        {
            Plants = new HashSet<Plant>();
        }

        public int CategoryId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime? CreationDate { get; set; }
        public DateTime? ModificationDate { get; set; }
        public int? ModificationById { get; set; }
        public int? Status { get; set; }

        public virtual User ModificationBy { get; set; }
        public virtual ICollection<Plant> Plants { get; set; }
    }
}
