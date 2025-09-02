using CentralCICD.Dashboard.Models;

namespace CentralCICD.Dashboard.Data;

public sealed class DataService
{
    // Front-runner projects (static for the demo)
    private static readonly string[] ProjectNames =
    [
        "Payments Hub","Murex","T24","MSD CRM","CMS","AWS MicroServices - Risk",
        "AWS MicroServices - CB","AWS MicroServices - PB","Insight","FAB Portal","GCN","Payit",
        "AWS IBMB","Internet Banking (IB)","SME Prism","DMS","Payit Modernization",
        "Dubai First","Apply.BankFab.com"
    ];

    public IReadOnlyList<ProjectAdoption> GetProjects()
    {
        var rand = new Random(42);
        var list = new List<ProjectAdoption>();
        foreach (var name in ProjectNames)
        {
            var total = rand.Next(6, 22);
            var migrated = rand.Next(total / 3, total);
            var sec = Math.Min(total, migrated + rand.Next(-2, 2));
            var tech = Math.Min(total, migrated + rand.Next(-3, 1));

            list.Add(new ProjectAdoption(
                name,
                DomainFor(name),
                total,
                Math.Clamp(migrated, 0, total),
                Math.Clamp(sec, 0, total),
                Math.Clamp(tech, 0, total),
                Math.Round(0.75 + rand.NextDouble() * 0.22, 2),
                Math.Round(0.7 + rand.NextDouble() * 0.25, 2),
                Math.Round(6 + rand.NextDouble() * 9, 1),
                Math.Round(2 + rand.NextDouble() * 5, 1),
                Math.Round(0.05 + rand.NextDouble() * 0.15, 2)
            ));
        }
        return list.OrderByDescending(p => p.AdoptionPct).ToList();
    }

    private static string DomainFor(string p) =>
        p.Contains("AWS", StringComparison.OrdinalIgnoreCase) ? "AWS" :
        p.Contains("IB", StringComparison.OrdinalIgnoreCase) ? "Channels" :
        p.Contains("Payit", StringComparison.OrdinalIgnoreCase) ? "Wallet" :
        p.Contains("Portal", StringComparison.OrdinalIgnoreCase) ? "Portal" :
        "Core Banking";

    public int TotalProjects() => GetProjects().Count;
    public int ProjectsOnboarded() => GetProjects().Count(p => p.MigratedPipelines > 0);
    public double AvgAdoptionPct() => GetProjects().Average(p => p.AdoptionPct);
    public double AvgSecurityAdoption() => GetProjects().Average(p => p.SecurityAdoptionPct);
    public double AvgTechAdoption() => GetProjects().Average(p => p.TechAdoptionPct);
    public double AvgBuildSuccess() => GetProjects().Average(p => p.BuildSuccessRate);
    public double AvgDeploySuccess() => GetProjects().Average(p => p.DeploySuccessRate);
    public double AvgLeadTimeDays() => GetProjects().Average(p => p.LeadTimeDays);

    // Monthly migration counts for the last 12 months (for sparklines)
    public IReadOnlyList<int> MonthlyMigrations()
    {
        var rnd = new Random(17);
        return Enumerable.Range(0, 12).Select(_ => rnd.Next(8, 28)).ToList();
    }

    // Histogram: distribution of adoption percentage
    public IReadOnlyList<double> AdoptionDistributionBuckets()
    {
        var buckets = new double[10];
        foreach (var p in GetProjects())
        {
            var idx = (int)Math.Clamp(Math.Floor(p.AdoptionPct * 10), 0, 9);
            buckets[idx]++;
        }
        return buckets.ToList();
    }

    public IReadOnlyList<KpiRow> ScorecardRows()
    {
        var trend1 = RandomTrend(seed: 1, start: 58, spread: 6);
        var trend2 = RandomTrend(seed: 2, start: 61, spread: 8);
        var trend3 = RandomTrend(seed: 3, start: 55, spread: 7);
        var trend4 = RandomTrend(seed: 4, start: 70, spread: 5);
        var trend5 = RandomTrend(seed: 5, start: 74, spread: 4);
        var trend6 = RandomTrend(seed: 6, start: 80, spread: 4);

        return new List<KpiRow>
        {
            new() { Kpi="Projects Onboarded", Actual=ProjectsOnboarded(), Target=TotalProjects(), Unit="",
                TrendLine=trend1 },
            new() { Kpi="Pipelines Migrated %", Actual=Math.Round(AvgAdoptionPct()*100,1), Target=85, Unit="%",
                TrendLine=trend2 },
            new() { Kpi="Security Templates Adoption %", Actual=Math.Round(AvgSecurityAdoption()*100,1), Target=95, Unit="%",
                TrendLine=trend3 },
            new() { Kpi="Tech Templates Adoption %", Actual=Math.Round(AvgTechAdoption()*100,1), Target=90, Unit="%",
                TrendLine=trend4 },
            new() { Kpi="Build Success Rate", Actual=Math.Round(AvgBuildSuccess()*100,1), Target=98, Unit="%",
                TrendLine=trend5 },
            new() { Kpi="Deployment Success Rate", Actual=Math.Round(AvgDeploySuccess()*100,1), Target=95, Unit="%",
                TrendLine=trend6 },
        };
    }

    public (double adoptionPct, double targetPct) OrgAdoptionGauge()
        => (Math.Round(AvgAdoptionPct() * 100, 1), 85);

    public (double securityPct, double targetPct) SecurityGauge()
        => (Math.Round(AvgSecurityAdoption() * 100, 1), 95);

    public IReadOnlyList<(string label, double value)> DomainSplit()
    {
        return GetProjects()
            .GroupBy(p => p.Domain)
            .Select(g => (g.Key, Math.Round(g.Average(x => x.AdoptionPct) * 100, 1)))
            .ToList();
    }

    private static IReadOnlyList<double> RandomTrend(int seed, double start, double spread)
    {
        var rnd = new Random(seed);
        var list = new List<double>();
        double val = start;
        for (int i = 0; i < 18; i++)
        {
            val += rnd.NextDouble() * spread - spread / 2.0;
            list.Add(Math.Round(Math.Clamp(val, 30, 100), 2));
        }
        return list;
    }
}
