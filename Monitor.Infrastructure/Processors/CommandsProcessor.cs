using System;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using Monitor.Application.MonitoringChecks.Models;
using MediatR;
using Monitor.Application.Interfaces;

namespace Monitor.Infrastructure.Processors
{
    public class CommandsProcessor : ICommandsProcessor
    {
        private ConcurrentDictionary<CheckTypeEnum, IRequest<CommandResult>> _handlersDict = new ConcurrentDictionary<CheckTypeEnum, IRequest<CommandResult>>();

        private IMediator _mediator;

        public CommandsProcessor(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
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

        public async Task RegisterCheckProcessor(CheckSettings check, IRequest<CommandResult> command)
        {
            if (_handlersDict.ContainsKey(check.Type))
            {
                throw new ArgumentException("Handler for check type" + check.Type + " already declared!");
            }
            _handlersDict.TryAdd(check.Type, command);
        }
    }
}
