# BoletoNetCore.Playwright.ConsoleApp

Aplicativo de console para testes e benchmarks da geração de boletos utilizando Playwright.

## Configuração do Playwright

Antes de executar, instale o browser Chromium:

```bash
dotnet build -o ./build
pwsh ./build/playwright.ps1 install chromium
```

Para builds em Release:

```bash
dotnet build -c Release -o ./build
pwsh ./build/playwright.ps1 install chromium
```

## Comandos

### simple

Gera boletos em diferentes formatos (PDF, PNG, JPEG).

```bash
dotnet run -c Release -- simple -o <diretorio> [opcoes]
```

| Parâmetro | Alias | Descrição | Padrão |
|-----------|-------|-----------|--------|
| `--output` | `-o` | Diretório de saída (obrigatório) | - |
| `--count` | `-c` | Quantidade de boletos | 4 |
| `--format` | `-f` | Formato de saída: pdf, png, jpeg | pdf |
| `--quality` | `-q` | Qualidade JPEG (0-100) | 80 |
| `--margin` | `-m` | Margem em pixels (apenas imagens) | 20 |

**Exemplos:**

```bash
# PDF com 4 boletos
dotnet run -c Release -- simple -o D:\temp

# PNG com margem de 30px
dotnet run -c Release -- simple -o D:\temp -f png -m 30

# JPEG com qualidade 90 e 10 boletos
dotnet run -c Release -- simple -o D:\temp -f jpeg -q 90 -c 10
```

### metrics

Executa benchmark de geração de PDFs com métricas de CPU e memória.

```bash
dotnet run -c Release -- metrics -o <diretorio> [opcoes]
```

| Parâmetro | Alias | Descrição | Padrão |
|-----------|-------|-----------|--------|
| `--output` | `-o` | Diretório de saída (obrigatório) | - |
| `--total` | `-t` | Total de PDFs a gerar | 100 |
| `--concurrency` | `-c` | Operações de renderização concorrentes | 4 |
| `--save-pdfs` | - | Salvar PDFs gerados em disco | false |

**Exemplos:**

```bash
# Benchmark padrão: 100 PDFs com 4 threads
dotnet run -c Release -- metrics -o D:\temp

# 200 PDFs com 8 threads concorrentes
dotnet run -c Release -- metrics -o D:\temp -t 200 -c 8

# Salvar os PDFs gerados
dotnet run -c Release -- metrics -o D:\temp --save-pdfs
```

**Saída:**

O comando exibe métricas detalhadas incluindo:
- Uso de memória (Working Set, Private Bytes)
- Utilização de CPU (média e pico)
- Coletas do GC (Gen 0, 1, 2)
- Latência de renderização (média, min, max, P50, P95, P99)
- Throughput (PDFs/seg)
