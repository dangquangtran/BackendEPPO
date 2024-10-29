using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOs.Wallet
{
    public class WalletDTO
    {
        public double? NumberBalance { get; set; }
        public DateTime? CreationDate { get; set; }
        public DateTime? ModificationDate { get; set; }
        public int? Status { get; set; }
    }
    public class CreateWalletDTO
    {
        public int WalletId { get; set; }
        public double? NumberBalance { get; set; }
        public DateTime? CreationDate { get; set; }
        public DateTime? ModificationDate { get; set; }
        public int? Status { get; set; }
    }
    public class UpdateWalletDTO
    {
        public int WalletId { get; set; }
        public double? NumberBalance { get; set; }
        public DateTime? CreationDate { get; set; }
        public DateTime? ModificationDate { get; set; }
        public int? Status { get; set; }
    }
}
