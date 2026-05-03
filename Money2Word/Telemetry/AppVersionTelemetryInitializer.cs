using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.Extensibility;
using System.Reflection;

namespace Money2Word.Telemetry;

/// <summary>
/// Stamps every telemetry item with the application version and hosting environment name.
/// These properties appear on all requests, exceptions, dependencies, and custom events
/// in Application Insights without repeating the values in every individual log call.
/// </summary>
public sealed class AppVersionTelemetryInitializer(IHostEnvironment hostEnvironment) : ITelemetryInitializer
{
    private readonly string _version = Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? "unknown";
    private readonly string _environment = hostEnvironment.EnvironmentName;

    public void Initialize(ITelemetry telemetry)
    {
        telemetry.Context.GlobalProperties.TryAdd("ApplicationVersion", _version);
        telemetry.Context.GlobalProperties.TryAdd("Environment", _environment);
    }
}
