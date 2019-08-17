using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using Monitor.Application.MonitoringChecks.Models;
using MediatR;
using Monitor.Application.Interfaces;

namespace Monitor.Application.MonitoringChecks.Decorators
{
    public class CommandExceptionsDecorator<TIn, TOut> : IPipelineBehavior<ICommand<CommandResult>, CommandResult> 
        where TIn : ICommand<TOut>
    {
        public async Task<CommandResult> Handle(ICommand<CommandResult> request, CancellationToken cancellationToken, RequestHandlerDelegate<CommandResult> next)
        {
            try
            {
                var result = await next();
                return result;
            }
            catch (Exception ex)
            {
                return new CommandResult
                {
                    Success = false,
                    Errors = new List<string> { ex.Message },
                    CheckModel = new Check {
                        Settings = request.CheckSettings,
                        State = new CheckState
                        {
                            Status = StatusesEnum.CRITICAL,
                            Description = "Exception during check execution: " + ex.Message,
                            LastCheckTime = DateTime.Now
                        }
                    }
                };
            }
        }
    }
}
