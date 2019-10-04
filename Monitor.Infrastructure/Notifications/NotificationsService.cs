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

        const short NOTIFICATION_DELAY_MIN = 2;
        const short NOTIFICATION_INTERVAL_MIN = 10;

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
                    _notificationsHistory.TryAdd(checkResults.Settings.Type, new CheckStatusAtTime(currentStatus, true));
                    return;
                }

                if (prevNotification.IsFirstAdd && (DateTime.Now - prevNotification.NotificationTime) > TimeSpan.FromMinutes(NOTIFICATION_DELAY_MIN)
                    || (DateTime.Now - prevNotification.NotificationTime) > TimeSpan.FromMinutes(NOTIFICATION_INTERVAL_MIN))
                { 
                    await _telegramNotificationService.Notify(checkResults);
                    var checkStatus = new CheckStatusAtTime(currentStatus, false);
                    _notificationsHistory.TryUpdate(checkResults.Settings.Type, checkStatus, prevNotification);
                }
            }
            else
            {                
                if (!recordExists)
                {
                    //ok, warning.
                    return;
                }
                //suspend sending to avoid spamming 'critical'-'recovery' within 2 minutes notifications
                if (!prevNotification.IsFirstAdd)
                {
                    //recovery
                    await _telegramNotificationService.Notify(checkResults);
                }
                _notificationsHistory.TryRemove(checkResults.Settings.Type, out var removed);
            }
        }
    }

    class CheckStatusAtTime
    {
        public DateTime NotificationTime { get; set; }
        public StatusesEnum Status { get; set; }
        public bool IsFirstAdd { get; set; }

        public CheckStatusAtTime(StatusesEnum status, bool isFirstNotification)
        {
            NotificationTime = DateTime.Now;
            Status = status;
            IsFirstAdd = isFirstNotification;
        }
    }
}
