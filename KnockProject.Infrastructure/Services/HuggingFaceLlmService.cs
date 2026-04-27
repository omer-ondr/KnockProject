using KnockProject.Core.Interfaces;
using Microsoft.Extensions.Configuration;
using System.Text;
using System.Text.Json;

namespace KnockProject.Infrastructure.Services;

/// <summary>
/// LLM servisi — Pollinations.ai text API (OpenAI uyumlu, ücretsiz, key yok)
/// POST https://text.pollinations.ai/openai
/// </summary>
public class PollinationsLlmService : ILlmService
{
    private readonly HttpClient _http;
    private const string ApiUrl = "https://text.pollinations.ai/openai";

    public PollinationsLlmService(IHttpClientFactory factory, IConfiguration _)
    {
        _http = factory.CreateClient("Pollinations");
    }

    public async Task<string> GenerateEpigraphAsync(string userFarewell, string historicalContext)
    {
        var systemPrompt =
            "Sen Bob Dylan tarzı yazan melankolik bir Türk şairsin. " +
            "Sana verilen veda metni ve 1973 anısını harmanlayarak " +
            "tek satırlık, derin ve melankolik bir Türkçe epigraf yazarsın. " +
            "SADECE epigrafı yaz, başka hiçbir şey ekleme.";

        var userPrompt =
            $"Kullanıcının vedası: \"{userFarewell}\"\n" +
            $"1973 anısı: \"{historicalContext}\"\n\n" +
            "Bu ikisini harmanlayan tek satırlık melankolik Türkçe epigrafı yaz:";

        var result = await CallAsync(systemPrompt, userPrompt, maxTokens: 100, temperature: 0.85);

        if (result is null)
            return $"Kapıyı çalarken içimde 1973 yankılanıyor — \"{userFarewell[..Math.Min(35, userFarewell.Length)]}...\"";

        return result.Trim().Split('\n')[0].Trim();
    }

    public async Task<string> GenerateVisualMetaphorAsync(string epigraph)
    {
        var systemPrompt =
            "You are a surrealist art director for 1973-era Western films. " +
            "Respond with ONLY a short 3-4 word concrete physical object in English. " +
            "No explanations, no punctuation, just the object name.";

        var userPrompt =
            $"Give a 3-4 word concrete visual object (physical thing) symbolizing: \"{epigraph}\"\n" +
            "Examples: broken guitar string, rusted door handle, old military dog tag\n" +
            "Object:";

        var result = await CallAsync(systemPrompt, userPrompt, maxTokens: 20, temperature: 0.75);

        if (result is null)
            return "rusted sheriff badge";

        var metaphor = result.Trim().Split('\n')[0].Trim().TrimEnd('.', ',', ';', '"');
        return string.Join(' ', metaphor.Split(' ', StringSplitOptions.RemoveEmptyEntries).Take(5));
    }

    private async Task<string?> CallAsync(string system, string user, int maxTokens, double temperature)
    {
        var payload = JsonSerializer.Serialize(new
        {
            model = "openai",
            messages = new[]
            {
                new { role = "system", content = system },
                new { role = "user",   content = user   }
            },
            max_tokens = maxTokens,
            temperature,
            stream = false
        });

        try
        {
            var response = await _http.PostAsync(
                ApiUrl,
                new StringContent(payload, Encoding.UTF8, "application/json"));

            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine($"[LlmService] HTTP {(int)response.StatusCode}");
                return null;
            }

            var json = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(json);
            return doc.RootElement
                .GetProperty("choices")[0]
                .GetProperty("message")
                .GetProperty("content")
                .GetString();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[LlmService] Hata: {ex.Message}");
            return null;
        }
    }
}
