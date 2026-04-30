using KnockProject.Core.Interfaces;
using Microsoft.Extensions.Logging;

namespace KnockProject.Infrastructure.Services;

public class GoogleTtsService : ITextToSpeechService
{
    private readonly HttpClient _http;
    private readonly ILogger<GoogleTtsService> _logger;

    public GoogleTtsService(HttpClient http, ILogger<GoogleTtsService> logger)
    {
        _http = http;
        _logger = logger;
    }

    public async Task<string?> GenerateSpeechBase64Async(string text)
    {
        try
        {
            var encodedText = Uri.EscapeDataString(text);
            var url = $"https://translate.google.com/translate_tts?ie=UTF-8&q={encodedText}&tl=tr&client=tw-ob";

            var response = await _http.GetAsync(url);
            response.EnsureSuccessStatusCode();
            
            var audioBytes = await response.Content.ReadAsByteArrayAsync();
            return $"data:audio/mp3;base64,{Convert.ToBase64String(audioBytes)}";
        }
        catch (HttpRequestException ex)
        {
            _logger.LogWarning(ex, "Google TTS API returned an error (likely text too long: 400 Bad Request). Returning null audio.");
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unexpected error occurred while calling Google TTS API.");
            return null;
        }
    }
}
