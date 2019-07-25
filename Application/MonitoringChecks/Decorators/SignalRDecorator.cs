using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Monitor.Application.Interfaces;
using Monitor.Application.MonitoringChecks.Models;
using MediatR;

namespace Monitor.Application.MonitoringChecks.Decorators
{
    public class SignalRDecorator<TIn, TOut> : IPipelineBehavior<TIn, TOut>
    {
        private ISignalRNotificationsService _notifier;

        public SignalRDecorator(ISignalRNotificationsService notifier)
        {
            _notifier = notifier ?? throw new ArgumentNullException(nameof(notifier));
        }

        public async Task<TOut> Handle(TIn request, CancellationToken cancellationToken, RequestHandlerDelegate<TOut> next)
        {
            var result = await next();

            //can't configure separate pipeline for queries because of Core IoC limitations
            if (request is IQuery<TOut>)
            {
                return result;
            }

            var commandResult = result as CommandResult;
            if (commandResult.Success)
            {
                await _notifier.Notify(new List<Check> { commandResult.CheckModel });
            }

            return result;
        }
    }
}
