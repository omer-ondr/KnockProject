namespace KnockProject.Core.Interfaces;

public record LlmFarewellResult(string Epigraph, string Metaphor, string RecommendedSongQuery);

public interface ILlmService
{
    /// <summary>
    /// Kullanıcının veda mesajını ve 1973 anısını tek seferde analiz ederek 
    /// epigraf, İngilizce görsel metafor ve şarkı önerisini döner.
    /// </summary>
    Task<LlmFarewellResult> AnalyzeFarewellAsync(string userFarewell, string historicalContext);
}