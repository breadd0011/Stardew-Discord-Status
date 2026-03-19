using Discord.WebSocket;
using StardewStatusApi.Models;

namespace StardewStatusApi.Services
{
    internal sealed class DiscordService
    {
        private readonly DiscordSocketClient _client;
        private readonly DiscordSettings _settings;

        private string? _lastDateChannelName;
        private DateTimeOffset _lastRenameAt = DateTimeOffset.MinValue;

        public DiscordService(DiscordSocketClient client, DiscordSettings settings)
        {
            _client = client;
            _settings = settings;
        }

        public async Task<(bool Success, string Message, string ChannelName)> UpdateDateChannelAsync(string newName)
        {
            SocketGuild? guild = _client.GetGuild(_settings.GuildId);
            if (guild is null)
                return (false, "Bot could not find the guild.", "");

            SocketGuildChannel? channel = guild.GetChannel(_settings.DateChannelId);
            if (channel is null)
                return (false, "Bot could not find the channel.", "");

            if (_lastDateChannelName == newName || channel.Name == newName)
            {
                _lastDateChannelName = newName;
                return (true, "No change needed", newName);
            }

            if (DateTimeOffset.UtcNow - _lastRenameAt < TimeSpan.FromSeconds(10))
            {
                return (true, "Rename skipped due to cooldown", channel.Name);
            }

            await channel.ModifyAsync(props => props.Name = newName);

            _lastDateChannelName = newName;
            _lastRenameAt = DateTimeOffset.UtcNow;

            return (true, "Channel renamed", newName);
        }
    }
}
