using System.Text;
using System.Text.Json;
using KnockProject.Core.Interfaces;
using Microsoft.Extensions.Configuration;

namespace KnockProject.Infrastructure.Services;

// Lokal Python Flask sunucusunu kullanır: python3 embedding_server.py
// Sunucu: http://localhost:5500/embed
public class HuggingFaceEmbeddingService : IEmbeddingService
{
    private readonly HttpClient _http;
    private const string LocalEmbedUrl = "http://localhost:5500/embed";

    public HuggingFaceEmbeddingService(IHttpClientFactory factory, IConfiguration config)
    {
        _http = factory.CreateClient("HuggingFace");
    }

    public async Task<float[]> GenerateEmbeddingAsync(string text)
    {
        var payload = JsonSerializer.Serialize(new { inputs = text });
        var response = await _http.PostAsync(
            LocalEmbedUrl,
            new StringContent(payload, Encoding.UTF8, "application/json"));

        response.EnsureSuccessStatusCode();
        var json = await response.Content.ReadAsStringAsync();

        // Lokal sunucu düz float dizisi döner: [0.1, 0.2, ...]
        using var doc = JsonDocument.Parse(json);
        var root = doc.RootElement;

        // İç içe array kontrolü: [[...]] veya [...]
        var vectorElement = root.ValueKind == JsonValueKind.Array
            && root.GetArrayLength() > 0
            && root[0].ValueKind == JsonValueKind.Array
            ? root[0]
            : root;

        return vectorElement.EnumerateArray().Select(e => e.GetSingle()).ToArray();
    }
}
