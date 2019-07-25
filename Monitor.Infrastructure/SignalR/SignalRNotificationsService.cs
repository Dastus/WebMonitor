using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Monitor.Application.Interfaces;
using Monitor.Application.MonitoringChecks.Models;

namespace Monitor.Infrastructure.SignalR
{
    public class SignalRNotificationsService : ISignalRNotificationsService
    {
        private IHubContext<MonitoringHub, ITypedHubClient> _hubContext;

        public SignalRNotificationsService(IHubContext<MonitoringHub, ITypedHubClient> hubContext)
        {
            _hubContext = hubContext ?? throw new ArgumentNullException(nameof(hubContext));
        }

        public async Task Notify(List<Check> checkResults)
        {
            await _hubContext.Clients.All.BroadcastChecks(checkResults);
        }
    }
}
