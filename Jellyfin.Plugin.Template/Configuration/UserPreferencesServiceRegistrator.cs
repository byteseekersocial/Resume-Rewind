using MediaBrowser.Controller;
using MediaBrowser.Controller.Plugins;
using MediaBrowser.Model.Tasks;
using Microsoft.Extensions.DependencyInjection;
using ResumeRewind.Services;

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
        serviceCollection.AddSingleton<PluginPagesIntegration>();
        serviceCollection.AddSingleton<IScheduledTask, PluginPagesStartupTask>();
    }
}
