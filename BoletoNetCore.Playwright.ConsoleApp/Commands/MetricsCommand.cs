using System.Collections.Concurrent;
using System.CommandLine;
using System.Diagnostics;
using BoletoNetCore.Playwright.ConsoleApp.Configuration;
using BoletoNetCore.Playwright.ConsoleApp.Metrics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace BoletoNetCore.Playwright.ConsoleApp.Commands;

/// <summary>
/// Comando de benchmark com métricas que testa geração concorrente de PDF capturando uso de CPU e memória.
/// </summary>
public static class MetricsCommand
{
    public static Command Create()
    {
        var outputOption = new Option<DirectoryInfo>("--output", "-o")
        {
            Required = true,
            Description = "Output directory for generated PDFs (if --save-pdfs is set)"
        };

        var totalOption = new Option<int>("--total", "-t")
        {
            Description = "Total number of PDFs to generate",
            DefaultValueFactory = _ => 100
        };

        var concurrencyOption = new Option<int>("--concurrency", "-c")
        {
            Description = "Number of concurrent render operations",
            DefaultValueFactory = _ => 4
        };

        var savePdfsOption = new Option<bool>("--save-pdfs")
        {
            Description = "Save generated PDFs to disk (default: discard)",
            DefaultValueFactory = _ => false
        };

        var command = new Command("metrics", "Benchmark PDF generation with CPU and memory metrics")
        {
            outputOption,
            totalOption,
            concurrencyOption,
            savePdfsOption
        };

        command.SetAction((ctx, _) =>
        {
            var output = ctx.GetValue(outputOption)!;
            var total = ctx.GetValue(totalOption);
            var concurrency = ctx.GetValue(concurrencyOption);
            var savePdfs = ctx.GetValue(savePdfsOption);

            return ExecuteAsync(output, total, concurrency, savePdfs);
        });

        return command;
    }

    private static async Task<int> ExecuteAsync(DirectoryInfo output, int total, int concurrency, bool savePdfs)
    {
        try
        {
            if (savePdfs && !output.Exists)
            {
                output.Create();
            }

            PrintHeader(total, concurrency);

            // Inicializa coletor de métricas
            using var metricsCollector = new ProcessMetricsCollector(TimeSpan.FromMilliseconds(100));

            // Constrói host com DI
            var host = BuildHost(concurrency);

            Console.WriteLine("  Warming up browser...");
            var warmupSw = Stopwatch.StartNew();
            await host.StartAsync();
            warmupSw.Stop();
            Console.WriteLine($"  Browser ready in {warmupSw.ElapsedMilliseconds:N0}ms");
            Console.WriteLine();

            // Captura métricas base após aquecimento do navegador
            var baseline = metricsCollector.CaptureBaseline();
            Console.WriteLine($"  Baseline memory: {baseline.WorkingSetMB:F0} MB");
            Console.WriteLine();

            // Obtém serviços
            var renderer = host.Services.GetRequiredService<IHtmlRenderer>();
            var pdfConfig = host.Services.GetRequiredService<PlaywrightPdfConfiguration>();
            var pdfFormat = pdfConfig.Options.ToPdfFormat();

            // Configura banco
            var banco = Banco.Instancia(Bancos.Sicredi);
            var contaBancaria = CreateContaBancaria();
            banco.Beneficiario = Utils.GerarBeneficiario("85305", "", "", contaBancaria);
            banco.FormataBeneficiario();

            // Coleção de resultados
            var results = new ConcurrentBag<RenderResult>();
            var completed = 0;
            var progressLock = new object();

            Console.WriteLine("  Generating PDFs...");

            // Inicia coleta de métricas
            metricsCollector.StartCollection();

            var benchmarkSw = Stopwatch.StartNew();

            // Gera boletos concorrentemente
            var boletoIndices = Enumerable.Range(1, total).ToList();

            await Parallel.ForEachAsync(
                boletoIndices,
                new ParallelOptions { MaxDegreeOfParallelism = concurrency },
                async (index, ct) =>
                {
                    var boletoIndex = (index % 12) + 1;
                    var boleto = Utils.GerarBoleto(banco, boletoIndex, "N", 10 + index);
                    var boletoBancario = new BoletoBancario { Boleto = boleto };
                    var html = boletoBancario.MontaHtmlEmbedded();

                    var renderSw = Stopwatch.StartNew();
                    RenderResult result;

                    try
                    {
                        var bytes = await renderer.RenderAsync(html, pdfFormat, ct);
                        renderSw.Stop();

                        result = new RenderResult(index, renderSw.ElapsedMilliseconds, true, bytes.Length);

                        if (savePdfs)
                        {
                            var fileName = Path.Combine(output.FullName, $"boleto-{index:D4}.pdf");
                            await File.WriteAllBytesAsync(fileName, bytes, ct);
                        }
                    }
                    catch (Exception ex)
                    {
                        renderSw.Stop();
                        result = new RenderResult(index, renderSw.ElapsedMilliseconds, false, 0, ex.Message);
                    }

                    results.Add(result);

                    // Atualiza progresso
                    lock (progressLock)
                    {
                        completed++;
                        PrintProgress(completed, total);
                    }
                });

            benchmarkSw.Stop();

            // Para coleta de métricas
            var metricsSummary = await metricsCollector.StopCollectionAsync();

            Console.WriteLine();
            Console.WriteLine();

            // Imprime resultados
            PrintResourceUsage(metricsSummary);
            PrintPerformance(results.ToList(), benchmarkSw.ElapsedMilliseconds);

            // Limpeza
            await host.StopAsync();

            return 0;
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Benchmark failed: {ex.Message}");
            Console.ResetColor();
            Console.WriteLine(ex.ToString());
            return 1;
        }
    }

