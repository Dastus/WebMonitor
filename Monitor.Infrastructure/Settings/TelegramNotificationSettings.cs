
namespace Monitor.Infrastructure.Settings
{
    public class TelegramNotificationSettings
    {
        public string BotName { get; set; }
        public string BotKey { get; set; }
        public string SendMessageEndpoint { get; set; }
        public long ChatId { get; set; }
    }
}
