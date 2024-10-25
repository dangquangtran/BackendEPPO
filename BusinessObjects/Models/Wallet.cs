using System;
using System.Collections.Generic;

namespace BusinessObjects.Models
{
    public partial class Wallet
    {
        public Wallet()
        {
            Transactions = new HashSet<Transaction>();
            Users = new HashSet<User>();
        }

        public int WalletId { get; set; }
        public double? NumberBalance { get; set; }
        public DateTime? CreationDate { get; set; }
        public DateTime? ModificationDate { get; set; }
        public int? Status { get; set; }

        public virtual ICollection<Transaction> Transactions { get; set; }
        public virtual ICollection<User> Users { get; set; }
    }
}
