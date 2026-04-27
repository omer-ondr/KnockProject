using KnockProject.Core.Interfaces;

namespace KnockProject.Infrastructure.Services;

public class MockEmbeddingService : IEmbeddingService
{
    public Task<float[]> GenerateEmbeddingAsync(string text) =>
        Task.FromResult(new float[384]);
}

public class MockLlmService : ILlmService
{
    public Task<string> GenerateEpigraphAsync(string userFarewell, string historicalContext) =>
        Task.FromResult($"[MOCK EPİGRAF]: '{userFarewell}' hüznü, 1973'ün tozlu yollarına karıştı.");

    public Task<string> GenerateVisualMetaphorAsync(string epigraph) =>
        Task.FromResult("rusted sheriff badge");
}

public class MockImageService : IImageService
{
    public Task<string> GenerateBadgeImageAsync(string epigraph, string visualMetaphor) =>
        Task.FromResult($"https://via.placeholder.com/512?text=1973+{Uri.EscapeDataString(visualMetaphor)}");
}