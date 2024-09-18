using System;
using System.Collections.Generic;

namespace BusinessObjects.Models
{
    public partial class TypeEcommerce
    {
        public TypeEcommerce()
        {
            Plants = new HashSet<Plant>();
            Transactions = new HashSet<Transaction>();
        }

        public int TypeEcommerceId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int? Status { get; set; }

        public virtual ICollection<Plant> Plants { get; set; }
        public virtual ICollection<Transaction> Transactions { get; set; }
    }
}
