using Monitor.Application.Interfaces;
using Monitor.Application.MonitoringChecks.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Monitor.Application.MonitoringChecks.ResultsHandlingLogic
{
    public class ResultHandlingService : IResultHandlingService
    {
        private readonly ITelegramNotificationService _notificationsService;

        public ResultHandlingService(ITelegramNotificationService notificationsService)
        {
            _notificationsService = notificationsService;
        }

        public async Task HandleResult(CommandResult result)
        {
            var handler = Resolve(result.CheckModel);
            await handler.HandleResult(result.CheckModel);
        }

        private CheckResultHandlerBase Resolve(Check check)
        {
            switch(check.Settings.Type)
            {
                default:
                    return new CheckResultHandlerBase(_notificationsService);
            }
        }
    }
}
