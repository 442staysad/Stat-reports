using Core.Entities;
using Core.Interfaces;

namespace Core.Services
{
    public class NotificationService : INotificationService
    {
        private readonly IRepository<Notification> _notificationRepository;

        public NotificationService(IRepository<Notification> notificationRepository)
        {
            _notificationRepository = notificationRepository;
        }

        public async Task AddNotificationAsync(int userId, string message)
        {
            var notification = new Notification
            {
                UserId = userId,
                Message = message,
                IsRead = false,
                CreatedAt = DateTime.UtcNow
            };

            await _notificationRepository.AddAsync(notification);
        }

        public async Task<IEnumerable<Notification>> GetUserNotificationsAsync(int userId)
        {
            return await _notificationRepository
                .FindAllAsync(n => n.UserId == userId && !n.IsRead);
                
        }

        public async Task MarkAsReadAsync(int notificationId)
        {
            var notification = await _notificationRepository.FindAsync(n => n.Id == notificationId);
            if (notification != null)
            {
                notification.IsRead = true;
                await _notificationRepository.UpdateAsync(notification);
            }
        }

        public async Task<bool> DeleteNotification(int id)
        {
            var notification = await _notificationRepository.FindAsync(n => n.Id == id);
            if (notification != null)
            {
                await _notificationRepository.DeleteAsync(notification);
                return true;
            }
            return false;
        }
    }
}
