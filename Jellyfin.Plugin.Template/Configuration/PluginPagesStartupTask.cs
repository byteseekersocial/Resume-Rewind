using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediaBrowser.Model.Tasks;
using ResumeRewind.Services;

namespace ResumeRewind.Configuration;

/// <summary>
/// Registers the Resume Rewind preferences page when Jellyfin starts.
/// </summary>
public class PluginPagesStartupTask : IScheduledTask
{
    private readonly PluginPagesIntegration _integration;

    /// <summary>
    /// Initializes a new instance of the <see cref="PluginPagesStartupTask"/> class.
    /// </summary>
    /// <param name="integration">The Plugin Pages integration.</param>
    public PluginPagesStartupTask(PluginPagesIntegration integration)
    {
        _integration = integration;
    }

    /// <inheritdoc />
    public string Name => "Register Resume Rewind preferences page";

    /// <inheritdoc />
    public string Key => "ResumeRewind-RegisterPluginPage";

    /// <inheritdoc />
    public string Description => "Registers the Resume Rewind user preferences page.";

    /// <inheritdoc />
    public string Category => "Resume Rewind";

    /// <inheritdoc />
    public Task ExecuteAsync(
        IProgress<double> progress,
        CancellationToken cancellationToken)
    {
        _integration.RegisterPage();
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public IEnumerable<TaskTriggerInfo> GetDefaultTriggers()
    {
        return
        [
            new TaskTriggerInfo
            {
                Type = TaskTriggerInfoType.StartupTrigger
            }
        ];
    }
}
