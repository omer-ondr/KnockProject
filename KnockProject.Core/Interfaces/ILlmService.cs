namespace KnockProject.Core.Interfaces;

public interface ILlmService
{
    /// <summary>Kullanıcı vedası + 1973 anısından melankolik Türkçe epigraf üretir.</summary>
    Task<string> GenerateEpigraphAsync(string userFarewell, string historicalContext);

    /// <summary>
    /// Epigrafı simgeleyen 3-4 kelimelik somut İngilizce görsel metafor üretir.
    /// Örn: "broken guitar string", "rusty door handle", "old military dog tag"
    /// </summary>
    Task<string> GenerateVisualMetaphorAsync(string epigraph);
}