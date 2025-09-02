namespace CentralCICD.Dashboard.Models;

public sealed class KpiRow
{
    public required string Kpi { get; init; }
    public required double Actual { get; init; }
    public required double Target { get; init; }
    public required string Unit { get; init; } // "%", "min", "days", etc.
    public required IReadOnlyList<double> TrendLine { get; init; }
}
