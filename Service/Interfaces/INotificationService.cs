using BusinessObjects.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Service.Interfaces
{
    public interface INotificationService
    {
        Task<IEnumerable<Notification>> GetListNotification(int page, int size);
        Task<Notification> GetNotificationByID(int Id);
    }
}
