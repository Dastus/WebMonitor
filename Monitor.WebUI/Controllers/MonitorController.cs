using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MediatR;
using Monitor.Application.MonitoringChecks.Models;
using Monitor.Application.MonitoringChecks.Queries;
using Monitor.Application.Interfaces;

namespace Monitor.WebUI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MonitorController
    {        
        private ICommandsProcessor _commandsProcessor;
        private IMediator _mediator;
        
        public MonitorController
        (
            ICommandsProcessor commandsProcessor,
            IMediator mediator
        )
        {
            _commandsProcessor = commandsProcessor ?? throw new ArgumentNullException(nameof(commandsProcessor));
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        [Route("get-checks")]
        [HttpGet]
        public async Task<IEnumerable<CheckWebModel>> GetChecks(int? environmentId)
        {
            return await _mediator.Send(new GetChecksQuery { EnvironmentId = environmentId });
        }

        [Route("manual-check")]
        [HttpGet]
        public async Task RunManualCheck(CheckTypeEnum checkType)
        {
            await _commandsProcessor.ExecuteCommand(checkType);
        }
    }
}
