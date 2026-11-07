using System;
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
    /// Gets the authenticated user's preferences.
    /// </summary>
    /// <returns>The current user's preferences.</returns>
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

        return Ok(_preferencesStore.Get(authorizationInfo.UserId));
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
