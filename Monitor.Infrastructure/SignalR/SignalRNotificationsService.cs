using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.SignalR;
using Monitor.Application.Interfaces;
using Monitor.Application.MonitoringChecks.Models;

namespace Monitor.Infrastructure.SignalR
{
    public class SignalRNotificationsService : ISignalRNotificationsService
    {
        private IHubContext<MonitoringHub, ITypedHubClient> _hubContext;   
        private IMapper _mapper;

        public SignalRNotificationsService(IHubContext<MonitoringHub, ITypedHubClient> hubContext, IMapper mapper)
        {
            _hubContext = hubContext ?? throw new ArgumentNullException(nameof(hubContext));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task Notify(Check checkResult)
        {
            var checkModel = _mapper.Map<Check, CheckWebModel>(checkResult);
            await _hubContext.Clients.All.BroadcastChecks(new List<CheckWebModel> { checkModel });
        }
    }
}
