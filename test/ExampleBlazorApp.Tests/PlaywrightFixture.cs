using System.Diagnostics;
using Microsoft.Playwright;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace ExampleBlazorApp.Tests 
{
    public class PlaywrightFixture : IAsyncLifetime
    {
        private readonly IMessageSink diagnosticMessageSink;

        private Process? _appProcess;

        public PlaywrightFixture(IMessageSink diagnosticMessageSink)
        {
            this.diagnosticMessageSink = diagnosticMessageSink;
        }

        /// <summary>
        /// Playwright module.
        /// </summary>
        public IPlaywright? Playwright { get; private set; }

        public async Task InitializeAsync()
        {
            InstallPlaywright();

            diagnosticMessageSink.OnMessage(new DiagnosticMessage("Init playwright fixture..."));

            if (_appProcess == null)
            {
                diagnosticMessageSink.OnMessage(new DiagnosticMessage("App not running"));

                _appProcess = AppStart();

                bool isReady = await ReadyCheck("http://localhost:5000");
                if (!isReady)
                {
                    throw new InvalidOperationException("App is not reachable within the expected time.");
                }

                Playwright = await Microsoft.Playwright.Playwright.CreateAsync();
            }
        }

        /// <summary>
        /// Close the Playwright browser and dispose resources
        /// </summary>
        /// <returns></returns>
        public async Task DisposeAsync()
        {
            diagnosticMessageSink.OnMessage(new DiagnosticMessage("Dispose playwright fixture..."));

            Playwright?.Dispose();

            if (_appProcess != null) {
                AppStop(_appProcess);
            }

            await Task.CompletedTask;
        }

        public async Task GotoPageAsync(
            string url,
            Func<IPage, Task> testHandler)
        {
            if (Playwright == null) 
            {
                await Task.CompletedTask;
                return;
            }

            Uri? baseUri = new Uri("http://localhost:5000");
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

        private Process AppStart()
        {
            var projectPath = "./src/ExampleBlazorApp/ExampleBlazorApp.csproj";
            
            ProcessStartInfo? processStartInfo = new ProcessStartInfo
            {
                FileName = "dotnet",
                Arguments = $"run --project {projectPath} --urls http://localhost:5000",
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true,
                WorkingDirectory = "C:/Users/cti1930/Repos/example-blazor-app/"
            };

            var process = new Process { StartInfo = processStartInfo };

            process.Start();

            diagnosticMessageSink.OnMessage(
                new DiagnosticMessage($"Starting app process in {processStartInfo.WorkingDirectory}..."));

            return process;
        }

        private void AppStop(Process process)
        {
            diagnosticMessageSink.OnMessage(new DiagnosticMessage("Stopping app process..."));

            process?.Kill();
        }

        /// <summary>
        /// A simple ping retry task to check if the app is running.
        /// </summary>
        /// <param name="appUrl"></param>
        /// <param name="maxRetries"></param>
        /// <returns></returns>
        private async Task<bool> ReadyCheck(string appUrl, int maxRetries = 5)
        {
            using var client = new HttpClient();
            var retryCount = 0;

            while (retryCount < maxRetries)
            {
                try
                {
                    var response = await client.GetAsync(appUrl);
                    if (response.IsSuccessStatusCode)
                    {
                        diagnosticMessageSink.OnMessage(new DiagnosticMessage("App started."));

                        return true;
                    }
                }
                catch (HttpRequestException)
                {
                    // Ignore errors and retry
                }

                await Task.Delay(1000);
                retryCount++;
            }

            return false;
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
