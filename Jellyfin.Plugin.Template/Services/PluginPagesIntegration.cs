using System;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;

namespace ResumeRewind.Services;

/// <summary>
/// Integrates Resume Rewind with the Jellyfin Plugin Pages plugin.
/// </summary>
public class PluginPagesIntegration
{
    private const string PageId = "ResumeRewind";

    private readonly ILogger<PluginPagesIntegration> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="PluginPagesIntegration"/> class.
    /// </summary>
    /// <param name="logger">The logger used for integration messages.</param>
    public PluginPagesIntegration(ILogger<PluginPagesIntegration> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Registers the Resume Rewind preferences page.
    /// </summary>
    public void RegisterPage()
    {
        Assembly? assembly = AssemblyLoadContext.All
            .SelectMany(context => context.Assemblies)
            .FirstOrDefault(candidate =>
                candidate.FullName?.Contains(
                    "Jellyfin.Plugin.PluginPages",
                    StringComparison.Ordinal) == true);

        Type? pluginInterface = assembly?.GetType(
            "Jellyfin.Plugin.PluginPages.PluginInterface");

        MethodInfo? registerMethod = pluginInterface?.GetMethod(
            "RegisterPage");

        if (registerMethod is null)
        {
            _logger.LogWarning(
                "Plugin Pages was not found. Resume Rewind preferences page was not registered.");
            return;
        }

        JObject page = new()
        {
            ["Id"] = PageId,
            ["Url"] = "/ResumeRewind/PreferencesPage",
            ["DisplayText"] = "Resume Rewind",
            ["Icon"] = "rewind",
            ["Version"] = 1,
            ["IsEnabledAssembly"] = typeof(PluginPagesIntegration).Assembly.FullName,
            ["IsEnabledClass"] = typeof(PluginPagesIntegration).FullName,
            ["IsEnabledMethod"] = nameof(IsEnabled)
        };

        registerMethod.Invoke(null, [page]);

        _logger.LogInformation(
            "Registered Resume Rewind preferences page.");
    }

    /// <summary>
    /// Removes the Resume Rewind preferences page.
    /// </summary>
    public void RemovePage()
    {
        Assembly? assembly = AssemblyLoadContext.All
            .SelectMany(context => context.Assemblies)
            .FirstOrDefault(candidate =>
                candidate.FullName?.Contains(
                    "Jellyfin.Plugin.PluginPages",
                    StringComparison.Ordinal) == true);

        Type? pluginInterface = assembly?.GetType(
            "Jellyfin.Plugin.PluginPages.PluginInterface");

        MethodInfo? removeMethod = pluginInterface?.GetMethod(
            "RemovePage");

        removeMethod?.Invoke(null, [PageId]);
    }

    /// <summary>
    /// Determines whether the preferences page is enabled.
    /// </summary>
    /// <returns><see langword="true"/> when the preferences page is enabled.</returns>
    public static bool IsEnabled()
    {
        return true;
    }
}
