using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOs.Address
{
    public class RequestAddress
    {
        public int? UserId { get; set; }
        public string Description { get; set; }
        public DateTime? CreationDate { get; set; }
        public DateTime? ModificationDate { get; set; }
        public int? Status { get; set; }

    }
    public class UpdateAddressDTO
    {
        public string Description { get; set; }
    //    public DateTime? CreationDate { get; set; }
        public DateTime? ModificationDate { get; set; }
        public int? Status { get; set; }

    }
    public class DeleteAddressDTO
    {
        public int AddressId { get; set; }

    }
    public class CreateAddressDTO
    {
        public string Description { get; set; }

    }
}
