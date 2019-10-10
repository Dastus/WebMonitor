using System.Collections.Generic;

namespace Monitor.Application.MonitoringChecks.ChecksLogic.API.Delivery.Models
{
    public class ApiResponse<T>
    {
        public T Data { get; set; }
        public bool Success { get; set; }
        public List<string> Errors { get; set; } = new List<string>();
    }
}
