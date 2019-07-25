
namespace Monitor.Application.MonitoringChecks.Models
{
    public class QueryResult<T> : ApiResponseResult where T : class
    {
        public T Data { get; set; }
    }
}
