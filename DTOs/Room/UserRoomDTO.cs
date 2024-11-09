using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOs.Room
{
    public class UserRoomDTO
    {
        public int UserRoomId { get; set; }
        public int? RoomId { get; set; }
        public int? UserId { get; set; }
        public DateTime? JoinDate { get; set; }
        public bool? IsActive { get; set; }
        public int? Status { get; set; }
    }
    public class CreateUserRoomDTO
    {
        public int? RoomId { get; set; }

    }
    public class DeleteUserRoomDTO
    {
        public int UserRoomId { get; set; }

    }
    public class UpdateUserRoomDTO
    {
        public int UserRoomId { get; set; }
        public int? RoomId { get; set; }
        public int? UserId { get; set; }
        public DateTime? JoinDate { get; set; }
        public bool? IsActive { get; set; }
        public int? Status { get; set; }
    }
}
