using System;
using System.Threading;
using System.Threading.Tasks;
using Monitor.Application.MonitoringChecks.Models;
using MediatR;
using Monitor.Application.Interfaces;
using System.Collections.Generic;

namespace Monitor.Application.MonitoringChecks.Decorators
{
    public class LoggerDecorator<TIn, TOut> : IPipelineBehavior<TIn, TOut>
        where TOut : class
    {
        private readonly ILoggerService _logger;
        
        public LoggerDecorator(ILoggerService logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }


        public async Task<TOut> Handle(TIn request, CancellationToken cancellationToken, RequestHandlerDelegate<TOut> next)
        {
            //can't configure separate pipeline for queries because of Core IoC limitations
            if (typeof(TOut) != typeof(CommandResult))
            {
                return await next();
            }
            try
            {
                var result = await next();
                await _logger.SaveLog(result as CommandResult);
                return result;
            }
            catch (Exception ex)
            {
                return new CommandResult { Success = false, Errors = new List<string> { "Exception during Log Save: " + ex.Message } } as TOut;
            }
        }
    }
}