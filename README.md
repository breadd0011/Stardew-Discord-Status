# Stardew Discord Status

A small Stardew Valley + Discord project that updates a Discord channel name with the current in-game date.

## Local setup

### 1) What you need
- .NET 8 SDK
- Stardew Valley
- SMAPI
- A Discord bot with permission to manage channels

### 2) Download the mod
Download the latest `StardewStatusMod` here:

[Download StardewStatusMod.zip](https://github.com/breadd0011/Stardew-Discord-Status/releases/latest/download/StardewStatusMod.zip)

Then extract it into your Stardew Valley `Mods` folder.

Your mod folder should contain files like:
- `StardewStatusMod.dll`
- `manifest.json`
- `config.json`

Example:

```text
Stardew Valley/Mods/StardewStatusMod/
```

### 3) Clone the repo for the API
```bash
git clone https://github.com/breadd0011/Stardew-Discord-Status.git
cd Stardew-Discord-Status/StardewStatusApi
```

### 4) Configure the API
Open `appsettings.json` and add your Discord IDs:

```json
{
  "Discord": {
    "BotToken": "",
    "StatusApiKey": "",
    "GuildId": 123456789012345678,
    "DateChannelId": 123456789012345678
  }
}
```
### 5) Run the API locally
```bash
dotnet run
```

The API should start on:

```text
http://localhost:5175
```

You can test it here:

```text
http://localhost:5175/health
```

### 6) Configure the mod
Open `config.json` inside `Stardew Valley/Mods/StardewStatusMod/` and set:

```json
{
  "ApiUrl": "http://localhost:5175/status",
  "ApiKey": "YOUR_SECRET_API_KEY",
  "Language": "EN"
}
```

Important:
- `ApiKey` must match the `StatusApiKey` in the API config
- Use `EN` or `HU` for `Language`

### 7) Start the game
- Keep the API running
- Launch Stardew Valley through SMAPI
- Load your save

The mod will send the current in-game date to the local API, and the API will update your Discord channel name.

## Notes
- No Docker is needed for this setup.
- Only the API needs to be cloned and run locally.
- The API and game should be running on the same machine for the default localhost setup.
