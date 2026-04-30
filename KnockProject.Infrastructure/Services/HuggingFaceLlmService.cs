using KnockProject.Core.Interfaces;
using Microsoft.Extensions.Configuration;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;

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

    public async Task<LlmFarewellResult> AnalyzeFarewellAsync(string userFarewell, string historicalContext)
    {
        var systemPrompt =
            "You are an AI generating a melancholic 1973-themed farewell response. You MUST return ONLY a raw, valid JSON object. No markdown tags (like ```json), no explanations.\n" +
            "JSON Schema: { \"epigraph\": \"string\", \"metaphor\": \"string\", \"recommendedSongQuery\": \"string\" }\n\n" +
            "epigraph: Türkçe yazılmış, 1-2 cümlelik, şiirsel, melankolik ve 1973 konseptine (hippiler, analog yaşam, şerifler veya tükenmişlik) uygun bir veda sözü.\n\n" +
            "metaphor: A short 3-4 word concrete physical object in English symbolizing the epigraph (e.g. 'rusted sheriff badge', 'broken guitar string', 'old military dog tag').\n\n" +
            "recommendedSongQuery: Kullanıcının duygu durumuna uyan ve KESİNLİKLE 1973 veya öncesinde çıkmış gerçek bir şarkı. Formatı tam olarak şu olmalı: '[Sanatçı Adı] - [Şarkı Adı] 1973 official audio'.";

        var userPrompt =
            $"Kullanıcının vedası: \"{userFarewell}\"\n" +
            $"1973 anısı: \"{historicalContext}\"\n\n" +
            "Bu ikisini harmanlayan JSON yanıtını yaz:";

        var jsonResult = await CallAsync(systemPrompt, userPrompt, maxTokens: 300, temperature: 0.85, useJson: true);

        var fallbackResult = new LlmFarewellResult("Geçmişin sessizliğinde kayboldum...", "rusted sheriff badge", "Pink Floyd - Time 1973 official audio");

        if (string.IsNullOrWhiteSpace(jsonResult))
            return fallbackResult;

        try
        {
            var cleanJson = jsonResult.Replace("```json", "").Replace("```", "").Trim();

            var epigraphMatch = Regex.Match(cleanJson, @"""epigraph""\s*:\s*""(.*?)""");
            var metaphorMatch = Regex.Match(cleanJson, @"""metaphor""\s*:\s*""(.*?)""");
            var songMatch = Regex.Match(cleanJson, @"""recommendedSongQuery""\s*:\s*""(.*?)""");

            var epigraph = epigraphMatch.Success && !string.IsNullOrWhiteSpace(epigraphMatch.Groups[1].Value) 
                ? epigraphMatch.Groups[1].Value 
                : "Geçmişin sessizliğinde kayboldum...";

            var metaphor = metaphorMatch.Success && !string.IsNullOrWhiteSpace(metaphorMatch.Groups[1].Value) 
                ? metaphorMatch.Groups[1].Value 
                : "rusted sheriff badge";

            var songQuery = songMatch.Success && !string.IsNullOrWhiteSpace(songMatch.Groups[1].Value) 
                ? songMatch.Groups[1].Value 
                : "Pink Floyd - Time 1973 official audio";

            return new LlmFarewellResult(epigraph, metaphor, songQuery);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[Regex Parse Error]: {ex.Message} - Payload: {jsonResult}");
        }

        return fallbackResult;
    }

    private async Task<string?> CallAsync(string system, string user, int maxTokens, double temperature, bool useJson = false)
    {
        var payloadObj = new
        {
            model = "openai",
            messages = new[]
            {
                new { role = "system", content = system },
                new { role = "user",   content = user   }
            },
            max_tokens = maxTokens,
            temperature,
            response_format = useJson ? new { type = "json_object" } : null,
            stream = false
        };

        var payload = JsonSerializer.Serialize(payloadObj, new JsonSerializerOptions { DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull });

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
