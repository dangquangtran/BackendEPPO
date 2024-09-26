using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOs.User
{
    public class ResponseUserDTO
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string FullName { get; set; }
        public string Gender { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string ImageUrl { get; set; }
        public int? IdentificationCard { get; set; }
        public int? WalletId { get; set; }
        public int? RoleId { get; set; }
        public int? RankId { get; set; }
        public bool? IsActive { get; set; }
        public DateTime? CreationDate { get; set; }
        public int? CreationBy { get; set; }
        public DateTime? ModificationDate { get; set; }
        public int? ModificationBy { get; set; }
        public int? Status { get; set; }
    }

}
