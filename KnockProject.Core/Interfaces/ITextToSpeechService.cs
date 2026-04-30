namespace KnockProject.Core.Interfaces;

public interface ITextToSpeechService
{
    /// <summary>
    /// Converts text to speech and returns the audio as a Base64 string.
    /// Returns null if the operation fails.
    /// </summary>
    Task<string?> GenerateSpeechBase64Async(string text);
}
