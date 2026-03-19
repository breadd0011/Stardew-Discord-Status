namespace StardewStatusApi.Models
{
    public class DiscordSettings
    {
        public string BotToken { get; set; } = string.Empty;
        public string StatusApiKey { get; set; } = string.Empty;
        public ulong GuildId { get; set; }
        public ulong DateChannelId { get; set; }
    }
}