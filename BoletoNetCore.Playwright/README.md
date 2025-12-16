# BoletoNetCore.Playwright

Biblioteca para renderização de boletos bancários brasileiros em PDF, PNG e JPEG utilizando Microsoft Playwright.

## Como Funciona

A biblioteca utiliza o Playwright para controlar uma instância headless do Chromium. O processo de renderização segue estes passos:

1. O HTML do boleto é gerado via `BoletoBancario.MontaHtmlEmbedded()`
2. O conteúdo HTML é carregado em uma página do navegador
3. O Playwright captura a página no formato desejado (PDF via emulação de impressão, ou screenshot para imagens)
4. O navegador permanece ativo para reutilização em requisições subsequentes

Esta abordagem garante fidelidade visual com o HTML original.

## Instalação

```bash
# Instalar o pacote
dotnet add package BoletoNetCore.Playwright

# Compilar o projeto (gera o script de instalação)
dotnet build -o ./build

# Instalar o Chromium
pwsh ./build/playwright.ps1 install chromium
```

## Quick Start

### Uso Direto

```csharp
// PDF
byte[] pdf = boleto.RenderPlaywright(PdfFormat.Default);

// PNG
byte[] png = boleto.RenderPlaywright(PngFormat.Default);

// JPEG com qualidade customizada
byte[] jpeg = boleto.RenderPlaywright(new JpegFormat(Quality: 90));

// Múltiplos boletos em um único PDF
byte[] pdfLote = boletos.RenderPlaywright(PdfFormat.Default);
```

### Com Injeção de Dependência

Para aplicações ASP.NET Core ou que utilizam `Microsoft.Extensions.DependencyInjection`, instale o pacote de extensões:

```bash
dotnet add package BoletoNetCore.Playwright.Extensions
```

```csharp
// Program.cs
services.AddPlaywrightRenderer(options =>
{
    options.MaxConcurrency = 4;
    options.PrewarmOnStart = true;
});
```

Consulte o [README do BoletoNetCore.Playwright.Extensions](../BoletoNetCore.Playwright.Extensions/README.md) para documentação completa sobre injeção de dependência, configuração via appsettings.json, prewarm e crash recovery.

## Formatos de Saída

| Formato | Classe | Características |
|---------|--------|-----------------|
| PDF | `PdfFormat` | Documento paginado, ideal para impressão |
| PNG | `PngFormat` | Imagem com transparência, compressão sem perdas |
| JPEG | `JpegFormat` | Imagem comprimida, qualidade configurável |

### PdfFormat

| Opção | Tipo | Padrão | Descrição |
|-------|------|--------|-----------|
| `PaperFormat` | string | "A4" | Formato do tamanho do papel (A4, Letter, Legal, etc.) |
| `Landscape` | bool | false | Orientação paisagem |
| `PrintBackground` | bool | true | Incluir backgrounds CSS |
| `Scale` | float | 1.0 | Fator de escala (0.1 a 2.0) |
| `Margin` | PdfMargins | 10mm | Margens da página |
| `PreferCssPageSize` | bool | false | Usar dimensões definidas por @page CSS |

### Exemplos

```csharp
// PDF com margens customizadas
var pdf = new PdfFormat(
    PaperFormat: "A4",
    Margin: new PdfMargins { Top = "5mm", Bottom = "5mm", Left = "10mm", Right = "10mm" }
);

// PNG com viewport específico
var png = new PngFormat(ViewportWidth: 800, ViewportHeight: 600);

// JPEG com qualidade alta
var jpeg = new JpegFormat(Quality: 95);
```

## Performance

Apesar de utilizar um navegador headless completo, o renderizador Playwright apresenta excelente performance. Benchmark executado com 100 PDFs gerados concorrentemente:

**Ambiente de teste:**
- CPU: AMD Ryzen 5 5600X (6 cores / 12 threads)
- OS: Windows 10
- .NET: 9.0 (Release build)

**Comando:**
```bash
cd BoletoNetCore.Playwright.ConsoleApp
dotnet run -c Release -- metrics --output ./results --total 100 --concurrency 4
```

**Resultados:**

| Categoria           | Métrica  | Valor         |
| ------------------- | -------- | ------------- |
| **Memória**         | Baseline | 44 MB         |
|                     | Peak     | 95 MB         |
|                     | Delta    | +50 MB        |
| **CPU**             | Média    | 2.9%          |
|                     | Pico     | 3.4%          |
| **Latência**        | Média    | 305 ms        |
|                     | P95      | 341 ms        |
|                     | P99      | 370 ms        |
| **Throughput**      |          | 12.8 PDFs/seg |

O overhead de memória é modesto (~50 MB para 4 slots concorrentes) e a latência é consistente com baixa variância entre P50 e P99.

## Arquitetura

```
BoletoNetCore.Playwright
├── IHtmlRenderer              # Interface principal para renderização multi-formato
├── PlaywrightHtmlRenderer     # Implementação com gerenciamento de browser
├── OutputFormat               # Discriminated union (PdfFormat, PngFormat, JpegFormat)
├── PlaywrightExtensions       # Métodos de extensão para Boleto/Boletos
└── Internal/
    ├── BrowserManager         # Ciclo de vida do Chromium com crash recovery
    └── HtmlUtilities          # Combinação de HTML para múltiplos boletos
```

**Características:**
- Singleton thread-safe com controle de concorrência via SemaphoreSlim
- Lazy initialization do navegador (ou prewarm configurável)
- Recuperação automática de crashes do navegador
- Double-checked locking para performance

## Requisitos

- .NET Standard 2.0+
- Playwright 1.57+
- Chromium (instalado via Playwright)
