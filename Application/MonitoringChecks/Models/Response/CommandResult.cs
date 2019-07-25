
namespace Monitor.Application.MonitoringChecks.Models
{
    public class CommandResult : ApiResponseResult
    {
        public Check CheckModel { get; set; }
    }    
}