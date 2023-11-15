using Bunit;

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

using ExampleBlazorDS.Components.CButton;

namespace ExampleBlazorDS.Tests.Components 
{
    public class CButton_Tests 
    {
        [Fact]
        public void CButton_RendersCorrectly()
        {
            // Arrange
            using var ctx = new TestContext();

            // Act
            var cut = ctx.RenderComponent<CButton>(parameters => parameters
                .Add(p => p.ButtonText, "Click me")
                .Add(p => p.OnClick, new EventCallback<MouseEventArgs>()));

            // Assert
            cut.MarkupMatches("<button class=\"CButton\">Click me</button>");
        }

        [Fact]
        public void CButton_InvokesCallbackOnClick()
        {
            // Arrange
            using var ctx = new TestContext();
            var clickCallbackInvoked = false;
            var cut = ctx.RenderComponent<CButton>(parameters => parameters
                .Add(p => p.ButtonText, "Click me")
                .Add(p => p.OnClick, () => { clickCallbackInvoked = true; }));

            // Act
            cut.Find("button").Click();

            // Assert
            Assert.True(clickCallbackInvoked);
        }
    }
}