    private static IHost BuildHost(int concurrency)
    {
        var builder = Host.CreateApplicationBuilder();

        // Configura logging - suprimido durante benchmark
        builder.Logging.ClearProviders();

        // Carrega configuração
        builder.Configuration
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: false);

        // Vincula configuração
        var pdfConfig = builder.Configuration
            .GetSection("PlaywrightPdf")
            .Get<PlaywrightPdfConfiguration>() ?? new PlaywrightPdfConfiguration();

        builder.Services.AddSingleton(pdfConfig);

        // Registra renderizador Playwright com concorrência especificada
        builder.Services.AddPlaywrightRenderer(options =>
        {
            options.MaxConcurrency = concurrency;
            options.RenderTimeout = TimeSpan.FromSeconds(60);
            options.PrewarmOnStart = true;
            options.RestartOnFailure = true;
        });

        return builder.Build();
    }

    private static void PrintHeader(int total, int concurrency)
    {
        var separator = new string('═', 70);

        Console.WriteLine();
        Console.WriteLine(separator);
        Console.WriteLine("  PLAYWRIGHT PDF METRICS BENCHMARK");
        Console.WriteLine(separator);
        Console.WriteLine($"  Total PDFs:   {total}");
        Console.WriteLine($"  Concurrency:  {concurrency}");
#if DEBUG
        Console.WriteLine($"  Mode:         Debug");
#else
        Console.WriteLine($"  Mode:         Release");
#endif
        Console.WriteLine();
    }

    private static void PrintProgress(int completed, int total)
    {
        const int barWidth = 40;
        var progress = (double)completed / total;
        var filledWidth = (int)(progress * barWidth);

        Console.Write($"\r  Progress: [{new string('█', filledWidth)}{new string('░', barWidth - filledWidth)}] {completed}/{total}  ");
    }

    private static void PrintResourceUsage(MetricsSummary summary)
    {
        var separator = new string('═', 70);

        Console.WriteLine(separator);
        Console.WriteLine("  RESOURCE USAGE");
        Console.WriteLine(separator);

        Console.WriteLine("  Memory (Working Set):");
        Console.WriteLine($"    Baseline:     {summary.BaselineWorkingSetMB,6:F0} MB");
        Console.WriteLine($"    Peak:         {summary.PeakWorkingSetMB,6:F0} MB");
        Console.WriteLine($"    Final:        {summary.FinalWorkingSetMB,6:F0} MB");
        Console.WriteLine($"    Delta:        {(summary.DeltaWorkingSetMB >= 0 ? "+" : "")}{summary.DeltaWorkingSetMB:F0} MB");
        Console.WriteLine();

        Console.WriteLine("  Memory (Private Bytes):");
        Console.WriteLine($"    Peak:         {summary.PeakPrivateMemoryMB,6:F0} MB");
        Console.WriteLine();

        Console.WriteLine("  CPU:");
        Console.WriteLine($"    Average:      {summary.AverageCpuPercent,6:F1}%");
        Console.WriteLine($"    Peak:         {summary.PeakCpuPercent,6:F1}%");
        Console.WriteLine();

        Console.WriteLine("  GC Collections:");
        Console.WriteLine($"    Gen 0:        {summary.TotalGen0Collections,6}");
        Console.WriteLine($"    Gen 1:        {summary.TotalGen1Collections,6}");
        Console.WriteLine($"    Gen 2:        {summary.TotalGen2Collections,6}");
        Console.WriteLine();
    }

    private static void PrintPerformance(List<RenderResult> results, long totalElapsedMs)
    {
        var separator = new string('═', 70);

        Console.WriteLine(separator);
        Console.WriteLine("  PERFORMANCE");
        Console.WriteLine(separator);

        var successResults = results.Where(r => r.Success).ToList();
        var failedResults = results.Where(r => !r.Success).ToList();

        Console.WriteLine($"  Total Time:       {totalElapsedMs / 1000.0:F1}s");
        Console.WriteLine($"  Throughput:       {results.Count / (totalElapsedMs / 1000.0):F1} PDFs/sec");
        Console.WriteLine();

        if (successResults.Count > 0)
        {
            var times = successResults.Select(r => r.ElapsedMs).OrderBy(t => t).ToList();
            var totalBytes = successResults.Sum(r => r.SizeBytes);

            Console.WriteLine("  Render Latency:");
            Console.WriteLine($"    Average:        {times.Average(),6:F0}ms");
            Console.WriteLine($"    Min:            {times.Min(),6}ms");
            Console.WriteLine($"    Max:            {times.Max(),6}ms");
            Console.WriteLine($"    P50:            {Percentile(times, 50),6}ms");
            Console.WriteLine($"    P95:            {Percentile(times, 95),6}ms");
            Console.WriteLine($"    P99:            {Percentile(times, 99),6}ms");
            Console.WriteLine();

            var avgSizeKB = totalBytes / 1024.0 / successResults.Count;
            Console.WriteLine($"  Total Output:     {totalBytes / 1024.0 / 1024.0:F1} MB (avg {avgSizeKB:F0} KB/PDF)");
        }

        Console.WriteLine();
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"  Success:          {successResults.Count}");
        Console.ResetColor();

        if (failedResults.Count > 0)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"  Failed:           {failedResults.Count}");
            Console.ResetColor();

            Console.WriteLine();
            Console.WriteLine("  Failures:");
            foreach (var failure in failedResults.Take(5))
            {
                Console.WriteLine($"    #{failure.Index}: {failure.ErrorMessage}");
            }
            if (failedResults.Count > 5)
            {
                Console.WriteLine($"    ... and {failedResults.Count - 5} more");
            }
        }

        Console.WriteLine(separator);
    }

    private static long Percentile(List<long> sortedData, int percentile)
    {
        if (sortedData.Count == 0) return 0;
        if (sortedData.Count == 1) return sortedData[0];

        var index = (int)Math.Ceiling(percentile / 100.0 * sortedData.Count) - 1;
        return sortedData[Math.Max(0, Math.Min(index, sortedData.Count - 1))];
    }

    private static ContaBancaria CreateContaBancaria()
    {
        return new ContaBancaria
        {
            Agencia = "0156",
            Conta = "85305",
            DigitoConta = "4",
            CarteiraPadrao = "1",
            TipoCarteiraPadrao = TipoCarteira.CarteiraCobrancaSimples,
            VariacaoCarteiraPadrao = "A",
            TipoFormaCadastramento = TipoFormaCadastramento.ComRegistro,
            TipoImpressaoBoleto = TipoImpressaoBoleto.Empresa,
            OperacaoConta = "05"
        };
    }
}
