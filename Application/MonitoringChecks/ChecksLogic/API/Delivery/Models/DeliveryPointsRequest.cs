
namespace Monitor.Application.MonitoringChecks.ChecksLogic.API.Delivery.Models
{
    public class DeliveryPointsRequest
    {
        public string CityKey { get; set; }

        public int DeliveryMethodKey { get; set; }

        public int PaymentMethodId { get; set; }

        public int LanguageId { get; set; } = 2;

        public int DistrictId { get; set; }

        public int RegionId { get; set; }
    }
}
