namespace KnockProject.Core.Interfaces;

public interface IImageService
{
    // 2. arkadaşın bu metodu doldurarak epigrafı prompt'a yedirecek ve görsel URL'si dönecek
    Task<string> GenerateBadgeImageAsync(string epigraph);
}