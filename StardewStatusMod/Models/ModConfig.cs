namespace StardewStatusMod.Models
{
    internal sealed class ModConfig
    {
        public string ApiUrl { get; set; } = "http://localhost:5175/status";
        public string ApiKey { get; set; } = "change-me";
        public string Language { get; set; } = "EN";
    }
}
