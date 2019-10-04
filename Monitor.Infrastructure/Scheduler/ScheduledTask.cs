using Cronos;
using Monitor.Application.Interfaces;
using Monitor.Application.MonitoringChecks.Models;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Monitor.Infrastructure.Scheduler
{
    public class ScheduledTask
    {
        ICommandsProcessor _processor;

        public ScheduledTask
        (
            ICommandsProcessor processor
        )
        {
            _processor = processor ?? throw new ArgumentNullException(nameof(processor));
        }

        public async Task ProcessCheck(CheckTypeEnum checkType, string schedule, CancellationToken cancellationToken)
        {
            var initialDelayTime = TimeSpan.FromMinutes(0);

            await Task.Delay(initialDelayTime);

            while (!cancellationToken.IsCancellationRequested)
            {
                CronExpression expression = CronExpression.Parse(schedule);
                var nextRunTime = expression.GetNextOccurrence(DateTime.UtcNow);
                var delay = nextRunTime - DateTime.UtcNow;

                if (delay == null)
                {
                    return;
                }

                await _processor.ExecuteCommand(checkType);

                if (cancellationToken.IsCancellationRequested)
                {
                    return;
                }

                await Task.Delay(delay.Value);
            }     
        }
    }
}
