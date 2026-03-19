using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewStatusMod.Models;
using StardewStatusMod.Services;
using StardewValley;

namespace StardewMod
{
    internal sealed class ModEntry : Mod
    {
        private ModConfig _config = null!;
        private StatusSenderService _statusSenderService = null!;

        public override void Entry(IModHelper helper)
        {
            _config = helper.ReadConfig<ModConfig>();

            HttpClient httpClient = new()
            {
                Timeout = TimeSpan.FromSeconds(10)
            };

            _statusSenderService = new StatusSenderService(httpClient, this.Monitor, _config);

            helper.Events.GameLoop.SaveLoaded += OnSaveLoaded;
            helper.Events.GameLoop.DayStarted += OnDayStarted;
        }

        private void OnSaveLoaded(object? sender, SaveLoadedEventArgs e)
        {
            if (Context.IsWorldReady)
                _ = SendCurrentStatusAsync();
        }

        private void OnDayStarted(object? sender, DayStartedEventArgs e)
        {
            if (Context.IsWorldReady)
                _ = SendCurrentStatusAsync();
        }

        private async Task SendCurrentStatusAsync()
        {
            SaveInfo saveInfo = new()
            {
                Day = Game1.dayOfMonth,
                Season = Game1.currentSeason,
                Year = Game1.year,
                Language = GetLanguageCode()
            };

            await _statusSenderService.SendAsync(saveInfo);
        }

        private string GetLanguageCode()
        {
            string language = _config.Language?.Trim().ToUpperInvariant() ?? "EN";
            return language is "HU" or "EN" ? language : "EN";
        }
    }
}