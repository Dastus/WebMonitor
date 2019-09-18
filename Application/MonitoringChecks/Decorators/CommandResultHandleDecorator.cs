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
    //this class is intended to run specific logic after check (notify, call external api etc)
    public class CommandResultHandleDecorator<TIn, TOut> : IPipelineBehavior<ICommand<CommandResult>, CommandResult>
        where TIn : ICommand<TOut>
    {
        private readonly IResultHandlingService _handler;
        private readonly ISchedulerService _scheduler;

        public CommandResultHandleDecorator(IResultHandlingService handler, ISchedulerService scheduler)
        {
            _handler = handler ?? throw new ArgumentNullException(nameof(handler));
            _scheduler = scheduler ?? throw new ArgumentNullException(nameof(scheduler));
        }

        public async Task<CommandResult> Handle(ICommand<CommandResult> request, CancellationToken cancellationToken, RequestHandlerDelegate<CommandResult> next)
        {
            var result = await next();

            var sw = new Stopwatch();//
            sw.Start();//

            await _scheduler.ReSchedule(result.CheckModel.Settings, result.CheckModel.State.Status);

            try
            {
                await _handler.HandleResult(result as CommandResult);
            }
            catch(Exception e)
            {
                throw new Exception("Error during post-check logic handling: " + e.Message);
            }

            sw.Stop();//            
            (result as CommandResult).CheckModel.State.DiagnosticsInfo += " ResultHandler: " + sw.ElapsedMilliseconds;

            return result;
        }
    }
}
