namespace BoletoNetCore;

/// <summary>
/// Opções de configuração para o ciclo de vida e comportamento do renderer Playwright.
/// </summary>
/// <remarks>
/// Estas opções controlam como o renderer gerencia o processo do navegador, concorrência,
/// timeouts e comportamento de recuperação. São definidas uma vez durante a inicialização do renderer.
/// </remarks>
public class PlaywrightRendererOptions
{
    /// <summary>
    /// Obtém ou define o número máximo de operações de renderização concorrentes.
    /// </summary>
    /// <value>
    /// Máximo de renderizações concorrentes. Padrão: 2.
    /// </value>
    /// <remarks>
    /// Cada renderização concorrente consome aproximadamente 50MB de RAM.
    /// Aumente este valor para melhorar o throughput em sistemas multi-core, mas monitore o uso de memória.
    /// Faixa recomendada: 2-4 para a maioria dos deployments.
    /// </remarks>
    public int MaxConcurrency { get; set; } = 2;

    /// <summary>
    /// Obtém ou define o timeout para uma única operação de renderização.
    /// </summary>
    /// <value>
    /// Duração do timeout. Padrão: 30 segundos.
    /// </value>
    /// <remarks>
    /// Este timeout é aplicado em múltiplas camadas (contexto, página e operação)
    /// para evitar que renderizações travadas bloqueiem o sistema indefinidamente.
    /// </remarks>
    public TimeSpan RenderTimeout { get; set; } = TimeSpan.FromSeconds(30);

    /// <summary>
    /// Obtém ou define se o navegador deve ser pré-aquecido na inicialização do renderer.
    /// </summary>
    /// <value>
    /// <c>true</c> para iniciar o navegador imediatamente durante a inicialização;
    /// <c>false</c> para iniciar de forma lazy na primeira renderização. Padrão: <c>false</c>.
    /// </value>
    /// <remarks>
    /// O pré-aquecimento elimina o atraso de cold-start de 1-2 segundos na primeira renderização.
    /// Habilite em ambientes de produção onde latência previsível é importante.
    /// </remarks>
    public bool PrewarmOnStart { get; set; } = false;

    /// <summary>
    /// Obtém ou define se o navegador deve ser reiniciado automaticamente em caso de falha.
    /// </summary>
    /// <value>
    /// <c>true</c> para reiniciar automaticamente o processo do navegador quando ele crashar;
    /// <c>false</c> para falhar permanentemente. Padrão: <c>true</c>.
    /// </value>
    /// <remarks>
    /// Quando habilitado, o renderer tentará recuperar de crashes do navegador iniciando
    /// uma nova instância. Operações de renderização que falharam são tentadas novamente uma vez com o novo navegador.
    /// </remarks>
    public bool RestartOnFailure { get; set; } = true;

    /// <summary>
    /// Obtém ou define o caminho para um executável de navegador customizado.
    /// </summary>
    /// <value>
    /// Caminho completo para o executável do navegador, ou <c>null</c> para usar o navegador gerenciado pelo Playwright.
    /// Padrão: <c>null</c>.
    /// </value>
    /// <remarks>
    /// Use esta opção em ambientes restritos onde a instalação do navegador gerenciado pelo Playwright
    /// não está disponível ou uma versão específica do navegador é necessária.
    /// </remarks>
    public string? BrowserExecutablePath { get; set; }

    /// <summary>
    /// Obtém ou define argumentos de linha de comando adicionais para passar ao navegador na inicialização.
    /// </summary>
    /// <value>
    /// Array de argumentos de inicialização do navegador, ou <c>null</c> para usar os padrões. Padrão: <c>null</c>.
    /// </value>
    /// <remarks>
    /// Argumentos comuns incluem:
    /// <list type="bullet">
    /// <item><description><c>--disable-gpu</c>: Desabilita aceleração de hardware GPU</description></item>
    /// <item><description><c>--no-sandbox</c>: Executa sem sandbox (necessário ao rodar como root em containers)</description></item>
    /// <item><description><c>--disable-dev-shm-usage</c>: Usa /tmp em vez de /dev/shm para memória compartilhada</description></item>
    /// </list>
    /// Use com cautela; argumentos incorretos podem causar falhas na inicialização do navegador.
    /// </remarks>
    public string[]? BrowserLaunchArgs { get; set; }
}
