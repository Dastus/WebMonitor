using Microsoft.Extensions.Hosting;
using Monitor.Application.Interfaces;
using Monitor.Application.MonitoringChecks;
using Monitor.Application.MonitoringChecks.Models;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Concurrent;

namespace Monitor.Infrastructure.Scheduler
{
    //TODO: add tasks cancellation on shutdown
    public class SchedulerService : IHostedService
    {
        private IScheduleRepository _scheduleRepository;
        private ICommandsProcessor _processor;
        private ConcurrentDictionary<CheckTypeEnum, CancellationTokenSource> _cancellationTokensMap = new ConcurrentDictionary<CheckTypeEnum, CancellationTokenSource>();

        public SchedulerService(
            ICommandsProcessor processor,
            IScheduleRepository scheduleRepository
        )
        {
            _processor = processor ?? throw new ArgumentNullException(nameof(processor));
            _scheduleRepository = scheduleRepository ?? throw new ArgumentNullException(nameof(scheduleRepository));
        }

        public async Task AddToSchedule(CheckTypeEnum checkType, string schedule)
        {
            try
            {
                var cancellationTokenSource = new CancellationTokenSource();
                _cancellationTokensMap.TryAdd(checkType, cancellationTokenSource);
                var checkProcessor = new ScheduledTask(_processor);
                await checkProcessor.ProcessCheck(checkType, schedule, cancellationTokenSource);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public void RemoveFromSchedule(Check check)
        {
            if (_cancellationTokensMap.ContainsKey(check.Type))
            {
                var cancellationToken = _cancellationTokensMap[check.Type];
                cancellationToken.Cancel();
            }
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await InitializeSchedule();
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            foreach (var token in _cancellationTokensMap.Values)
            {
                token.Cancel();
            }
        }

        private async Task InitializeSchedule()
        {
            var schedule = await _scheduleRepository.GetSchedule();

            Parallel.ForEach(schedule.Values , async (x) => {
                await AddToSchedule(x.CheckType, x.Schedule);
            });
        }
    }
}
