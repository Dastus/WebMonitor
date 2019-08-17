using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using Monitor.Application.Interfaces;
using Monitor.Application.MonitoringChecks.Models;

namespace Monitor.Infrastructure.Scheduler
{
    public class SchedulerService : ISchedulerService
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

        public async Task AddToSchedule(CheckSettings check)
        {
            var cancellationTokenSource = new CancellationTokenSource();
            _cancellationTokensMap.TryAdd(check.Type, cancellationTokenSource);
            var checkProcessor = new ScheduledTask(_processor);
            await checkProcessor.ProcessCheck(check.Type, check.NormalSchedule, cancellationTokenSource);
        }

        public void RemoveFromSchedule(CheckSettings check)
        {
            if (_cancellationTokensMap.ContainsKey(check.Type))
            {
                var cancellationToken = _cancellationTokensMap[check.Type];
                cancellationToken.Cancel();
            }
        }

        public async Task StopAll()
        {
            foreach (var token in _cancellationTokensMap.Values)
            {
                token.Cancel();
            }
        }

        //private async Task InitializeSchedule()
        //{
        //    var schedule = await _scheduleRepository.GetSchedule();

        //    Parallel.ForEach(schedule.Values , async (x) => {
        //        await AddToSchedule(x.CheckType, x.Schedule);
        //    });
        //}
    }
}
