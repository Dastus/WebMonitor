
namespace Monitor.Application.MonitoringChecks.ChecksLogic.API.Delivery.Models
{
    public class CityRequest
    {
        public int AreaId { get; set; }

        public int DeliveryMethodKey { get; set; }

        public string SettlementKey { get; set; }

        public string SearchString { get; set; }

        public int LanguageId { get; set; } = 2;

        public int DistrictId { get; set; } = 0;

        public int RegionId { get; set; } = 0;
    }
}
