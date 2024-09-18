using System;
using System.Collections.Generic;

namespace BusinessObjects.Models
{
    public partial class UserVoucher
    {
        public UserVoucher()
        {
            Orders = new HashSet<Order>();
        }

        public int UserVoucherId { get; set; }
        public int? UserId { get; set; }
        public DateTime? CreationDate { get; set; }
        public DateTime? EndDate { get; set; }
        public double? VoucherPrice { get; set; }
        public string Description { get; set; }
        public bool? IsUserVoucher { get; set; }
        public int? Status { get; set; }

        public virtual User User { get; set; }
        public virtual ICollection<Order> Orders { get; set; }
    }
}
