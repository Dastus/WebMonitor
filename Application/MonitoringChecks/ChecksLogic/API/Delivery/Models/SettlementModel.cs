
namespace Monitor.Application.MonitoringChecks.ChecksLogic.API.Delivery.Models
{
    public class SettlementModel
    {
        public string NameRus { get; set; }
        public string NameUkr { get; set; }
        public decimal Longitude { get; set; }
        public decimal Latitude { get; set; }
        public int AreaId { get; set; }
        public string CityKey { get; set; }
    }
}
