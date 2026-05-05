using System.Text;
using System.Text.Json;
using KnockProject.Core.Interfaces;
using Microsoft.Extensions.Configuration;

namespace KnockProject.Infrastructure.Services;

// HuggingFace Bulut API (Inference API) veya Lokal Python kullanır
public class HuggingFaceEmbeddingService : IEmbeddingService
{
    private readonly HttpClient _http;
    private readonly string? _apiKey;
    private const string CloudModelUrl = "https://api-inference.huggingface.co/pipeline/feature-extraction/sentence-transformers/all-MiniLM-L6-v2";
    private const string LocalEmbedUrl = "http://localhost:5500/embed";

    public HuggingFaceEmbeddingService(IHttpClientFactory factory, IConfiguration config)
    {
        _http = factory.CreateClient("Pollinations"); // Genel HttpClient'ı kullanıyoruz
        _apiKey = config["HuggingFaceApiKey"];
    }

    public async Task<float[]> GenerateEmbeddingAsync(string text)
    {
        var payload = JsonSerializer.Serialize(new { inputs = text });
        var request = new HttpRequestMessage(HttpMethod.Post, string.IsNullOrEmpty(_apiKey) ? LocalEmbedUrl : CloudModelUrl);
        request.Content = new StringContent(payload, Encoding.UTF8, "application/json");

        if (!string.IsNullOrEmpty(_apiKey))
        {
            request.Headers.Add("Authorization", $"Bearer {_apiKey}");
        }

        var response = await _http.SendAsync(request);
        response.EnsureSuccessStatusCode();
        
        var json = await response.Content.ReadAsStringAsync();

        using var doc = JsonDocument.Parse(json);
        var root = doc.RootElement;

        // HuggingFace Cloud bazen [[...]] döner, bazen [...]
        var vectorElement = root.ValueKind == JsonValueKind.Array
            && root.GetArrayLength() > 0
            && root[0].ValueKind == JsonValueKind.Array
            ? root[0]
            : root;

        return vectorElement.EnumerateArray().Select(e => e.GetSingle()).ToArray();
    }
}
