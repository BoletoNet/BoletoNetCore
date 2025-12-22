# BoletoNetCore.Playwright.Extensions

Extensões para integração do BoletoNetCore.Playwright com Microsoft.Extensions.DependencyInjection.

## Instalação

```bash
dotnet add package BoletoNetCore.Playwright.Extensions
```

## Quick Start

```csharp
// Program.cs
services.AddPlaywrightRenderer();
```

```csharp
public class BoletoService
{
    private readonly IHtmlRenderer _renderer;

    public BoletoService(IHtmlRenderer renderer)
    {
        _renderer = renderer;
    }

    public async Task<byte[]> GerarPdfAsync(Boleto boleto, CancellationToken ct = default)
    {
        return await boleto.RenderPlaywrightAsync(_renderer, PdfFormat.Default, cancellationToken: ct);
    }

    public async Task<byte[]> GerarPngAsync(Boleto boleto, CancellationToken ct = default)
    {
        return await boleto.RenderPlaywrightAsync(_renderer, PngFormat.Default, cancellationToken: ct);
    }

    public async Task<byte[]> GerarLotePdfAsync(Boletos boletos, CancellationToken ct = default)
    {
        return await boletos.RenderPlaywrightAsync(_renderer, PdfFormat.Default, cancellationToken: ct);
    }
}
```

## Configuração

### Via Delegate

```csharp
services.AddPlaywrightRenderer(options =>
{
    options.MaxConcurrency = 4;
    options.RenderTimeout = TimeSpan.FromSeconds(30);
    options.PrewarmOnStart = true;
    options.RestartOnFailure = true;
});
```

### Via appsettings.json

```csharp
// Program.cs
builder.Services.AddPlaywrightRenderer();
builder.Services.Configure<PlaywrightRendererOptions>(
    builder.Configuration.GetSection("PlaywrightRenderer"));
```

```json
// appsettings.json
{
  "PlaywrightRenderer": {
    "MaxConcurrency": 4,
    "RenderTimeoutSeconds": 30,
    "PrewarmOnStart": true,
    "RestartOnFailure": true
  }
}
```

### Opções Disponíveis

| Opção | Tipo | Padrão | Descrição |
|-------|------|--------|-----------|
| `MaxConcurrency` | int | 2 | Número máximo de renderizações simultâneas. Cada slot consome ~50MB de RAM. |
| `RenderTimeout` | TimeSpan | 30s | Timeout para cada operação de renderização. |
| `PrewarmOnStart` | bool | false | Inicializa o navegador no startup da aplicação, eliminando cold-start. |
| `RestartOnFailure` | bool | true | Reinicia o navegador automaticamente após crash. |
| `BrowserExecutablePath` | string? | null | Caminho para executável customizado do Chromium. |
| `BrowserLaunchArgs` | string[]? | null | Argumentos adicionais para o navegador. |

## Comportamento

### Singleton Thread-Safe

O renderizador é registrado como **singleton** para reutilizar o processo do navegador em todas as requisições. Uma única instância do Chromium pode lidar com múltiplas requisições concorrentes até o limite de `MaxConcurrency`.

### Prewarm

Quando `PrewarmOnStart = true`, um `IHostedService` é registrado para inicializar o navegador durante o startup da aplicação. Isso elimina o atraso de ~500ms na primeira renderização.

> **Nota:** Este recurso requer um host ASP.NET Core ou Generic Host para funcionar.

```
info: BoletoNetCore.PlaywrightRendererHostedService
      Initializing Playwright renderer...
info: BoletoNetCore.PlaywrightRendererHostedService
      Playwright renderer initialized successfully in 487ms
```

### Crash Recovery

Com `RestartOnFailure = true`, o navegador é reiniciado automaticamente se o processo Chromium falhar. As requisições em andamento receberão erro, mas requisições subsequentes funcionarão normalmente.

## Serviços Registrados

| Serviço | Lifetime | Descrição |
|---------|----------|-----------|
| `PlaywrightHtmlRenderer` | Singleton | Implementação concreta do renderizador |
| `IHtmlRenderer` | Singleton | Interface para injeção de dependência |
| `PlaywrightRendererHostedService` | Hosted | Serviço de prewarm (ativo apenas se configurado) |

## Requisitos

- .NET Standard 2.0+
- Microsoft.Extensions.DependencyInjection 2.0+
- Microsoft.Extensions.Hosting.Abstractions 2.0+
- BoletoNetCore.Playwright

## Licença

MIT
