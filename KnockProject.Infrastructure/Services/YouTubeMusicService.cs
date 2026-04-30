using System.Text.Json;
using KnockProject.Core.Interfaces;
using Microsoft.Extensions.Configuration;

namespace KnockProject.Infrastructure.Services;

public class YouTubeMusicService : IYouTubeMusicService
{
    private readonly HttpClient _http;
    private readonly string _apiKey;

    public YouTubeMusicService(HttpClient http, IConfiguration config)
    {
        _http = http;
        _apiKey = config["YouTube:ApiKey"] ?? string.Empty;
    }

    public async Task<MusicTrackDto?> SearchMusicAsync(string query)
    {
        if (string.IsNullOrWhiteSpace(_apiKey)) return null;

        var url = $"https://www.googleapis.com/youtube/v3/search?part=snippet&q={Uri.EscapeDataString(query)}&type=video&videoCategoryId=10&maxResults=1&key={_apiKey}";
        
        try
        {
            var response = await _http.GetAsync(url);
            if (!response.IsSuccessStatusCode) return null;

            var json = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(json);
            var items = doc.RootElement.GetProperty("items");

            if (items.GetArrayLength() == 0) return null;

            var firstItem = items[0];
            var videoId = firstItem.GetProperty("id").GetProperty("videoId").GetString();
            var snippet = firstItem.GetProperty("snippet");
            var title = snippet.GetProperty("title").GetString();
            var thumbnailUrl = snippet.GetProperty("thumbnails").GetProperty("default").GetProperty("url").GetString();

            if (videoId == null) return null;

            return new MusicTrackDto(
                SongTitle: title ?? "Unknown Title",
                VideoUrl: $"https://www.youtube.com/watch?v={videoId}",
                ThumbnailUrl: thumbnailUrl ?? string.Empty
            );
        }
        catch
        {
            return null;
        }
    }
}
