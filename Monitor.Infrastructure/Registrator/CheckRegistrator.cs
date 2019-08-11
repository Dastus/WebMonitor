using System;
using MediatR;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Monitor.Application.Interfaces;
using Monitor.Application.MonitoringChecks.Models;
using Monitor.Infrastructure.Scheduler;
using System.Threading;
using System.Collections.Generic;
using Monitor.Application.MonitoringChecks.Commands;
using Monitor.Application.MonitoringChecks;

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
        
        public async Task RegisterCheck(Check check, IRequest<CommandResult> command)
        {
            await _checksProcessor.RegisterCheckProcessor(check, command);
            await _scheduler.AddToSchedule(check);
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            var settingsList = new List<(Check check, IRequest<CommandResult> command)>
            {
                (new Check { Type = CheckTypeEnum.HomePageAvailableProd, Schedule = "* * * * *" }, new HomePageCheckCommand()),
                (new Check { Type = CheckTypeEnum.MetaTagsProd, Schedule = "*/3 * * * *" }, new MetatagsCheckCommand()),
                (new Check { Type = CheckTypeEnum.WebUISearchProd, Schedule = "*/2 * * * *" }, new WebUISearchCheckCommand()),
                (new Check { Type = CheckTypeEnum.ApiSearchCheckProd, Schedule = "*/2 * * * *" }, new ApiSearchCheckProdCommand()),

                (new Check { Type = CheckTypeEnum.HomePageAvailableBeta, Schedule = "* * * * *" }, new HomePageCheckBetaCommand()),
                (new Check { Type = CheckTypeEnum.MetaTagsBeta, Schedule = "*/3 * * * *" }, new MetatagsCheckBetaCommand()),
                (new Check { Type = CheckTypeEnum.WebUISearchBeta, Schedule = "*/2 * * * *" }, new WebUISearchCheckBetaCommand()),
                (new Check { Type = CheckTypeEnum.ApiSearchCheckBeta, Schedule = "*/2 * * * *" }, new ApiSearchCheckBetaCommand()),
            };

            Parallel.ForEach(settingsList, async (x) => await RegisterCheck(x.check, x.command));
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await _scheduler.StopAll();
        }
    }
}
