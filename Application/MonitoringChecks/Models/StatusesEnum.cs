namespace Monitor.Application.MonitoringChecks.Models
{
    public enum StatusesEnum
    {
        OK = 0,
        WARNING = 1,
        CRITICAL = 2
    }

    public static class StatusesEnumExtension
    {
        public  static string ToFriendlyString(this StatusesEnum status)
        {
            switch(status)
            {
                case StatusesEnum.OK:
                    return "OK";
                case StatusesEnum.CRITICAL:
                    return "CRITICAL";
                case StatusesEnum.WARNING:
                    return "WARNING";
                default:
                    return "UNKNOWN";
            }
        }
    }
}