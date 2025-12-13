using System.Collections.Concurrent;
using System.Diagnostics;

namespace BoletoNetCore.Playwright.ConsoleApp.Metrics;

/// <summary>
/// Coleta métricas do processo (CPU, memória) em intervalos regulares durante execução de benchmark.
/// </summary>
public sealed class ProcessMetricsCollector : IDisposable
{
    private readonly Process _process;
    private readonly TimeSpan _sampleInterval;
    private readonly ConcurrentBag<MetricsSnapshot> _snapshots = new();
    private readonly CancellationTokenSource _cts = new();

    private Task? _collectionTask;
    private MetricsSnapshot? _baseline;
    private TimeSpan _lastCpuTime;
    private DateTime _lastSampleTime;

    public ProcessMetricsCollector(TimeSpan? sampleInterval = null)
    {
        _process = Process.GetCurrentProcess();
        _sampleInterval = sampleInterval ?? TimeSpan.FromMilliseconds(100);
    }

    /// <summary>
    /// Captura um snapshot base antes do início do benchmark.
    /// </summary>
    public MetricsSnapshot CaptureBaseline()
    {
        _process.Refresh();
        _baseline = TakeSnapshot();
        _lastCpuTime = _process.TotalProcessorTime;
        _lastSampleTime = DateTime.UtcNow;
        return _baseline;
    }

    /// <summary>
    /// Inicia coleta de métricas em background.
    /// </summary>
    public void StartCollection()
    {
        if (_baseline == null)
            throw new InvalidOperationException("Must call CaptureBaseline() before StartCollection()");

        _collectionTask = Task.Run(CollectionLoopAsync);
    }

    /// <summary>
    /// Para a coleta e retorna o resumo agregado.
    /// </summary>
    public async Task<MetricsSummary> StopCollectionAsync()
    {
        _cts.Cancel();

        if (_collectionTask != null)
        {
            try
            {
                await _collectionTask.ConfigureAwait(false);
            }
            catch (OperationCanceledException)
            {
                // Esperado
            }
        }

        _process.Refresh();
        var final = TakeSnapshot();

        return BuildSummary(final);
    }

    private async Task CollectionLoopAsync()
    {
        var token = _cts.Token;

        while (!token.IsCancellationRequested)
        {
            try
            {
                await Task.Delay(_sampleInterval, token).ConfigureAwait(false);

                _process.Refresh();
                var snapshot = TakeSnapshot();
                _snapshots.Add(snapshot);
            }
            catch (OperationCanceledException)
            {
                break;
            }
        }
    }

    private MetricsSnapshot TakeSnapshot()
    {
        return new MetricsSnapshot(
            Timestamp: DateTime.UtcNow,
            WorkingSetBytes: _process.WorkingSet64,
            PrivateMemoryBytes: _process.PrivateMemorySize64,
            GcTotalMemoryBytes: GC.GetTotalMemory(forceFullCollection: false),
            TotalProcessorTime: _process.TotalProcessorTime,
            Gen0Collections: GC.CollectionCount(0),
            Gen1Collections: GC.CollectionCount(1),
            Gen2Collections: GC.CollectionCount(2)
        );
    }

    private MetricsSummary BuildSummary(MetricsSnapshot final)
    {
        var allSnapshots = _snapshots.ToList();

        // Encontra pico pelo working set
        var peak = allSnapshots.Count > 0
            ? allSnapshots.MaxBy(s => s.WorkingSetBytes) ?? final
            : final;

        // Calcula percentuais de CPU
        var cpuPercentages = CalculateCpuPercentages(allSnapshots);
        var avgCpu = cpuPercentages.Count > 0 ? cpuPercentages.Average() : 0;
        var peakCpu = cpuPercentages.Count > 0 ? cpuPercentages.Max() : 0;

        return new MetricsSummary
        {
            Baseline = _baseline!,
            Peak = peak,
            Final = final,
            AverageCpuPercent = avgCpu,
            PeakCpuPercent = peakCpu
        };
    }

    private List<double> CalculateCpuPercentages(List<MetricsSnapshot> snapshots)
    {
        if (snapshots.Count < 2)
            return new List<double>();

        var percentages = new List<double>(snapshots.Count - 1);
        var processorCount = Environment.ProcessorCount;

        for (int i = 1; i < snapshots.Count; i++)
        {
            var prev = snapshots[i - 1];
            var curr = snapshots[i];

            var cpuDelta = curr.TotalProcessorTime - prev.TotalProcessorTime;
            var timeDelta = curr.Timestamp - prev.Timestamp;

            if (timeDelta.TotalMilliseconds > 0)
            {
                // CPU% = (tempo CPU usado / tempo real) / número de processadores * 100
                var cpuPercent = (cpuDelta.TotalMilliseconds / timeDelta.TotalMilliseconds) / processorCount * 100;
                percentages.Add(Math.Min(cpuPercent, 100)); // Limita em 100%
            }
        }

        return percentages;
    }

    public void Dispose()
    {
        _cts.Cancel();
        _cts.Dispose();
    }
}
