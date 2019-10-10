
namespace Monitor.Application.MonitoringChecks.ChecksLogic.API.Delivery.Models
{
    public class DeliveryPointModel
    {
        public string RefKey { get; set; }
        public string ShortNameUkr { get; set; }
        public string ShortNameRus { get; set; }
        public string AddressUkr { get; set; }
        public string AddressRus { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        //optional parameters
        public string Icon { get; set; }
        public string PictureUrl { get; set; }
    }
}
