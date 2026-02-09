using System.Diagnostics.Metrics;

namespace Infrastructure.Metrics;

public class MetricsRegistry
{
    public static int ActiveSessions = 0;

    public static readonly Meter Meter = new("Cortado-webapi");

    public static readonly ObservableGauge<int> ActiveSessionGauge =
        Meter.CreateObservableGauge("active_sessions_count", () => new Measurement<int>(ActiveSessions));
}