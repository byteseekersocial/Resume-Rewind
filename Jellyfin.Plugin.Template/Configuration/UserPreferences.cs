namespace ResumeRewind.Configuration;

/// <summary>
/// Stores playback rewind preferences for an individual Jellyfin user.
/// </summary>
public class UserPreferences
{
    /// <summary>
    /// Gets or sets a value indicating whether episode preferences are customized.
    /// </summary>
    public bool HasCustomEpisodeSettings { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether episode rewind is enabled.
    /// </summary>
    public bool EpisodesEnabled { get; set; } = true;

    /// <summary>
    /// Gets or sets the episode rewind duration in seconds.
    /// </summary>
    public int EpisodeRewindSeconds { get; set; } = 10;

    /// <summary>
    /// Gets or sets a value indicating whether movie preferences are customized.
    /// </summary>
    public bool HasCustomMovieSettings { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether movie rewind is enabled.
    /// </summary>
    public bool MoviesEnabled { get; set; } = true;

    /// <summary>
    /// Gets or sets the movie rewind duration in seconds.
    /// </summary>
    public int MovieRewindSeconds { get; set; } = 10;
}
