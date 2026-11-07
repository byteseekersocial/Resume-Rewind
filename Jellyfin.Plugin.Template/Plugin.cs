using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using Jellyfin.Database.Implementations.Entities;
using MediaBrowser.Common.Configuration;
using MediaBrowser.Common.Plugins;
using MediaBrowser.Controller.Entities;
using MediaBrowser.Controller.Library;
using MediaBrowser.Controller.Session;
using MediaBrowser.Model.Entities;
using MediaBrowser.Model.Plugins;
using MediaBrowser.Model.Serialization;
using Microsoft.Extensions.Logging;
using ResumeRewind.Configuration;

namespace ResumeRewind;

/// <summary>
/// The main Resume Rewind plugin.
/// </summary>
public class Plugin : BasePlugin<PluginConfiguration>, IHasWebPages
{
    private readonly ISessionManager _sessionManager;
    private readonly IUserDataManager _userDataManager;
    private readonly ILogger<Plugin> _logger;
    private readonly UserPreferencesStore _preferencesStore;
    private readonly ConcurrentDictionary<string, long> _sessionStartPositions = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="Plugin"/> class.
    /// </summary>
    /// <param name="applicationPaths">The application paths.</param>
    /// <param name="xmlSerializer">The XML serializer.</param>
    /// <param name="sessionManager">The session manager.</param>
    /// <param name="userDataManager">The user data manager.</param>
    /// <param name="preferencesStore">The user preferences store.</param>
    /// <param name="logger">The plugin logger.</param>
    public Plugin(
        IApplicationPaths applicationPaths,
        IXmlSerializer xmlSerializer,
        ISessionManager sessionManager,
        IUserDataManager userDataManager,
        UserPreferencesStore preferencesStore,
        ILogger<Plugin> logger)
        : base(applicationPaths, xmlSerializer)
    {
        Instance = this;

        _sessionManager = sessionManager;
        _userDataManager = userDataManager;
        _logger = logger;
        _preferencesStore = preferencesStore;

        _sessionManager.PlaybackStart += OnPlaybackStart;
        _sessionManager.PlaybackStopped += OnPlaybackStopped;

        _logger.LogInformation("Resume Rewind initialized.");
    }

    /// <inheritdoc />
    public override string Name => "Resume Rewind";

    /// <inheritdoc />
    public override Guid Id =>
        Guid.Parse("95d6f48a-397e-4206-b97a-8474a864c7fe");

    /// <summary>
    /// Gets the current plugin instance.
    /// </summary>
    public static Plugin? Instance { get; private set; }

    /// <inheritdoc />
    public IEnumerable<PluginPageInfo> GetPages()
    {
        return
        [
            new PluginPageInfo
            {
                Name = Name,
                EmbeddedResourcePath = string.Format(
                    CultureInfo.InvariantCulture,
                    "{0}.Configuration.configPage.html",
                    GetType().Namespace)
            }
        ];
    }

    private void OnPlaybackStart(
        object? sender,
        PlaybackProgressEventArgs e)
    {
        if (e.Item is null || string.IsNullOrEmpty(e.PlaySessionId))
        {
            return;
        }

        long startPosition = e.PlaybackPositionTicks ?? 0;

        _sessionStartPositions[e.PlaySessionId] = startPosition;

        _logger.LogDebug(
            "Tracking playback session {SessionId}, starting at {Position}.",
            e.PlaySessionId,
            startPosition);
    }

    private void OnPlaybackStopped(
        object? sender,
        PlaybackStopEventArgs e)
    {
        if (e.Item is null ||
            string.IsNullOrEmpty(e.PlaySessionId) ||
            !e.PlaybackPositionTicks.HasValue)
        {
            return;
        }

        if (!_sessionStartPositions.TryRemove(
            e.PlaySessionId,
            out long startPosition))
        {
            return;
        }

        if (e.PlayedToCompletion)
        {
            return;
        }

        long finalPosition = e.PlaybackPositionTicks.Value;

        // Avoid applying rewind when playback barely progressed.
        if (finalPosition <= startPosition + TimeSpan.TicksPerSecond)
        {
            return;
        }

        bool isEpisode = e.Item.GetType().Name == "Episode";
        bool isMovie = e.Item.GetType().Name == "Movie";

        if (!isEpisode && !isMovie)
        {
            return;
        }

        if (e.Users is null)
{
    return;
}

        foreach (User user in e.Users)
{
    try
    {
        UserPreferences preferences = _preferencesStore.Get(user.Id);

        bool hasCustomSettings = isEpisode
            ? preferences.HasCustomEpisodeSettings
            : preferences.HasCustomMovieSettings;

        bool enabled = hasCustomSettings
            ? (isEpisode ? preferences.EpisodesEnabled : preferences.MoviesEnabled)
            : (isEpisode ? Configuration.EpisodesEnabled : Configuration.MoviesEnabled);

        int seconds = hasCustomSettings
            ? (isEpisode ? preferences.EpisodeRewindSeconds : preferences.MovieRewindSeconds)
            : (isEpisode ? Configuration.EpisodeRewindSeconds : Configuration.MovieRewindSeconds);

        seconds = Math.Clamp(seconds, 0, 60);

        if (!enabled || seconds <= 0)
        {
            continue;
        }

        long rewindTicks = TimeSpan.FromSeconds(seconds).Ticks;
        long adjustedPosition = Math.Max(0, finalPosition - rewindTicks);

        UserItemData? data = _userDataManager.GetUserData(user, e.Item);

        if (data is null)
                {
                    _logger.LogWarning(
                        "Could not find user data for {Item} ({User}).",
                        e.Item.Name,
                        user.Username);

                    continue;
                }

        data.PlaybackPositionTicks = adjustedPosition;

        _userDataManager.SaveUserData(
                    user,
                    e.Item,
                    data,
                    UserDataSaveReason.PlaybackFinished,
                    CancellationToken.None);

        _logger.LogInformation(
                    "Resume Rewind: rewound {Seconds} seconds for {Item} ({User}).",
                    seconds,
                    e.Item.Name,
                    user.Username);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Failed to rewind resume position for {Item}.",
                    e.Item.Name);
            }
        }
    }
}
