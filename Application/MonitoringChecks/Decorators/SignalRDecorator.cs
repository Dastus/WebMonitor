using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Monitor.Application.Interfaces;
using Monitor.Application.MonitoringChecks.Models;
using MediatR;
using System.Diagnostics;

namespace Monitor.Application.MonitoringChecks.Decorators
{
    public class SignalRDecorator<TIn, TOut> : IPipelineBehavior<TIn, TOut>
        where TIn : ICommand<TOut>
    {
        private ISignalRNotificationsService _notifier;

        public SignalRDecorator(ISignalRNotificationsService notifier)
        {
            _notifier = notifier ?? throw new ArgumentNullException(nameof(notifier));
        }

        public async Task<TOut> Handle(TIn request, CancellationToken cancellationToken, RequestHandlerDelegate<TOut> next)            
        {
            var result = await next();

            var sw = new Stopwatch();//
            sw.Start();//

            var commandResult = result as CommandResult;

            await _notifier.Notify(commandResult.CheckModel);

            sw.Stop();//
            commandResult.CheckModel.State.DiagnosticsInfo += " SignalR: " + sw.ElapsedMilliseconds;

            return result;
        }
    }
}
