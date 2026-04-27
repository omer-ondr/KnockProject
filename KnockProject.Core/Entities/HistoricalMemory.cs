using Pgvector;

namespace KnockProject.Core.Entities;

public class HistoricalMemory
{
    public Guid Id { get; set; } = Guid.NewGuid();
    
    // 1973 dönemine ait metin (Örn: Asker mektubu, senaryo satırı)
    public required string TextContent { get; set; }
    
    // Metnin RAG için oluşturulmuş sayısal vektör karşılığı
    public Vector? Embedding { get; set; } 
}