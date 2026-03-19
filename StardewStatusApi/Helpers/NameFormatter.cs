using StardewStatusApi.Models;

namespace StardewStatusApi.Helpers
{
    internal static class NameFormatter
    {
        public static string BuildDateChannelName(SaveInfo saveInfo)
        {
            string seasonKey = saveInfo.Season.ToLowerInvariant();
            string language = string.IsNullOrWhiteSpace(saveInfo.Language)
                ? "EN"
                : saveInfo.Language.Trim().ToUpperInvariant();

            string emoji = seasonKey switch
            {
                "spring" => "🌸",
                "summer" => "☀️",
                "fall" => "🍂",
                "winter" => "❄️",
                _ => "📅"
            };

            string season = GetSeasonDisplayName(seasonKey, language);
            string yearLabel = language == "HU" ? "Év" : "Year";

            return $"{emoji} {season} {saveInfo.Day} • {yearLabel} {saveInfo.Year}";
        }

        private static string GetSeasonDisplayName(string seasonKey, string language)
        {
            return language switch
            {
                "HU" => seasonKey switch
                {
                    "spring" => "Tavasz",
                    "summer" => "Nyár",
                    "fall" => "Ősz",
                    "winter" => "Tél",
                    _ => "Ismeretlen"
                },
                _ => seasonKey switch
                {
                    "spring" => "Spring",
                    "summer" => "Summer",
                    "fall" => "Fall",
                    "winter" => "Winter",
                    _ => "Unknown"
                }
            };
        }
    }
}
