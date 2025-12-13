namespace BoletoNetCore.Playwright.ConsoleApp.Metrics;

/// <summary>
/// Representa um snapshot pontual de métricas do processo.
/// </summary>
public sealed record MetricsSnapshot(
    DateTime Timestamp,
    long WorkingSetBytes,
    long PrivateMemoryBytes,
    long GcTotalMemoryBytes,
    TimeSpan TotalProcessorTime,
    int Gen0Collections,
    int Gen1Collections,
    int Gen2Collections)
{
    public double WorkingSetMB => WorkingSetBytes / (1024.0 * 1024.0);
    public double PrivateMemoryMB => PrivateMemoryBytes / (1024.0 * 1024.0);
    public double GcTotalMemoryMB => GcTotalMemoryBytes / (1024.0 * 1024.0);
}

/// <summary>
/// Resumo agregado de métricas de uma execução de benchmark.
/// </summary>
public sealed class MetricsSummary
{
    public required MetricsSnapshot Baseline { get; init; }
    public required MetricsSnapshot Peak { get; init; }
    public required MetricsSnapshot Final { get; init; }

    public double BaselineWorkingSetMB => Baseline.WorkingSetMB;
    public double PeakWorkingSetMB => Peak.WorkingSetMB;
    public double FinalWorkingSetMB => Final.WorkingSetMB;
    public double DeltaWorkingSetMB => PeakWorkingSetMB - BaselineWorkingSetMB;

    public double PeakPrivateMemoryMB => Peak.PrivateMemoryMB;

    public double AverageCpuPercent { get; init; }
    public double PeakCpuPercent { get; init; }

    public int TotalGen0Collections => Final.Gen0Collections - Baseline.Gen0Collections;
    public int TotalGen1Collections => Final.Gen1Collections - Baseline.Gen1Collections;
    public int TotalGen2Collections => Final.Gen2Collections - Baseline.Gen2Collections;
}

/// <summary>
/// Resultado de uma única operação de renderização de PDF.
/// </summary>
public sealed record RenderResult(
    int Index,
    long ElapsedMs,
    bool Success,
    long SizeBytes,
    string? ErrorMessage = null);
