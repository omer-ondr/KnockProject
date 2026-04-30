namespace KnockProject.Core.Interfaces;

public record MusicTrackDto(string SongTitle, string VideoUrl, string ThumbnailUrl);

public interface IYouTubeMusicService
{
    Task<MusicTrackDto?> SearchMusicAsync(string query);
}
