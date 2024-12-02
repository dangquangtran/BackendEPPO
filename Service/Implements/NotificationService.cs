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

        public async Task<Dictionary<DateTime, List<Notification>>> GetListNotification(int page, int size, int userId)
        {
            var notifications = await _unitOfWork.NotificationRepository.GetAsync(
                filter: x => x.UserId == userId,
                pageIndex: page,
                pageSize: size
            );

            if (notifications == null || !notifications.Any())
                return null;

            // Nhóm theo ngày
            var groupedNotifications = notifications
                .Where(n => n.CreatedDate.HasValue)
                .GroupBy(n => n.CreatedDate.Value.Date) // Lấy theo ngày
                .ToDictionary(g => g.Key, g => g.ToList());

            return groupedNotifications;
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
                CreatedDate = DateTime.UtcNow.AddHours(7),
                UpdatedDate = DateTime.UtcNow.AddHours(7),
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
            entity.UpdatedDate = DateTime.UtcNow.AddHours(7);
            entity.Status = NotificationDTO.Status;

            _unitOfWork.NotificationRepository.Update(entity);
            await _unitOfWork.SaveAsync();
        }
    }
}
