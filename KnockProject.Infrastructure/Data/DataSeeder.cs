using System.Text.Json;
using KnockProject.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Pgvector;

namespace KnockProject.Infrastructure.Data;

public static class DataSeeder
{
    public static async Task SeedAsync(AppDbContext context, string seedFilePath)
    {
        if (await context.HistoricalMemories.AnyAsync())
            return;

        if (!File.Exists(seedFilePath))
        {
            Console.WriteLine($"[DataSeeder] seed.json bulunamadı: {seedFilePath}");
            return;
        }

        var json = await File.ReadAllTextAsync(seedFilePath);

        if (string.IsNullOrWhiteSpace(json))
        {
            Console.WriteLine("[DataSeeder] seed.json boş, seeding atlandı.");
            return;
        }

        var records = JsonSerializer.Deserialize<List<SeedRecord>>(json, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        if (records is null || records.Count == 0)
            return;

        var memories = records.Select(r => new HistoricalMemory
        {
            TextContent = r.TextContent,
            Embedding = new Vector(r.Embedding.ToArray())
        }).ToList();

        await context.HistoricalMemories.AddRangeAsync(memories);
        await context.SaveChangesAsync();

        Console.WriteLine($"[DataSeeder] {memories.Count} kayıt veritabanına eklendi.");
    }

    private sealed class SeedRecord
    {
        public string TextContent { get; set; } = string.Empty;
        public string Source { get; set; } = string.Empty;
        public int Year { get; set; }
        public List<float> Embedding { get; set; } = [];
    }
}
