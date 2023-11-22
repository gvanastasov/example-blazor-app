using Microsoft.Playwright;

namespace ExampleBlazorApp.Tests.Fixtures
{
    /// <summary>
    /// Test fixture that starts up the application, as well as virtual browser used for
    /// synthetic testing of pages.
    /// </summary>
    public sealed class PlaywrightFixture : IAsyncLifetime
    {
        private readonly IAppManager appManager;

        public PlaywrightFixture(IAppManager appManager)
        {
            this.appManager = appManager;
        }

        /// <summary>
        /// Playwright module.
        /// </summary>
        public IPlaywright? Playwright { get; private set; }

        public async Task InitializeAsync()
        {
            InstallPlaywright();
            Playwright = await Microsoft.Playwright.Playwright.CreateAsync();

            await appManager.AppStart();
        }

        /// <summary>
        /// Close the Playwright browser and dispose resources
        /// </summary>
        /// <returns></returns>
        public async Task DisposeAsync()
        {
            Playwright?.Dispose();

            await appManager.AppStop();
        }

        /// <summary>
        /// Navigates the current virtual browser to a target location and performs 
        /// a callback test once the blazor wasm app finishes loading.
        /// </summary>
        /// <param name="url"></param>
        /// <param name="testHandler"></param>
        /// <returns></returns>
        public async Task GotoPageAsync(
            string url,
            Func<IPage, Task> testHandler)
        {
            if (Playwright == null) 
            {
                await Task.CompletedTask;
                return;
            }

            Uri? baseUri = new Uri(AppManager.TEST_URI);
            var browser = await Playwright.Chromium.LaunchAsync();
            await using var context = await browser
                .NewContextAsync(
                    new BrowserNewContextOptions
                    {
                        IgnoreHTTPSErrors = true
                    });
            var page = await context.NewPageAsync();
            try
            {
                var gotoResult = await page.GotoAsync(
                    new Uri(baseUri, url).ToString(),
                    new PageGotoOptions {
                        WaitUntil = WaitUntilState.NetworkIdle
                    }
                );

                if (gotoResult != null) {
                    await gotoResult.FinishedAsync();
                    await page.EvaluateAsync(
                        "eventName => new Promise(callback => window.addEventListener(eventName, callback, { once: true })), 'blazorPageLoaded'");
                    
                    await testHandler(page);
                }
            }
            finally
            {
                await page.CloseAsync();
            }
        }

        private static void InstallPlaywright()
        {
            var exitCode = Microsoft.Playwright.Program.Main(
                new[] { "install-deps" });
            if (exitCode != 0)
            {
                throw new Exception(
                $"Playwright exited with code {exitCode} on install-deps");
            }
            exitCode = Microsoft.Playwright.Program.Main(new[] { "install" });
            if (exitCode != 0)
            {
                throw new Exception(
                $"Playwright exited with code {exitCode} on install");
            }
        }

        public const string PlaywrightCollection = nameof(PlaywrightCollection);
        
        [CollectionDefinition(PlaywrightCollection)]
        public class PlaywrightCollectionDefinition : ICollectionFixture<PlaywrightFixture>
        {
            // This class is just xUnit plumbing code to apply
            // [CollectionDefinition] and the ICollectionFixture<>
            // interfaces. Witch in our case is parametrized
            // with the PlaywrightFixture.
        }
    }
}
