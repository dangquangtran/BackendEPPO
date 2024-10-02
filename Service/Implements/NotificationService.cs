using BusinessObjects.Models;
using Repository.Interfaces;
using Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Implements
{
    public class NotificationService : INotificationService
    {
        private readonly IUnitOfWork _unitOfWork;

        public NotificationService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<Notification>> GetListNotification(int page, int size)
        {
            return await _unitOfWork.NotificationRepository.GetAsync(pageIndex: page, pageSize: size);
        }
        public async Task<Notification> GetNotificationByID(int Id)
        {
            return await Task.FromResult(_unitOfWork.NotificationRepository.GetByID(Id));
        }
    }
}
