using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Monitor.Application.MonitoringChecks;
using Monitor.Application.MonitoringChecks.Commands;
using Monitor.Application.MonitoringChecks.Models;
using MediatR;

namespace Monitor.Application.MonitoringChecks
{
    public class CommandsProcessor : ICommandsProcessor
    {
        private Dictionary<CheckTypeEnum, IRequest<CommandResult>> _handlersDict = new Dictionary<CheckTypeEnum, IRequest<CommandResult>>();

        private IMediator _mediator;

        public CommandsProcessor(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            SetupDict();
        }

        public async Task ExecuteCommand(CheckTypeEnum checkType)
        {
            var command = ResolveCommand(checkType);
            await _mediator.Send(command);
        }

        private IRequest<CommandResult> ResolveCommand(CheckTypeEnum checkType)
        {
            if (!_handlersDict.ContainsKey(checkType))
            {
                throw new Exception($"No command specified for {checkType}");
            }
            return _handlersDict[checkType];
        }

        private void SetupDict()
        {
            _handlersDict = new Dictionary<CheckTypeEnum, IRequest<CommandResult>> {
                { CheckTypeEnum.HomePageAvailableProd, new HomePageCheckCommand()},
                { CheckTypeEnum.MetaTagsProd, new MetatagsCheckCommand()},
                { CheckTypeEnum.WebUISearchProd, new WebUISearchCheckCommand()},
                { CheckTypeEnum.HomePageAvailableBeta, new HomePageCheckBetaCommand()},
                { CheckTypeEnum.MetaTagsBeta, new MetatagsCheckBetaCommand()},
                { CheckTypeEnum.WebUISearchBeta, new WebUISearchCheckBetaCommand()}
            };
        }
    }
}
