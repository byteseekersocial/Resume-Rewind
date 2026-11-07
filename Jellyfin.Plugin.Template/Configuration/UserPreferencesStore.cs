using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using MediaBrowser.Common.Configuration;
using Microsoft.Extensions.Logging;

namespace ResumeRewind.Configuration;

/// <summary>
/// Handles persistent storage of individual user preferences.
/// </summary>
public class UserPreferencesStore
{
    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        WriteIndented = true
    };

    private readonly object _lock = new();
    private readonly string _filePath;
    private readonly ILogger<UserPreferencesStore> _logger;

    private Dictionary<Guid, UserPreferences> _preferences = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="UserPreferencesStore"/> class.
    /// </summary>
    /// <param name="applicationPaths">The Jellyfin application paths.</param>
    /// <param name="logger">The plugin logger.</param>
    public UserPreferencesStore(
        IApplicationPaths applicationPaths,
        ILogger<UserPreferencesStore> logger)
    {
        _logger = logger;

        string directory = Path.Combine(
            applicationPaths.PluginConfigurationsPath,
            "ResumeRewind");

        Directory.CreateDirectory(directory);

        _filePath = Path.Combine(directory, "user-preferences.json");

        Load();
    }

    /// <summary>
    /// Gets a user's saved preferences.
    /// </summary>
    /// <param name="userId">The Jellyfin user identifier.</param>
    /// <returns>The user's saved preferences or default preferences.</returns>
    public UserPreferences Get(Guid userId)
    {
        lock (_lock)
        {
            if (_preferences.TryGetValue(userId, out UserPreferences? saved))
            {
                return Copy(saved);
            }

            return new UserPreferences();
        }
    }

    /// <summary>
    /// Saves a user's preferences.
    /// </summary>
    /// <param name="userId">The Jellyfin user identifier.</param>
    /// <param name="preferences">The preferences to save.</param>
    public void Save(Guid userId, UserPreferences preferences)
    {
        ArgumentNullException.ThrowIfNull(preferences);

        lock (_lock)
        {
            UserPreferences sanitized = Copy(preferences);

            sanitized.EpisodeRewindSeconds = Math.Clamp(
                sanitized.EpisodeRewindSeconds, 0, 60);

            sanitized.MovieRewindSeconds = Math.Clamp(
                sanitized.MovieRewindSeconds, 0, 60);

            _preferences[userId] = sanitized;

            string temporaryPath = _filePath + ".tmp";

            try
            {
                string json = JsonSerializer.Serialize(
                    _preferences,
                    SerializerOptions);

                File.WriteAllText(temporaryPath, json);
                File.Move(temporaryPath, _filePath, true);
            }
            catch
            {
                _preferences.Remove(userId);
                Load();
                throw;
            }
        }

        _logger.LogDebug(
            "Saved Resume Rewind preferences for user {UserId}.",
            userId);
    }

    private void Load()
    {
        if (!File.Exists(_filePath))
        {
            return;
        }

        try
        {
            string json = File.ReadAllText(_filePath);

            _preferences =
                JsonSerializer.Deserialize<Dictionary<Guid, UserPreferences>>(json)
                ?? new Dictionary<Guid, UserPreferences>();

            _logger.LogInformation(
                "Loaded Resume Rewind preferences for {UserCount} users.",
                _preferences.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Failed to load Resume Rewind user preferences.");

            _preferences = new Dictionary<Guid, UserPreferences>();
        }
    }

    private static UserPreferences Copy(UserPreferences source)
    {
        return new UserPreferences
        {
            HasCustomEpisodeSettings = source.HasCustomEpisodeSettings,
            EpisodesEnabled = source.EpisodesEnabled,
            EpisodeRewindSeconds = source.EpisodeRewindSeconds,
            HasCustomMovieSettings = source.HasCustomMovieSettings,
            MoviesEnabled = source.MoviesEnabled,
            MovieRewindSeconds = source.MovieRewindSeconds
        };
    }
}
