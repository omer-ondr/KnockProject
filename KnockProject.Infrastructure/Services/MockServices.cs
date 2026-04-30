using KnockProject.Core.Interfaces;

namespace KnockProject.Infrastructure.Services;

public class MockEmbeddingService : IEmbeddingService
{
    public Task<float[]> GenerateEmbeddingAsync(string text) =>
        Task.FromResult(new float[384]);
}

public class MockLlmService : ILlmService
{
    public Task<LlmFarewellResult> AnalyzeFarewellAsync(string userFarewell, string historicalContext) =>
        Task.FromResult(new LlmFarewellResult(
            $"[MOCK EPİGRAF]: '{userFarewell}' hüznü, 1973'ün tozlu yollarına karıştı.",
            "rusted sheriff badge",
            "Pink Floyd Time 1973"
        ));
}

public class MockImageService : IImageService
{
    public Task<string> GenerateBadgeImageAsync(string epigraph, string visualMetaphor) =>
        Task.FromResult($"https://via.placeholder.com/512?text=1973+{Uri.EscapeDataString(visualMetaphor)}");
}