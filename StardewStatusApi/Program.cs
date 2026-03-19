using Discord;
using Discord.WebSocket;
using StardewStatusApi.Helpers;
using StardewStatusApi.Models;
using StardewStatusApi.Services;

var builder = WebApplication.CreateBuilder(args);

DiscordSettings discordSettings = new();
builder.Configuration.GetSection("Discord").Bind(discordSettings);

ValidateDiscordSettings(discordSettings);

var client = new DiscordSocketClient(new DiscordSocketConfig
{
    LogLevel = LogSeverity.Info
});

client.Log += message =>
{
    Console.WriteLine(message.ToString());
    return Task.CompletedTask;
};

await client.LoginAsync(TokenType.Bot, discordSettings.BotToken.Trim());
await client.StartAsync();

var discordChannelService = new DiscordService(client, discordSettings);

var app = builder.Build();

app.MapGet("/health", () => Results.Ok("OK"));

app.MapPost("/status", async (HttpRequest request, SaveInfo saveInfo) =>
{
    if (!IsAuthorized(request, discordSettings.StatusApiKey))
        return Results.Unauthorized();

    string? validationError = ValidateSaveInfo(saveInfo);
    if (validationError is not null)
        return Results.BadRequest(validationError);

    string newName = NameFormatter.BuildDateChannelName(saveInfo);

    var result = await discordChannelService.UpdateDateChannelAsync(newName);

    if (!result.Success)
        return Results.Problem(result.Message);

    return Results.Ok(new
    {
        message = result.Message,
        channelName = result.ChannelName
    });
});

app.Lifetime.ApplicationStopping.Register(() =>
{
    client.StopAsync().GetAwaiter().GetResult();
    client.Dispose();
});

app.Run();

static void ValidateDiscordSettings(DiscordSettings settings)
{
    if (string.IsNullOrWhiteSpace(settings.BotToken))
        throw new InvalidOperationException("Discord bot token is missing.");

    if (string.IsNullOrWhiteSpace(settings.StatusApiKey))
        throw new InvalidOperationException("Discord status API key is missing.");

    if (settings.GuildId == 0)
        throw new InvalidOperationException("Discord GuildId is missing.");

    if (settings.DateChannelId == 0)
        throw new InvalidOperationException("Discord DateChannelId is missing.");
}

static bool IsAuthorized(HttpRequest request, string expectedApiKey)
{
    return request.Headers.TryGetValue("X-Api-Key", out var providedKey)
        && string.Equals(providedKey.ToString(), expectedApiKey, StringComparison.Ordinal);
}

static string? ValidateSaveInfo(SaveInfo saveInfo)
{
    if (saveInfo.Day < 1 || saveInfo.Day > 28)
        return "Invalid day.";

    if (saveInfo.Year < 1)
        return "Invalid year.";

    string seasonKey = saveInfo.Season?.ToLowerInvariant() ?? string.Empty;
    if (seasonKey is not ("spring" or "summer" or "fall" or "winter"))
        return "Invalid season.";

    string language = saveInfo.Language?.Trim().ToUpperInvariant() ?? "EN";
    if (language is not ("EN" or "HU"))
        return "Invalid language.";

    return null;
}