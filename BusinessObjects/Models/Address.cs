using System;
using System.Collections.Generic;

namespace BusinessObjects.Models
{
    public partial class Address
    {
        public int AddressId { get; set; }
        public int? UserId { get; set; }
        public string Description { get; set; }
        public DateTime? CreationDate { get; set; }
        public DateTime? ModificationDate { get; set; }
        public int? Status { get; set; }

        public virtual User User { get; set; }
    }
}
