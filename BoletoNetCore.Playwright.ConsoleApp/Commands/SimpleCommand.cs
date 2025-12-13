using System.CommandLine;
using System.Diagnostics;
using BoletoNetCore.Playwright.ConsoleApp.Configuration;
using Microsoft.Extensions.Configuration;

namespace BoletoNetCore.Playwright.ConsoleApp.Commands;

/// <summary>
/// Comando simples que gera arquivos de saída usando o método de extensão diretamente.
/// Suporta formatos de saída PDF, PNG e JPEG.
/// </summary>
public static class SimpleCommand
{
    public static Command Create()
    {
        var outputOption = new Option<DirectoryInfo>("--output", "-o")
        {
            Required = true,
            Description = "Output directory for the generated files"
        };

        var countOption = new Option<int>("--count", "-c")
        {
            Description = "Number of boletos to generate",
            DefaultValueFactory = _ => 4
        };

        var formatOption = new Option<string>("--format", "-f")
        {
            Description = "Output format: pdf, png, or jpeg",
            DefaultValueFactory = _ => "pdf"
        };

        var qualityOption = new Option<int>("--quality", "-q")
        {
            Description = "JPEG quality (0-100). Only applies to jpeg format",
            DefaultValueFactory = _ => 80
        };

        var marginOption = new Option<int>("--margin", "-m")
        {
            Description = "Margin in pixels around the content. Only applies to image formats (png, jpeg)",
            DefaultValueFactory = _ => 20
        };

        var command = new Command("simple", "Generate boleto output using the simple extension method")
        {
            outputOption,
            countOption,
            formatOption,
            qualityOption,
            marginOption
        };

        command.SetAction((ctx, _) =>
        {
            var output = ctx.GetValue(outputOption)!;
            var count = ctx.GetValue(countOption);
            var format = ctx.GetValue(formatOption)!;
            var quality = ctx.GetValue(qualityOption);
            var margin = ctx.GetValue(marginOption);

            return ExecuteAsync(output, count, format, quality, margin);
        });

        return command;
    }

    private static async Task<int> ExecuteAsync(DirectoryInfo output, int count, string formatName, int quality, int margin)
    {
        try
        {
            if (!output.Exists)
            {
                Console.WriteLine($"Creating output directory: {output.FullName}");
                output.Create();
            }

            // Converte o formato de saída
            var outputFormat = ParseOutputFormat(formatName, quality, margin);

            // Usa Sicredi como banco padrão
            var banco = Banco.Instancia(Bancos.Sicredi);

            Console.WriteLine($"Banco: {banco.Nome}");
            Console.WriteLine($"Boletos: {count}");
            Console.WriteLine($"Format: {formatName.ToUpperInvariant()}");
            if (outputFormat is JpegFormat jpegFmt)
            {
                Console.WriteLine($"Quality: {jpegFmt.Quality}");
                Console.WriteLine($"Margin: {jpegFmt.Margin}px");
            }
            else if (outputFormat is PngFormat pngFmt)
            {
                Console.WriteLine($"Margin: {pngFmt.Margin}px");
            }
            Console.WriteLine($"Output: {output.FullName}");
            Console.WriteLine();

            // Configura banco e beneficiário
            var contaBancaria = CreateContaBancaria();
            banco.Beneficiario = Utils.GerarBeneficiario("85305", "", "", contaBancaria);
            banco.FormataBeneficiario();

            // Gera boletos
            var boletos = Utils.GerarBoletos(banco, count, "N", 10);

            Console.WriteLine($"Generating {formatName.ToUpperInvariant()} with Playwright...");
            var stopwatch = Stopwatch.StartNew();

            // Usa o método de extensão assíncrono unificado do Playwright
            var bytes = await boletos.RenderPlaywrightAsync(outputFormat);

            stopwatch.Stop();

            // Salva arquivo com extensão correta
            var fileName = Path.Combine(output.FullName, $"boletos-{count}{outputFormat.FileExtension}");
            await File.WriteAllBytesAsync(fileName, bytes);

            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"{formatName.ToUpperInvariant()} generated successfully!");
            Console.ResetColor();
            Console.WriteLine($"  File: {fileName}");
            Console.WriteLine($"  Size: {bytes.Length:N0} bytes");
            Console.WriteLine($"  Time: {stopwatch.ElapsedMilliseconds:N0}ms");

            return 0;
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Error: {ex.Message}");
            Console.ResetColor();
            Console.WriteLine();
            Console.WriteLine(ex.ToString());
            return 1;
        }
    }

    private static OutputFormat ParseOutputFormat(string formatName, int quality, int margin)
    {
        return formatName.ToLowerInvariant() switch
        {
            "pdf" => PdfFormat.Default,
            "png" => new PngFormat(Margin: margin),
            "jpeg" or "jpg" => new JpegFormat(Quality: quality, Margin: margin),
            _ => throw new ArgumentException($"Unknown format: {formatName}. Supported formats: pdf, png, jpeg")
        };
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
