using System;
using System.Threading;
using System.Threading.Tasks;
using Monitor.Application.MonitoringChecks.Models;
using Monitor.Application.Interfaces;
using MediatR;
using Monitor.Application.MonitoringChecks.Helpers;

namespace Monitor.Application.MonitoringChecks.Decorators
{
    public class DataStoreDecorator<TIn, TOut> : IPipelineBehavior<TIn, TOut>
    {
        private IChecksRepository _store;

        public DataStoreDecorator(IChecksRepository store)
        {
            _store = store ?? throw new ArgumentNullException(nameof(store));
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
                var prevState = _store.GetCheck(commandResult.CheckModel.Type);
                commandResult.CheckModel = new CheckTimeHelper().SetDurations(commandResult.CheckModel, prevState);

                await _store.Save(commandResult.CheckModel);
            }

            return result;
        }
    }
}