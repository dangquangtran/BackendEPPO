using BusinessObjects.Models;
using DTOs.Address;
using DTOs.Notification;
using Repository.Interfaces;
using Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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
        public async Task CreateNotification(CreateNotificationDTO NotificationDTO)
        {
            var entity = new Notification
            {
                UserId = NotificationDTO.UserId,
                Title = NotificationDTO.Title,
                Description = NotificationDTO.Description,
                IsRead = NotificationDTO.IsRead,
                IsNotifications = NotificationDTO.IsNotifications,
                CreatedDate = DateTime.Now,
                UpdatedDate = DateTime.Now,
                Status = 1,
            };
            _unitOfWork.NotificationRepository.Insert(entity);
            await _unitOfWork.SaveAsync();
        }
        public async Task UpdateNotification(UpdateNotificationDTO NotificationDTO)
        {

            var entity = await Task.FromResult(_unitOfWork.NotificationRepository.GetByID(NotificationDTO.NotificationId));

            if (entity == null)
            {
                throw new Exception($"Notification with ID {NotificationDTO.NotificationId} not found.");
            }
            entity.UserId = NotificationDTO.UserId;
            entity.Title = NotificationDTO.Title;
            entity.Description = NotificationDTO.Description;
            entity.IsRead = NotificationDTO.IsRead;
            entity.IsNotifications = NotificationDTO.IsNotifications;
            entity.UpdatedDate = DateTime.Now;
            entity.Status = NotificationDTO.Status;

            _unitOfWork.NotificationRepository.Update(entity);
            await _unitOfWork.SaveAsync();
        }
    }
}
