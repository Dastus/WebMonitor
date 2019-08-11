using MediatR;
using Monitor.Application.Interfaces;
using Monitor.Application.MonitoringChecks.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Monitor.Application.MonitoringChecks.Decorators
{
    //this class is intended to run specific logic after check (notify, call external api etc)
    public class CommandResultHandleDecorator<TIn, TOut> : IPipelineBehavior<TIn, TOut>
        where TOut : class
    {
        private readonly IResultHandlingService _handler;

        public CommandResultHandleDecorator(IResultHandlingService handler)
        {
            _handler = handler ?? throw new ArgumentNullException(nameof(handler));
        }

        public async Task<TOut> Handle(TIn request, CancellationToken cancellationToken, RequestHandlerDelegate<TOut> next)
        {
            //can't configure separate pipeline for queries because of Core IoC limitations
            if (typeof(TOut) != typeof(CommandResult))
            {
                return await next();
            }

            var result = await next();

            try
            {
                await _handler.HandleResult(result as CommandResult);
            }
            catch(Exception e)
            {
                throw new Exception("Error during post-check logic handling: " + e.Message);
            }
            
            return result;
        }
    }
}
