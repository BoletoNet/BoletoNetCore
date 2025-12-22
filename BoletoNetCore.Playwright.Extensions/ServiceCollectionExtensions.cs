using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using System;

namespace BoletoNetCore
{
    /// <summary>
    /// Métodos de extensão para registro dos serviços de renderização Playwright
    /// em um container de injeção de dependência ASP.NET Core.
    /// </summary>
    public static class PlaywrightServiceCollectionExtensions
    {
        /// <summary>
        /// Registra o renderizador Playwright como serviço singleton no container de injeção de dependência.
        /// </summary>
        /// <param name="services">O <see cref="IServiceCollection"/> para adicionar serviços.</param>
        /// <param name="configure">
        /// Uma ação opcional para configurar <see cref="PlaywrightRendererOptions"/>.
        /// Se nulo, opções padrão são utilizadas.
        /// </param>
        /// <returns>O <see cref="IServiceCollection"/> para encadeamento de métodos.</returns>
        /// <remarks>
        /// <para>
        /// O renderizador é registrado como singleton para reutilizar o processo do navegador em todas as requisições,
        /// maximizando throughput e minimizando consumo de recursos. Uma única instância do navegador
        /// pode lidar com segurança com requisições concorrentes de renderização até o limite configurado de MaxConcurrency.
        /// </para>
        /// <para>
        /// Se <see cref="PlaywrightRendererOptions.PrewarmOnStart"/> estiver definido como true, um
        /// <see cref="Microsoft.Extensions.Hosting.IHostedService"/> é registrado para inicializar
        /// o navegador durante a inicialização da aplicação com tratamento assíncrono adequado e logging.
        /// Isso elimina o atraso de cold-start na primeira requisição de renderização.
        /// </para>
        /// <para>
        /// Exemplo de uso:
        /// <code>
        /// services.AddPlaywrightRenderer(options =>
        /// {
        ///     options.MaxConcurrency = 4;
        ///     options.RenderTimeout = TimeSpan.FromSeconds(30);
        ///     options.PrewarmOnStart = true;
        /// });
        /// </code>
        /// </para>
        /// </remarks>
        /// <exception cref="ArgumentNullException">Lançada quando <paramref name="services"/> é nulo.</exception>
        public static IServiceCollection AddPlaywrightRenderer(
            this IServiceCollection services,
            Action<PlaywrightRendererOptions>? configure = null)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));

            // Configure options if provided
            if (configure != null)
            {
                services.Configure(configure);
            }

            // Register the concrete renderer type as singleton
            services.TryAddSingleton(serviceProvider =>
            {
                var options = serviceProvider.GetService<IOptions<PlaywrightRendererOptions>>()?.Value
                    ?? new PlaywrightRendererOptions();

                return new PlaywrightHtmlRenderer(options);
            });

            // Register interface pointing to same singleton
            services.TryAddSingleton<IHtmlRenderer>(sp => sp.GetRequiredService<PlaywrightHtmlRenderer>());

            // Always register hosted service - it checks PrewarmOnStart at runtime via IOptions<T>
            // This ensures prewarm works regardless of how options are configured (delegate, IConfiguration, etc.)
            services.AddHostedService<PlaywrightRendererHostedService>();

            return services;
        }
    }
}
