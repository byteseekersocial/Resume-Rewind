using MediaBrowser.Model.Plugins;

namespace ResumeRewind.Configuration;

/// <summary>
/// Administrator defaults for Resume Rewind.
/// </summary>
public class PluginConfiguration : BasePluginConfiguration
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PluginConfiguration"/> class.
    /// </summary>
    public PluginConfiguration()
    {
        EpisodesEnabled = true;
        EpisodeRewindSeconds = 10;

        MoviesEnabled = true;
        MovieRewindSeconds = 10;
    }

    /// <summary>
    /// Gets or sets a value indicating whether episode rewind is enabled by default.
    /// </summary>
    public bool EpisodesEnabled { get; set; }

    /// <summary>
    /// Gets or sets the default episode rewind duration.
    /// </summary>
    public int EpisodeRewindSeconds { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether movie rewind is enabled by default.
    /// </summary>
    public bool MoviesEnabled { get; set; }

    /// <summary>
    /// Gets or sets the default movie rewind duration.
    /// </summary>
    public int MovieRewindSeconds { get; set; }
}
