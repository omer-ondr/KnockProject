using KnockProject.Core.Interfaces;
using KnockProject.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
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
    private readonly ILogger<FarewellController> _logger;

    public FarewellController(
        AppDbContext db,
        IEmbeddingService embedding,
        ILlmService llm,
        IImageService image,
        ILogger<FarewellController> logger)
    {
        _db = db;
        _embedding = embedding;
        _llm = llm;
        _image = image;
        _logger = logger;
    }

    /// <summary>
    /// Kullanıcının veda metnini 1973 anılarıyla eşleştiren tam RAG boru hattı.
    /// POST /api/farewell  →  {"message": "..."}
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> ProcessFarewell([FromBody] FarewellRequest request)
    {
        if (string.IsNullOrWhiteSpace(request?.Message))
            return BadRequest(new { error = "message boş olamaz." });

        // ── 1. Embed ──────────────────────────────────────────────────────────
        _logger.LogInformation("[1/5] Embedding oluşturuluyor...");
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

        // ── 2. pgvector kosinüs benzerliği → en yakın 1973 anısı ──────────────
        _logger.LogInformation("[2/5] pgvector araması yapılıyor...");
        var pgVector = new Vector(queryVector);
        var closestMemory = await _db.HistoricalMemories
            .OrderBy(m => m.Embedding!.CosineDistance(pgVector))
            .FirstOrDefaultAsync();

        if (closestMemory is null)
            return StatusCode(500, new { error = "Veritabanında hafıza bulunamadı. Seeding yapıldı mı?" });

        _logger.LogInformation("[2/5] Eşleşen anı: {Id}", closestMemory.Id);

        // ── 3. RAG → melankolik epigraf ───────────────────────────────────────
        _logger.LogInformation("[3/5] Epigraf üretiliyor...");
        string epigraph;
        try
        {
            epigraph = await _llm.GenerateEpigraphAsync(request.Message, closestMemory.TextContent);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Epigraf hatası");
            epigraph = "Kapıyı çalarken içimde 1973 yankılanıyor...";
        }

        // ── 4. LLM → görsel metafor (dinamik image prompt için) ──────────────
        _logger.LogInformation("[4/5] Görsel metafor üretiliyor...");
        string visualMetaphor;
        try
        {
            visualMetaphor = await _llm.GenerateVisualMetaphorAsync(epigraph);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Metafor hatası");
            visualMetaphor = "rusted sheriff badge";
        }

        _logger.LogInformation("[4/5] Metafor: '{Metaphor}'", visualMetaphor);

        // ── 5. SDXL → sürreal rozet görseli ─────────────────────────────────
        _logger.LogInformation("[5/5] Rozet görseli üretiliyor (SDXL)...");
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
            pipeline = new[]
            {
                "1. Metin → 384-boyutlu vektör (lokal all-MiniLM-L6-v2)",
                "2. pgvector kosinüs benzerliği → en yakın 1973 anısı",
                "3. RAG + Zephyr-7b → melankolik Türkçe epigraf",
                "4. Zephyr-7b → İngilizce görsel metafor",
                "5. SDXL (stabilityai/sdxl-base-1.0) → dinamik 1973 rozet görseli"
            }
        });
    }
}

public record FarewellRequest(string Message);