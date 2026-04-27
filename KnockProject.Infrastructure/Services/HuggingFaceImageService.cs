using KnockProject.Core.Interfaces;
using Microsoft.Extensions.Configuration;
using System.Text;

namespace KnockProject.Infrastructure.Services;

/// <summary>
/// Görsel servisi — Pollinations.ai image API (ücretsiz, key yok, gerçek PNG)
/// GET https://image.pollinations.ai/prompt/{encoded}?seed=N&width=1024&height=1024&nologo=true
/// </summary>
public class PollinationsImageService : IImageService
{
    private readonly HttpClient _http;

    public PollinationsImageService(IHttpClientFactory factory, IConfiguration _)
    {
        _http = factory.CreateClient("Pollinations");
    }

    public async Task<string> GenerateBadgeImageAsync(string epigraph, string visualMetaphor)
    {
        var safeMetaphor = string.IsNullOrWhiteSpace(visualMetaphor)
            ? "rusted clock hands"
            : visualMetaphor.Trim();

        // Dinamik prompt: [METAPHOR] + sabit 1973 estetik suffix
        var prompt =
            $"1973 analog photography style, {safeMetaphor}, " +
            "rusty sheriff badge engraved with a surreal symbol, placed on a weathered wooden saloon table, " +
            "35mm film grain, warm Kodak-style color grading, muted earthy tones, " +
            "high contrast shadows, dusty atmosphere, cinematic depth of field, " +
            "weathered textures, Pat Garrett aesthetic, 8k resolution, masterpiece.";

        var seed = Random.Shared.Next(1, 1_000_000);
        var encodedPrompt = Uri.EscapeDataString(prompt);
        var url = $"https://image.pollinations.ai/prompt/{encodedPrompt}" +
                  $"?seed={seed}&width=1024&height=1024&model=flux&nologo=true";

        Console.WriteLine($"[ImageService] Metaphor: '{safeMetaphor}' | Seed: {seed}");

        const int maxRetries = 3;

        for (int attempt = 1; attempt <= maxRetries; attempt++)
        {
            try
            {
                var imageBytes = await _http.GetByteArrayAsync(url);

                if (imageBytes.Length < 1024)
                {
                    Console.WriteLine($"[ImageService] Küçük yanıt ({imageBytes.Length}b), fallback.");
                    return FallbackSvg(safeMetaphor);
                }

                var base64 = Convert.ToBase64String(imageBytes);
                Console.WriteLine($"[ImageService] ✓ Görsel üretildi ({imageBytes.Length / 1024} KB), deneme {attempt}");
                return $"data:image/jpeg;base64,{base64}";
            }
            catch (TaskCanceledException)
            {
                Console.WriteLine($"[ImageService] Timeout (Deneme {attempt}/{maxRetries})");
                if (attempt < maxRetries) { await Task.Delay(5000); continue; }
                return FallbackSvg(safeMetaphor);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ImageService] Hata: {ex.Message}");
                return FallbackSvg(safeMetaphor);
            }
        }

        return FallbackSvg(safeMetaphor);
    }

    private static string FallbackSvg(string metaphor)
    {
        var label = metaphor.Length > 22 ? metaphor[..22] : metaphor;
        var svg =
            $"<svg width=\"1024\" height=\"1024\" xmlns=\"http://www.w3.org/2000/svg\">" +
            $"<rect width=\"1024\" height=\"1024\" fill=\"#1a120b\"/>" +
            $"<polygon points=\"512,140 640,440 940,440 700,620 780,920 512,740 244,920 324,620 84,440 384,440\" " +
            $"fill=\"none\" stroke=\"#a87030\" stroke-width=\"8\"/>" +
            $"<text x=\"50%\" y=\"43%\" fill=\"#a87030\" font-size=\"36\" font-family=\"Georgia,serif\" " +
            $"text-anchor=\"middle\" letter-spacing=\"6\">SHERIFF</text>" +
            $"<text x=\"50%\" y=\"53%\" fill=\"#c8943a\" font-size=\"20\" font-family=\"Georgia,serif\" " +
            $"text-anchor=\"middle\">{label}</text>" +
            $"<text x=\"50%\" y=\"62%\" fill=\"#664422\" font-size=\"22\" font-family=\"Georgia,serif\" " +
            $"text-anchor=\"middle\">1973</text>" +
            $"</svg>";

        return "data:image/svg+xml;base64," + Convert.ToBase64String(Encoding.UTF8.GetBytes(svg));
    }
}
