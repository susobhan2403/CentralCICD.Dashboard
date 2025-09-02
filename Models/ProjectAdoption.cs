namespace CentralCICD.Dashboard.Models;

public sealed record ProjectAdoption(
    string Name,
    string Domain,
    int TotalPipelines,
    int MigratedPipelines,
    int SecurityTemplatesApplied,
    int TechTemplatesApplied,
    double BuildSuccessRate,     // 0..1
    double DeploySuccessRate,    // 0..1
    double AvgBuildDurationMin,  // minutes
    double LeadTimeDays,         // change lead-time
    double ChangeFailureRate     // 0..1
)
{
    public double AdoptionPct => TotalPipelines == 0 ? 0 : (double)MigratedPipelines / TotalPipelines;
    public double SecurityAdoptionPct => TotalPipelines == 0 ? 0 : (double)SecurityTemplatesApplied / TotalPipelines;
    public double TechAdoptionPct => TotalPipelines == 0 ? 0 : (double)TechTemplatesApplied / TotalPipelines;
}
