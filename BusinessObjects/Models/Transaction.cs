using System;
using System.Collections.Generic;

namespace BusinessObjects.Models
{
    public partial class Transaction
    {
        public int TransactionId { get; set; }
        public int? WalletId { get; set; }
        public string Description { get; set; }
        public double? WithdrawNumber { get; set; }
        public double? RechargeNumber { get; set; }
        public int? PaymentId { get; set; }
        public bool? IsActive { get; set; }
        public DateTime? RechargeDate { get; set; }
        public DateTime? WithdrawDate { get; set; }
        public DateTime? CreationDate { get; set; }
        public int? Status { get; set; }

        public virtual Payment Payment { get; set; }
        public virtual Wallet Wallet { get; set; }
    }
}
