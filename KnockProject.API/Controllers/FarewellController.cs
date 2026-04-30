using KnockProject.Core.Interfaces;
using KnockProject.Infrastructure.Data;
using KnockProject.API.Hubs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Pgvector;
using Pgvector.EntityFrameworkCore;

namespace KnockProject.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FarewellController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly IEmbeddingService _embedding;
    private readonly ILlmService _llm;
    private readonly IImageService _image;
    private readonly IYouTubeMusicService _youtube;
    private readonly IHubContext<ProgressHub> _hub;
    private readonly ILogger<FarewellController> _logger;

    public FarewellController(
        AppDbContext db,
        IEmbeddingService embedding,
        ILlmService llm,
        IImageService image,
        IYouTubeMusicService youtube,
        IHubContext<ProgressHub> hub,
        ILogger<FarewellController> logger)
    {
        _db = db;
        _embedding = embedding;
        _llm = llm;
        _image = image;
        _youtube = youtube;
        _hub = hub;
        _logger = logger;
    }

    [HttpPost]
    public async Task<IActionResult> ProcessFarewell([FromBody] FarewellRequest request)
    {
        if (string.IsNullOrWhiteSpace(request?.Message))
            return BadRequest(new { error = "message boş olamaz." });

        async Task NotifyClient(string message)
        {
            if (!string.IsNullOrEmpty(request.ConnectionId))
            {
                await _hub.Clients.Client(request.ConnectionId).SendAsync("ReceiveProgress", message);
                await Task.Delay(400); // Gecikmeyi kısalttım (400ms)
            }
        }

        // 1. Embed
        _logger.LogInformation("[1/5] Embedding oluşturuluyor...");
        await NotifyClient("Veda mesajın zihne kazınıyor...");
        float[] queryVector;
        try
        {
            queryVector = await _embedding.GenerateEmbeddingAsync(request.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Embedding hatası");
            return StatusCode(503, new { error = "Embedding servisi şu an kullanılamıyor.", detail = ex.Message });
        }

        // 2. pgvector
        _logger.LogInformation("[2/5] pgvector araması yapılıyor...");
        await NotifyClient("1973 anılarında kendine eş bir hatıra aranıyor...");
        var pgVector = new Vector(queryVector);
        var closestMemory = await _db.HistoricalMemories
            .OrderBy(m => m.Embedding!.CosineDistance(pgVector))
            .FirstOrDefaultAsync();

        if (closestMemory is null)
            return StatusCode(500, new { error = "Veritabanında hafıza bulunamadı. Seeding yapıldı mı?" });

        // 3. Paralel Epigraph & Metafor
        _logger.LogInformation("[3/5] Epigraf ve Metafor paralel üretiliyor...");
        await NotifyClient("Hatıralardan şiir ve görsel metafor kaleme alınıyor...");
        
        string epigraph = "Kapıyı çalarken içimde 1973 yankılanıyor...";
        string visualMetaphor = "rusted sheriff badge";
        string recommendedSongQuery = "Pink Floyd Time 1973";

        try
        {
            var analysisResult = await _llm.AnalyzeFarewellAsync(request.Message, closestMemory.TextContent);
            epigraph = analysisResult.Epigraph;
            recommendedSongQuery = analysisResult.RecommendedSongQuery;
            visualMetaphor = analysisResult.Metaphor;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "LLM Analiz hatası");
        }

        _logger.LogInformation("[4/6] Metafor: '{Metaphor}'", visualMetaphor);

        // 5. YouTube Music
        _logger.LogInformation("[5/6] YouTube'da müzik aranıyor: {Query}", recommendedSongQuery);
        await NotifyClient("Sana özel 1973 yılından bir şarkı bulunuyor...");
        MusicTrackDto? musicTrack = null;
        try
        {
            musicTrack = await _youtube.SearchMusicAsync(recommendedSongQuery);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "YouTube Music hatası");
        }

        // 6. Image
        _logger.LogInformation("[6/6] Rozet görseli üretiliyor...");
        await NotifyClient("Rozetin resmediliyor (bu işlem birkaç saniye sürecek)...");
        string imageData;
        try
        {
            imageData = await _image.GenerateBadgeImageAsync(epigraph, visualMetaphor);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Görsel üretim hatası");
            imageData = "image_generation_unavailable";
        }
        
        await NotifyClient("Tamamlandı.");

        return Ok(new
        {
            epigraph,
            visualMetaphor,
            closestMemory = new
            {
                text = closestMemory.TextContent,
                id = closestMemory.Id
            },
            badgeImage = imageData,
            musicTrack = musicTrack
        });
    }
}

public record FarewellRequest(string Message, string? ConnectionId);
