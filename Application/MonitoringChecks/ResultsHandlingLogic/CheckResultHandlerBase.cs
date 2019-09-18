using Monitor.Application.Interfaces;
using Monitor.Application.MonitoringChecks.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Monitor.Application.MonitoringChecks.ResultsHandlingLogic
{
    public class CheckResultHandlerBase
    {
        private readonly INotificationsService _notificationsService;

        public CheckResultHandlerBase(
            INotificationsService notificationsService
        )
        {
            _notificationsService = notificationsService;
        }

        public async Task HandleResult(Check check)
        {
            //here should be implementation of some action that needs to be performed
            //after check
            //await _notificationsService.Notify(check);
        }
    }
}
