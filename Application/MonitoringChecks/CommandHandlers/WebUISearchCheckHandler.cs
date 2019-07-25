﻿using Monitor.Application.MonitoringChecks.Commands;
using Monitor.Application.MonitoringChecks.Models;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using System;
using Monitor.Application.Interfaces;


namespace Monitor.Application.MonitoringChecks.CommandHandlers
{
    public class WebUISearchCheckHandler : IRequestHandler<WebUISearchCheckCommand, CommandResult>
    {
        private IWebDriversFactory _driversFactory;

        public WebUISearchCheckHandler(IWebDriversFactory driversFactory)
        {
            _driversFactory = driversFactory ?? throw new ArgumentNullException(nameof(driversFactory));
        }

        public async Task<CommandResult> Handle(WebUISearchCheckCommand request, CancellationToken cancellationToken)
        {
            var result = new CommandResult();
            result.Success = true;
            var check = new WebUISearchCheck(_driversFactory);
            result.CheckModel = await check.CheckWebUISearch(EnvironmentsEnum.Prod);

            return result;
        }        
    }
}
