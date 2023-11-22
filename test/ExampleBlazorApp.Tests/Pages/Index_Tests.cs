using Microsoft.Playwright;
using ExampleBlazorApp.Tests.Fixtures;

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
        public async Task Page_HasHeading()
        {
            // Act
            await _playwrightFixture.GotoPageAsync(
                "/",
                async (page) =>
                {
                    // Assert
                    var headingElement = await page.QuerySelectorAsync("h1");
                    Assert.NotNull(headingElement);

                    if (headingElement != null)
                    {
                        var headingText = await headingElement.TextContentAsync();
                        Assert.Equal("Example Blazor WASM Application", headingText);
                    }
                });
        }

        [Fact]
        public async Task Page_ButtonsUpdateCounterValue() 
        {
            // Act
            await _playwrightFixture.GotoPageAsync(
                "/",
                async (page) =>
                {
                    // Assert
                    var counterElement = await page.QuerySelectorAsync("p.counter");
                    Assert.NotNull(counterElement);

                    var controls = await page.QuerySelectorAllAsync("button");
                    var incrementButton = controls.FirstOrDefault();
                    var decreaseButton = controls.LastOrDefault();

                    Assert.NotNull(incrementButton);
                    Assert.NotNull(decreaseButton);

                    if (counterElement != null)
                    {
                        var value = await counterElement.InnerTextAsync();
                        Assert.Equal("0", value);
                        
                        if (incrementButton != null)
                        {
                            await incrementButton.ClickAsync();
                            value = await counterElement.InnerTextAsync();

                            Assert.Equal("1", value);
                        }

                        if (decreaseButton != null)
                        {
                            await decreaseButton.ClickAsync();
                            value = await counterElement.InnerTextAsync();

                            Assert.Equal("0", value);
                        }
                    }
                });
        }
    }
}