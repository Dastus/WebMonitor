
namespace Monitor.Application.MonitoringChecks.ChecksLogic.API.Delivery.Models
{
    public class Area
    {
        public int AreaId { get; set; }
        public int NearestStoreAreaId { get; set; }
        public string NearestStoreAreaKey { get; set; }
        public string AreaName { get; set; }
        public string AreaNameUkr { get; set; }
        public string AreaNameRus { get; set; }
        public bool IsFavorite { get; set; }

    }
}
