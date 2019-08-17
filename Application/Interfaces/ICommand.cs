using Monitor.Application.MonitoringChecks.Models;

namespace Monitor.Application.Interfaces
{
    public interface ICommand<TOut>
    {
        CheckSettings CheckSettings { get; }
    }
}