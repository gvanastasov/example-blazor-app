using System.Diagnostics;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace ExampleBlazorApp.Tests
{
    /// <summary>
    /// A simple application manager that starts and runs the main SUT container.
    /// </summary>
    internal sealed class AppManager : IAppManager
    {
        internal const string TEST_URI = "http://localhost:5000";

        private readonly IMessageSink diagnosticMessageSink;

        private Process? _appProcess;

        public AppManager(IMessageSink diagnosticMessageSink)
        {
            this.diagnosticMessageSink = diagnosticMessageSink;
        }

        /// <summary>
        /// Start the application process.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public async Task AppStart()
        {
            diagnosticMessageSink.OnMessage(new DiagnosticMessage("Init playwright fixture..."));

            if (_appProcess == null)
            {
                diagnosticMessageSink.OnMessage(new DiagnosticMessage("App not running"));

                _appProcess = Run();

                bool isReady = await ReadyCheck(TEST_URI);
                if (!isReady)
                {
                    throw new InvalidOperationException("App is not reachable within the expected time.");
                }
            }
        }

        /// <summary>
        /// Stops the application process.
        /// </summary>
        /// <returns></returns>
        public Task AppStop()
        {
            diagnosticMessageSink.OnMessage(new DiagnosticMessage("Stopping app process..."));

            if (_appProcess != null) 
            {
                if (!_appProcess.HasExited)
                {
                    _appProcess.CloseMainWindow();
                    if (!_appProcess.WaitForExit(5000))
                    {
                        _appProcess.Kill();
                    }
                }

                _appProcess.Dispose();
            }

            return Task.CompletedTask;
        }

        /// <summary>
        /// Runs a process from the application project.
        /// </summary>
        /// <returns></returns>
        private Process Run()
        {
            var projectPath = "./src/ExampleBlazorApp/ExampleBlazorApp.csproj";
            
            // TODO: Environment.CurrentDirectory is being set to moved to the test binaries, 
            // therefore we need to go back a bit can probably rewrite this better but for now this.
            string currentDirectory = Directory.GetCurrentDirectory();
            string solutionRoot = Path.GetFullPath(Path.Combine(currentDirectory, "..", "..", "..", "..", ".."));

            ProcessStartInfo? processStartInfo = new ProcessStartInfo
            {
                FileName = "dotnet",
                Arguments = $"run --project {projectPath} --urls {TEST_URI}",
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true,
                WorkingDirectory = solutionRoot
            };

            var process = new Process { StartInfo = processStartInfo };

            process.Start();

            diagnosticMessageSink.OnMessage(
                new DiagnosticMessage($"Starting app process in '{processStartInfo.WorkingDirectory}'..."));

            return process;
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
    }
}