using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;

namespace Money2Word.Telemetry;

/// <summary>
/// Filters out low-value telemetry items to reduce noise in Application Insights:
/// <list type="bullet">
///   <item>Requests to /swagger paths (UI + JSON spec)</item>
///   <item>Successful HTTP 200 GET requests to "/" (home page static loads)</item>
/// </list>
/// Only API conversion calls and failures are retained.
/// </summary>
public sealed class RequestFilterProcessor(ITelemetryProcessor next) : ITelemetryProcessor
{
    public void Process(ITelemetry item)
    {
        if (item is RequestTelemetry request && ShouldFilter(request))
            return;

        next.Process(item);
    }

    private static bool ShouldFilter(RequestTelemetry request)
    {
        var url = request.Url?.AbsolutePath ?? string.Empty;

        // Drop all Swagger UI and spec requests
        if (url.StartsWith("/swagger", StringComparison.OrdinalIgnoreCase))
            return true;

        // Drop successful home-page GETs — keep failures and API traffic
        if (url == "/" && request.ResponseCode == "200")
            return true;

        return false;
    }
}
