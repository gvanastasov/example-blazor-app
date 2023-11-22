using System.Diagnostics;

namespace ExampleBlazorApp.Tests
{
    public interface IAppManager
    {
        Task AppStart();
        Task AppStop();
    }
}