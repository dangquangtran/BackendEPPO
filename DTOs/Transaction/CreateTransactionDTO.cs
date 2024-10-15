using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOs.Transaction
{
    public class CreateTransactionDTO
    {
        public int? WalletId { get; set; }
        public string Description { get; set; }
        public double? WithdrawNumber { get; set; }
        public double? RechargeNumber { get; set; }
        public int? PaymentId { get; set; }
        public int? TypeEcommerceId { get; set; }
        public DateTime? RechargeDate { get; set; }
        public DateTime? WithdrawDate { get; set; }
    }
}
