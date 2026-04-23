using Application.DTOs;
using Application.Interfaces;
using Domain_layer.Entities;
using Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public class NotificationService : INotificationService
    {
        public Task<NotificationDto> CreateNotificationAsync(int userId, string message)
        {
            int newId = MockDatabase.Notifications.Any() ? MockDatabase.Notifications.Max(n => n.Id) + 1 : 1;
            var notification = new Notification { Id = newId, UserId = userId, Message = message, IsRead = false, CreatedAt = DateTime.UtcNow };
            MockDatabase.Notifications.Add(notification);

            return Task.FromResult(new NotificationDto
            {
                Id = notification.Id,
                Message = notification.Message,
                IsRead = notification.IsRead,
                CreatedAt = notification.CreatedAt
            });
        }

        public Task<IEnumerable<NotificationDto>> GetUserNotificationsAsync(int userId)
        {
            var items = MockDatabase.Notifications.Where(n => n.UserId == userId).OrderByDescending(n => n.CreatedAt).Select(n => new NotificationDto
            {
                Id = n.Id,
                Message = n.Message,
                IsRead = n.IsRead,
                CreatedAt = n.CreatedAt
            }).ToList();

            return Task.FromResult<IEnumerable<NotificationDto>>(items);
        }

        public Task<bool> MarkAsReadAsync(int notificationId)
        {
            var notification = MockDatabase.Notifications.FirstOrDefault(n => n.Id == notificationId);
            if (notification != null)
            {
                notification.IsRead = true;
                return Task.FromResult(true);
            }
            return Task.FromResult(false);
        }
    }
}
