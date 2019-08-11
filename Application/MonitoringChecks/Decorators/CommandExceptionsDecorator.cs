using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using Monitor.Application.MonitoringChecks.Models;
using MediatR;

namespace Monitor.Application.MonitoringChecks.Decorators
{
    public class CommandExceptionsDecorator<TIn, TOut> : IPipelineBehavior<TIn, TOut> 
        where TOut : class
    {
        public async Task<TOut> Handle(TIn request, CancellationToken cancellationToken, RequestHandlerDelegate<TOut> next)
        {
            if (typeof(TOut) != typeof(CommandResult))
            {
                return await next();
            }

            try
            {
                return await next();
            }
            catch(Exception ex)
            {
                return new CommandResult {
                    Success = false,
                    Errors = new List<string> { ex.Message }
                } as TOut;
            }
        }
    }
}
