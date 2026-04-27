namespace KnockProject.Core.Interfaces;

public interface IImageService
{
    /// <summary>
    /// LLM'den gelen görsel metaforu kullanarak dinamik prompt oluşturur
    /// ve SDXL ile 1973-estetik rozet görseli üretir.
    /// </summary>
    Task<string> GenerateBadgeImageAsync(string epigraph, string visualMetaphor);
}