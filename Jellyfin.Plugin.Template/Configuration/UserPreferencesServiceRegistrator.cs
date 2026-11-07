using MediaBrowser.Controller;
using MediaBrowser.Controller.Plugins;
using Microsoft.Extensions.DependencyInjection;

namespace ResumeRewind.Configuration;

/// <summary>
/// Registers Resume Rewind services with Jellyfin.
/// </summary>
public class UserPreferencesServiceRegistrator : IPluginServiceRegistrator
{
    /// <inheritdoc />
    public void RegisterServices(
        IServiceCollection serviceCollection,
        IServerApplicationHost applicationHost)
    {
        serviceCollection.AddSingleton<UserPreferencesStore>();
    }
}
