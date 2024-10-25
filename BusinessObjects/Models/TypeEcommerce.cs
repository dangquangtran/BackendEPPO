using System;
using System.Collections.Generic;

namespace BusinessObjects.Models
{
    public partial class TypeEcommerce
    {
        public TypeEcommerce()
        {
            Plants = new HashSet<Plant>();
        }

        public int TypeEcommerceId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int? Status { get; set; }

        public virtual ICollection<Plant> Plants { get; set; }
    }
}
