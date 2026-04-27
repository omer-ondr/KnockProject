namespace KnockProject.Core.Interfaces;

public interface IEmbeddingService
{
    // 2. arkadaşın bu metodu doldurarak metni alıp 1536 boyutlu float dizisi (vektör) dönecek
    Task<float[]> GenerateEmbeddingAsync(string text);
}