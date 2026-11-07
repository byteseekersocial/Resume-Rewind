using System;
using System.IO;
using System.Threading.Tasks;
using MediaBrowser.Controller.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ResumeRewind.Configuration;

namespace ResumeRewind.Controllers;

/// <summary>
/// Provides authenticated endpoints for individual user preferences.
/// </summary>
[Authorize]
[ApiController]
[Route("ResumeRewind/Preferences")]
public class UserPreferencesController : ControllerBase
{
    private readonly IAuthorizationContext _authorizationContext;
    private readonly UserPreferencesStore _preferencesStore;

    /// <summary>
    /// Initializes a new instance of the <see cref="UserPreferencesController"/> class.
    /// </summary>
    /// <param name="authorizationContext">The Jellyfin authorization context.</param>
    /// <param name="preferencesStore">The user preferences storage service.</param>
    public UserPreferencesController(
        IAuthorizationContext authorizationContext,
        UserPreferencesStore preferencesStore)
    {
        _authorizationContext = authorizationContext;
        _preferencesStore = preferencesStore;
    }

    /// <summary>
    /// Serves the Resume Rewind user preferences page.
    /// </summary>
    /// <returns>The embedded preferences HTML page.</returns>
    [HttpGet("/ResumeRewind/PreferencesPage")]
    public IActionResult GetPreferencesPage()
    {
        using Stream? stream = typeof(UserPreferencesController).Assembly
            .GetManifestResourceStream(
                "ResumeRewind.Configuration.preferencesPage.html");

        if (stream is null)
        {
            return NotFound("Resume Rewind preferences page was not found.");
        }

        using StreamReader reader = new(stream);

        return Content(reader.ReadToEnd(), "text/html");
    }

    /// <summary>
    /// Gets the authenticated user's preferences.
    /// </summary>
    /// <returns>The current user's effective preferences.</returns>
    [HttpGet]
    public async Task<ActionResult<UserPreferences>> GetPreferences()
    {
        var authorizationInfo = await _authorizationContext
            .GetAuthorizationInfo(HttpContext)
            .ConfigureAwait(false);

        if (!authorizationInfo.IsAuthenticated ||
            authorizationInfo.UserId == Guid.Empty)
        {
            return Unauthorized();
        }

        UserPreferences preferences =
            _preferencesStore.Get(authorizationInfo.UserId);

        PluginConfiguration defaults =
            Plugin.Instance?.Configuration ?? new PluginConfiguration();

        if (!preferences.HasCustomEpisodeSettings)
        {
            preferences.EpisodesEnabled = defaults.EpisodesEnabled;
            preferences.EpisodeRewindSeconds = defaults.EpisodeRewindSeconds;
        }

        if (!preferences.HasCustomMovieSettings)
        {
            preferences.MoviesEnabled = defaults.MoviesEnabled;
            preferences.MovieRewindSeconds = defaults.MovieRewindSeconds;
        }

        return Ok(preferences);
    }

    /// <summary>
    /// Saves the authenticated user's preferences.
    /// </summary>
    /// <param name="preferences">The preferences to save.</param>
    /// <returns>The result of the save operation.</returns>
    [HttpPut]
    public async Task<IActionResult> SavePreferences(
        [FromBody] UserPreferences preferences)
    {
        var authorizationInfo = await _authorizationContext
            .GetAuthorizationInfo(HttpContext)
            .ConfigureAwait(false);

        if (!authorizationInfo.IsAuthenticated ||
            authorizationInfo.UserId == Guid.Empty)
        {
            return Unauthorized();
        }

        _preferencesStore.Save(authorizationInfo.UserId, preferences);

        return NoContent();
    }
}
