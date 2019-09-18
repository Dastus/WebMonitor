using MediatR;
using Monitor.Application.Interfaces;
using Monitor.Application.MonitoringChecks.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Monitor.Application.MonitoringChecks.Decorators
{
    public class NotificationsDecorator<TIn, TOut> : IPipelineBehavior<ICommand<CommandResult>, CommandResult>
    where TIn : ICommand<TOut>
    {
        private readonly INotificationsService _notificationsService;

        public NotificationsDecorator(INotificationsService notificationsService)
        {
            _notificationsService = notificationsService ?? throw new ArgumentNullException(nameof(notificationsService));
        }

        public async Task<CommandResult> Handle(ICommand<CommandResult> request, CancellationToken cancellationToken, RequestHandlerDelegate<CommandResult> next)
        {
            var result = await next();

            var sw = new Stopwatch();
            sw.Start();

            await _notificationsService.Notify(result.CheckModel);

            sw.Stop();            
            (result as CommandResult).CheckModel.State.DiagnosticsInfo += " NotificationsDecorator: " + sw.ElapsedMilliseconds;

            return result;
        }
    }
}
