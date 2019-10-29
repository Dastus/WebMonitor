using System;
using System.Threading;
using System.Threading.Tasks;
using Monitor.Application.MonitoringChecks.Models;
using Monitor.Application.Interfaces;
using MediatR;
using Monitor.Application.MonitoringChecks.Helpers;
using System.Diagnostics;

namespace Monitor.Application.MonitoringChecks.Decorators
{
    public class DataStoreDecorator<TIn, TOut> : IPipelineBehavior<ICommand<CommandResult>, CommandResult>
        where TIn : ICommand<TOut>
    {
        private IChecksRepository _store;

        public DataStoreDecorator(IChecksRepository store)
        {
            _store = store ?? throw new ArgumentNullException(nameof(store));
        }

        public async Task<CommandResult> Handle(ICommand<CommandResult> request, CancellationToken cancellationToken, RequestHandlerDelegate<CommandResult> next)
        {  
            var result = await next();

            var sw = new Stopwatch();//
            sw.Start();//

            var commandResult = result;           

            var prevState = _store.GetCheck(commandResult.CheckModel.Settings.Type);
            commandResult.CheckModel = new CheckTimeHelper().SetDurations(commandResult.CheckModel, prevState);

            await _store.Save(commandResult.CheckModel);

            sw.Stop();//
            result.CheckModel.State.DiagnosticsInfo += " DataStore: " + sw.ElapsedMilliseconds;

            return result;
        }
    }
}