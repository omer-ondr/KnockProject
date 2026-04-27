using KnockProject.Core.Interfaces;
using KnockProject.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pgvector.EntityFrameworkCore; // Kosinüs benzerliği için gerekli

namespace KnockProject.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FarewellController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IEmbeddingService _embeddingService;
    private readonly ILlmService _llmService;
    private readonly IImageService _imageService;

    // Tüm servisleri Dependency Injection ile içeri alıyoruz
    public FarewellController(
        AppDbContext context,
        IEmbeddingService embeddingService,
        ILlmService llmService,
        IImageService imageService)
    {
        _context = context;
        _embeddingService = embeddingService;
        _llmService = llmService;
        _imageService = imageService;
    }

    // 3. arkadaşının arayüzden çağıracağı endpoint
    [HttpPost("process")]
    public async Task<IActionResult> ProcessFarewell([FromBody] string userFarewellText)
    {
        if (string.IsNullOrWhiteSpace(userFarewellText))
            return BadRequest("Veda metni boş olamaz.");

        try
        {
            // 1. Kullanıcının metnini vektöre çevir (2. arkadaşın yazdığı servisi çağırır)
            var userVector = await _embeddingService.GenerateEmbeddingAsync(userFarewellText);
            var pgVector = new Pgvector.Vector(userVector);

            // 2. RAG Hafıza Katmanı: Veritabanında en benzer 1973 anısını bul (Kosinüs benzerliği)
            // L2Distance veya CosineDistance kullanılabilir.
            var closestMemory = await _context.HistoricalMemories
                .OrderBy(m => m.Embedding!.CosineDistance(pgVector))
                .FirstOrDefaultAsync();

            if (closestMemory == null)
                return StatusCode(500, "Tarihsel hafızada eşleşecek veri bulunamadı.");

            // 3. Anlam Katmanı: Veda ve 1973 verisini birleştirip epigraf üret
            var epigraph = await _llmService.GenerateEpigraphAsync(userFarewellText, closestMemory.TextContent);

            // 4. Görsel Katman: Epigraf üzerinden rozet görselini üret
            var imageUrl = await _imageService.GenerateBadgeImageAsync(epigraph);

            // 5. Deneyim Tasarımcısına (3. arkadaşına) veriyi dön
            return Ok(new
            {
                OriginalFarewell = userFarewellText,
                HistoricalMatch = closestMemory.TextContent,
                Epigraph = epigraph,
                BadgeImageUrl = imageUrl
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Boru hattında bir hata oluştu: {ex.Message}");
        }
    }
}