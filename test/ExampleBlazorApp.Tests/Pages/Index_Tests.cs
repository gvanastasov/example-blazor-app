using Microsoft.Playwright;

namespace ExampleBlazorApp.Tests.Pages
{
    [Collection(PlaywrightFixture.PlaywrightCollection)]
    public class Index_Tests
    {
        private readonly PlaywrightFixture _playwrightFixture;

        public Index_Tests(PlaywrightFixture playwrightFixture)
        {
            _playwrightFixture = playwrightFixture;
        }

        [Fact]
        public async Task YourTest2()
        {
            // Act
            await _playwrightFixture.GotoPageAsync(
                "/",
                async (page) =>
                {
                    // Assert
                    var headingElement = await page.QuerySelectorAsync("h1");
                    Assert.NotNull(headingElement);

                    var headingText = await headingElement.TextContentAsync();
                    Assert.Equal("Example Blazor WASM Application", headingText);
                });
        }
    }
}