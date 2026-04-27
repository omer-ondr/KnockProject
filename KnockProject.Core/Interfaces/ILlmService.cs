namespace KnockProject.Core.Interfaces;

public interface ILlmService
{
    // 2. arkadaşın bu metodu doldurarak kullanıcı vedasını ve 1973 verisini birleştirip epigraf üretecek
    Task<string> GenerateEpigraphAsync(string userFarewell, string historicalContext);
}