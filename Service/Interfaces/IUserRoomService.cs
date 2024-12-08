using BusinessObjects.Models;
using DTOs.Room;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Interfaces
{
    public interface IUserRoomService
    {
        Task<IEnumerable<UserRoom>> GetListUserRoom(int page, int size);
        Task<IEnumerable<UserRoom>> GetListUserRoomWithUserToken(int page, int size, int userId);
        Task<UserRoom> GetUserRoomByID(int Id);
        Task<UserRoom> GetUserRoomByRoomID(int roomId);

        Task UpdateUserRoom(UpdateUserRoomDTO userRoom);
        Task DeleteUserRoom(DeleteUserRoomDTO userRoom, int userRoomId);
        Task CreateUserRoom(CreateUserRoomDTO userRoom, int userID);

        Task<int> CountUserRegister(int roomId);
        Task<int> CountTimeActive(int roomId);
        Task<int> CountTimeClose(int roomId);

        //Task<bool> ForgotPassword(string email);
       
    }
}
