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
        private ConcurrentDictionary<CheckTypeEnum, string> _schedulesMap = new ConcurrentDictionary<CheckTypeEnum, string>();


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
            _schedulesMap.TryAdd(check.Type, check.NormalSchedule);
            var checkProcessor = new ScheduledTask(_processor);
            await checkProcessor.ProcessCheck(check.Type, check.NormalSchedule, cancellationTokenSource.Token);
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

        public async Task ReSchedule(CheckSettings settings, StatusesEnum status)
        {
            var newSchedule = GetSchedule(settings, status);
            var currentSchedule = "";
            _schedulesMap.TryGetValue(settings.Type, out currentSchedule);

            if (currentSchedule == newSchedule)
            {
                return;
            }

            CancellationTokenSource cts = null;
            _cancellationTokensMap.TryGetValue(settings.Type, out cts);
            cts.Cancel();

            var cancellationTokenSource = new CancellationTokenSource();
            var ctsUpd = _cancellationTokensMap.TryUpdate(settings.Type, cancellationTokenSource, cts);
            var schUpd = _schedulesMap.TryUpdate(settings.Type, newSchedule, currentSchedule);                       

            var checkProcessor = new ScheduledTask(_processor);
            await checkProcessor.ProcessCheck(settings.Type, newSchedule, cancellationTokenSource.Token);
        }

        private string GetSchedule(CheckSettings settings, StatusesEnum status)
        {
            switch (status)
            {
                case StatusesEnum.CRITICAL:
                    return settings.CriticalSchedule;
                case StatusesEnum.WARNING:
                    return settings.WarningSchedule;
                case StatusesEnum.OK:
                default:
                    return settings.NormalSchedule;
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
