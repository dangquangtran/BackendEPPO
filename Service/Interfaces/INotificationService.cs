using BusinessObjects.Models;
using DTOs.Notification;
using DTOs.User;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Service.Interfaces
{
    public interface INotificationService
    {
        Task<Dictionary<DateTime, List<Notification>>> GetListNotification(int page, int size, int userId);
        Task<Notification> GetNotificationByID(int Id);
        Task UpdateNotification(UpdateNotificationDTO notification);
        Task CreateNotification(CreateNotificationDTO notification);
    }
}
