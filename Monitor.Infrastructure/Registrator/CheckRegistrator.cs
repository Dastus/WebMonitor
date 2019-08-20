using System;
using System.Linq;
using MediatR;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Monitor.Application.Interfaces;
using Monitor.Application.MonitoringChecks.Models;
using Monitor.Infrastructure.Scheduler;
using System.Threading;
using System.Collections.Generic;

namespace Monitor.Infrastructure.Registrator
{
    public class CheckRegistrator : IHostedService
    {
        private readonly ICommandsProcessor _checksProcessor;
        private readonly ISchedulerService _scheduler;

        public CheckRegistrator
        (
            ICommandsProcessor checksProcessor,
            ISchedulerService scheduler
        )
        {
            _checksProcessor = checksProcessor ?? throw new ArgumentNullException(nameof(checksProcessor));
            _scheduler = scheduler ?? throw new ArgumentNullException(nameof(scheduler));
        }
        
        public async Task RegisterCheck(CheckSettings check, IRequest<CommandResult> command)
        {
            await _checksProcessor.RegisterCheckProcessor(check, command);
            await _scheduler.AddToSchedule(check);
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            var commandsList = ScanForCommandsList();
            Parallel.ForEach(commandsList, async (x) => await RegisterCheck(x.CheckSettings, x as IRequest<CommandResult>));
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await _scheduler.StopAll();
        }

        private List<ICommand<CommandResult>> ScanForCommandsList()
        {
            var commandType = typeof(ICommand<CommandResult>);
            var requestType = typeof(IRequest<CommandResult>);

            var commands = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(s => s.GetTypes())
                .Where(p => p.IsClass && commandType.IsAssignableFrom(p) && requestType.IsAssignableFrom(p))
                .Select(t => Activator.CreateInstance(t) as ICommand<CommandResult>)
                .ToList();

            return commands;
        }
    }
}
