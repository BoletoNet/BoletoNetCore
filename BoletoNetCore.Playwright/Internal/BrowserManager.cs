using Microsoft.Playwright;

namespace BoletoNetCore.Playwright.Internal
{
    /// <summary>
    /// Manages the browser lifecycle with thread-safe initialization and crash recovery.
    /// Implements double-checked locking pattern for browser initialization.
    /// </summary>
    internal sealed class BrowserManager : IAsyncDisposable, IDisposable
    {
        private readonly IPlaywright _playwright;
        private readonly BrowserTypeLaunchOptions _launchOptions;
        private readonly SemaphoreSlim _initializationLock = new(1, 1);

        private IBrowser? _browser;
        private int _browserGeneration;
        private bool _disposed;

        /// <summary>
        /// Gets the current browser generation number. Increments on each browser restart.
        /// </summary>
        public int CurrentGeneration => Volatile.Read(ref this._browserGeneration);

        /// <summary>
        /// Initializes a new instance of the <see cref="BrowserManager"/> class.
        /// </summary>
        /// <param name="playwright">The Playwright instance.</param>
        /// <param name="launchOptions">Browser launch options.</param>
        public BrowserManager(
            IPlaywright playwright,
            BrowserTypeLaunchOptions launchOptions)
        {
            this._playwright = playwright ?? throw new ArgumentNullException(nameof(playwright));
            this._launchOptions = launchOptions ?? throw new ArgumentNullException(nameof(launchOptions));
        }

        /// <summary>
        /// Gets or creates a browser instance using double-checked locking pattern.
        /// Handles browser crash detection and automatic restart.
        /// </summary>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>A connected browser instance.</returns>
        public async Task<IBrowser> GetOrCreateBrowserAsync(CancellationToken ct = default)
        {
            this.ThrowIfDisposed();

            // First check: avoid lock if browser is already connected
            if (this._browser?.IsConnected == true)
                return this._browser;

            // Acquire lock for initialization
            await this._initializationLock.WaitAsync(ct).ConfigureAwait(false);
            try
            {
                // Double-check after acquiring lock
                if (this._browser?.IsConnected == true)
                    return this._browser;

                // Dispose old browser safely if it exists but is not connected
                if (this._browser != null)
                {
                    try
                    {
                        await this._browser.CloseAsync().ConfigureAwait(false);
                    }
                    catch
                    {
                        // Ignore errors on close - browser may already be crashed
                    }
                    finally
                    {
                        this._browser = null;
                    }
                }

                // Launch new browser
                this._browser = await this._playwright.Chromium.LaunchAsync(this._launchOptions).ConfigureAwait(false);

                // Increment generation counter to track browser restarts
                Interlocked.Increment(ref this._browserGeneration);

                return this._browser;
            }
            finally
            {
                this._initializationLock.Release();
            }
        }

        /// <summary>
        /// Disposes the current browser instance if it exists.
        /// This method is thread-safe and can be called to force browser restart.
        /// </summary>
        public async Task DisposeBrowserAsync()
        {
            await this._initializationLock.WaitAsync().ConfigureAwait(false);
            try
            {
                if (this._browser != null)
                {
                    try
                    {
                        if (this._browser.IsConnected)
                        {
                            await this._browser.CloseAsync().ConfigureAwait(false);
                        }
                    }
                    catch
                    {
                        // Ignore errors during disposal
                    }
                    finally
                    {
                        this._browser = null;
                    }
                }
            }
            finally
            {
                this._initializationLock.Release();
            }
        }

        /// <summary>
        /// Disposes the browser manager and releases all resources.
        /// Takes the initialization lock to prevent races with GetOrCreateBrowserAsync.
        /// </summary>
        public async ValueTask DisposeAsync()
        {
            if (this._disposed)
                return;

            this._disposed = true;

            // Take the lock to prevent racing with GetOrCreateBrowserAsync
            await this._initializationLock.WaitAsync().ConfigureAwait(false);
            try
            {
                if (this._browser != null)
                {
                    try
                    {
                        if (this._browser.IsConnected)
                        {
                            await this._browser.CloseAsync().ConfigureAwait(false);
                        }
                    }
                    catch
                    {
                        // Ignore errors during disposal
                    }
                    finally
                    {
                        this._browser = null;
                    }
                }
            }
            finally
            {
                this._initializationLock.Release();
            }

            this._initializationLock.Dispose();
        }

        /// <summary>
        /// Synchronous disposal implementation.
        /// Takes the initialization lock to prevent races with GetOrCreateBrowserAsync.
        /// </summary>
        public void Dispose()
        {
            if (this._disposed)
                return;

            this._disposed = true;

            // Take the lock to prevent racing with GetOrCreateBrowserAsync
            this._initializationLock.Wait();
            try
            {
                if (this._browser != null)
                {
                    try
                    {
                        if (this._browser.IsConnected)
                        {
                            this._browser.CloseAsync().GetAwaiter().GetResult();
                        }
                    }
                    catch
                    {
                        // Ignore errors during disposal
                    }
                    finally
                    {
                        this._browser = null;
                    }
                }
            }
            finally
            {
                this._initializationLock.Release();
            }

            this._initializationLock.Dispose();
        }

        private void ThrowIfDisposed()
        {
            if (this._disposed)
                throw new ObjectDisposedException(nameof(BrowserManager));
        }
    }
}
