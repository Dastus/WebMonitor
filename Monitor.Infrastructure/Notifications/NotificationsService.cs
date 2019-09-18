using Monitor.Application.Interfaces;
using Monitor.Application.MonitoringChecks.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Monitor.Infrastructure.Notifications
{
    public class NotificationsService : INotificationsService
    {
        private ConcurrentDictionary<CheckTypeEnum, CheckStatusAtTime> _notificationsHistory = new ConcurrentDictionary<CheckTypeEnum, CheckStatusAtTime>();
        private readonly ITelegramNotificationService _telegramNotificationService;

        public NotificationsService
        (
            ITelegramNotificationService telegramNotificationService
        )
        {
            _telegramNotificationService = telegramNotificationService ?? throw new ArgumentNullException(nameof(telegramNotificationService));
        }

        public async Task Notify(Check checkResults)
        {
            if (checkResults.Settings.EnvironmentId != (int)EnvironmentsEnum.Prod)
            {
                return;
            }

            CheckStatusAtTime prevNotification;
            var recordExists = _notificationsHistory.TryGetValue(checkResults.Settings.Type, out prevNotification);

            var currentStatus = checkResults.State.Status;

            if (currentStatus == StatusesEnum.CRITICAL)
            {
                if (!recordExists)
                {
                    await _telegramNotificationService.Notify(checkResults);
                    _notificationsHistory.TryAdd(checkResults.Settings.Type, new CheckStatusAtTime { NotificationTime = DateTime.Now, Status = currentStatus });
                    return;
                }

                if (DateTime.Now - prevNotification.NotificationTime > TimeSpan.FromMinutes(10))
                {
                    await _telegramNotificationService.Notify(checkResults);
                    _notificationsHistory.TryUpdate(checkResults.Settings.Type, new CheckStatusAtTime { NotificationTime = DateTime.Now, Status = currentStatus }, prevNotification);
                }
            }
            else
            {                
                if (!recordExists)
                {
                    //ok, warning.
                    return;
                }
                //recovery
                await _telegramNotificationService.Notify(checkResults);
                _notificationsHistory.TryRemove(checkResults.Settings.Type, out var removed);
            }

            //if (!recordExists && currentStatus == StatusesEnum.CRITICAL)
            //{
            //    await _telegramNotificationService.Notify(checkResults);
            //    _notificationsHistory.TryAdd(checkResults.Settings.Type, new CheckStatusAtTime { NotificationTime = DateTime.Now, Status = currentStatus });
            //    return;
            //}

            //if (prevNotification != null && prevNotification.Status != currentStatus || DateTime.Now - prevNotification.NotificationTime > TimeSpan.FromMinutes(10))
            //{
            //    await _telegramNotificationService.Notify(checkResults);
            //    _notificationsHistory.TryUpdate(checkResults.Settings.Type, new CheckStatusAtTime { NotificationTime = DateTime.Now, Status = currentStatus }, prevNotification);
            //}

            //if (currentStatus != StatusesEnum.CRITICAL)
            //{
            //    _notificationsHistory.TryRemove(checkResults.Settings.Type, out var removed);
            //}
        }
    }

    class CheckStatusAtTime
    {
        public DateTime NotificationTime;
        public StatusesEnum Status;
    }
}
