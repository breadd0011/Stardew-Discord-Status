using StardewModdingAPI;
using StardewStatusMod.Models;
using System.Net.Http.Json;

namespace StardewStatusMod.Services
{
    internal sealed class StatusSenderService
    {
        private readonly HttpClient _httpClient;
        private readonly IMonitor _monitor;
        private readonly ModConfig _config;

        public StatusSenderService(HttpClient httpClient, IMonitor monitor, ModConfig config)
        {
            _httpClient = httpClient;
            _monitor = monitor;
            _config = config;
        }

        public async Task SendAsync(SaveInfo saveInfo)
        {
            if (string.IsNullOrWhiteSpace(_config.ApiUrl))
            {
                _monitor.Log("ApiUrl is missing in config.json.", LogLevel.Error);
                return;
            }

            try
            {
                using HttpRequestMessage request = new(HttpMethod.Post, _config.ApiUrl)
                {
                    Content = JsonContent.Create(saveInfo)
                };

                if (!string.IsNullOrWhiteSpace(_config.ApiKey))
                    request.Headers.Add("X-Api-Key", _config.ApiKey);

                HttpResponseMessage response = await _httpClient.SendAsync(request);

                if (!response.IsSuccessStatusCode)
                    _monitor.Log($"API returned {(int)response.StatusCode} {response.ReasonPhrase}", LogLevel.Warn);
            }
            catch (Exception ex)
            {
                _monitor.Log($"Failed to send status: {ex.Message}", LogLevel.Error);
            }
        }
    }
}
