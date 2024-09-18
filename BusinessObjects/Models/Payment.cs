using System;
using System.Collections.Generic;

namespace BusinessObjects.Models
{
    public partial class Payment
    {
        public Payment()
        {
            Orders = new HashSet<Order>();
            Transactions = new HashSet<Transaction>();
        }

        public int PaymentId { get; set; }
        public string PaymentMethod { get; set; }
        public string Description { get; set; }
        public int? Status { get; set; }

        public virtual ICollection<Order> Orders { get; set; }
        public virtual ICollection<Transaction> Transactions { get; set; }
    }
}
