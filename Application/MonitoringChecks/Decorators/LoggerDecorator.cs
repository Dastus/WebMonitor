using System;
using System.Threading;
using System.Threading.Tasks;
using Monitor.Application.MonitoringChecks.Models;
using MediatR;

namespace Monitor.Application.MonitoringChecks.Decorators
{
    public class LoggerDecorator<TIn, TOut> : IPipelineBehavior<TIn, TOut>
    {
        public async Task<TOut> Handle(TIn request, CancellationToken cancellationToken, RequestHandlerDelegate<TOut> next)
        {
            Console.WriteLine(request.GetType());
            var result = await next();
            Console.WriteLine(result.ToString());
            return result;
        }
    }
}