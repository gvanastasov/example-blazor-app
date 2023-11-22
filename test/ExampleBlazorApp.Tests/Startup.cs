using Microsoft.Extensions.DependencyInjection;

namespace ExampleBlazorApp.Tests
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddTransient<IAppManager, AppManager>();
        }
    }
}