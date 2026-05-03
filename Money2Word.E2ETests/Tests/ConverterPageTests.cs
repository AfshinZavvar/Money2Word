using FluentAssertions;
using Microsoft.Playwright;
using Money2Word.E2ETests.Fixtures;

namespace Money2Word.E2ETests.Tests;

public class ConverterPageTests(AppHostFixture app, PlaywrightFixture playwright)
    : IClassFixture<AppHostFixture>, IClassFixture<PlaywrightFixture>
{
    private async Task<IPage> NewPageAsync()
    {
        var page = await playwright.Browser.NewPageAsync();
        await page.GotoAsync(app.BaseUrl);
        return page;
    }

    [Fact]
    public async Task Page_Loads_ShowsConverterCard()
    {
        var page = await NewPageAsync();

        await page.Locator(".glass-card").WaitForAsync();
        await page.Locator("#Amount").WaitForAsync();
        await page.Locator("#btnSubmit").WaitForAsync();

        var title = await page.Locator(".card-title").TextContentAsync();
        title.Should().Be("Money to Words");
    }

    [Fact]
    public async Task Input_AutoFormats_ThousandsSeparator()
    {
        var page = await NewPageAsync();

        await page.Locator("#Amount").FillAsync("1234.56");

        var value = await page.Locator("#Amount").InputValueAsync();
        value.Should().Be("1,234.56");
    }

    [Fact]
    public async Task Input_Clamps_To15WholeDigits()
    {
        var page = await NewPageAsync();

        await page.Locator("#Amount").FillAsync("9999999999999999"); // 16 digits

        var raw = (await page.Locator("#Amount").InputValueAsync()).Replace(",", "");
        raw.Length.Should().BeLessThanOrEqualTo(15);
    }

    [Fact]
    public async Task Input_Clamps_To2DecimalPlaces()
    {
        var page = await NewPageAsync();

        await page.Locator("#Amount").FillAsync("1.999");

        var value = await page.Locator("#Amount").InputValueAsync();
        value.Should().Be("1.99");
    }

    [Fact]
    public async Task Convert_ValidAmount_ShowsResultPanel()
    {
        var page = await NewPageAsync();

        await page.Locator("#Amount").FillAsync("1.00");
        await page.Locator("#btnSubmit").ClickAsync();

        await page.Locator("#resultPanel").WaitForAsync(new() { State = WaitForSelectorState.Visible });
        var text = await page.Locator("#responseAmount").TextContentAsync();
        text.Should().Contain("ONE DOLLAR");
    }

    [Fact]
    public async Task Convert_ValidAmount_HighlightsScaleWords()
    {
        var page = await NewPageAsync();

        await page.Locator("#Amount").FillAsync("1000.00");
        await page.Locator("#btnSubmit").ClickAsync();

        await page.Locator("#resultPanel").WaitForAsync(new() { State = WaitForSelectorState.Visible });
        var highlightCount = await page.Locator("#responseAmount .word-highlight").CountAsync();
        highlightCount.Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task Convert_EmptyInput_ShowsValidationError()
    {
        var page = await NewPageAsync();

        await page.Locator("#btnSubmit").ClickAsync();

        var error = await page.Locator("#responseError").TextContentAsync();
        error.Should().NotBeNullOrWhiteSpace();

        var ariaInvalid = await page.Locator("#Amount").GetAttributeAsync("aria-invalid");
        ariaInvalid.Should().Be("true");
    }

    [Fact]
    public async Task Convert_BelowMin_ShowsValidationError()
    {
        var page = await NewPageAsync();

        await page.Locator("#Amount").FillAsync("0.00");
        await page.Locator("#btnSubmit").ClickAsync();

        var error = await page.Locator("#responseError").TextContentAsync();
        error.Should().Contain("0.01");
    }

    [Fact]
    public async Task Convert_MaxAmount_Succeeds()
    {
        var page = await NewPageAsync();

        await page.Locator("#Amount").FillAsync("999999999999999.99");
        await page.Locator("#btnSubmit").ClickAsync();

        await page.Locator("#resultPanel").WaitForAsync(new() { State = WaitForSelectorState.Visible });
        var text = await page.Locator("#responseAmount").TextContentAsync();
        text.Should().Contain("TRILLION");
    }

    [Fact]
    public async Task Convert_EnterKey_TriggersSubmission()
    {
        var page = await NewPageAsync();

        await page.Locator("#Amount").FillAsync("42.00");
        await page.Locator("#Amount").PressAsync("Enter");

        await page.Locator("#resultPanel").WaitForAsync(new() { State = WaitForSelectorState.Visible });
        var text = await page.Locator("#responseAmount").TextContentAsync();
        text.Should().Contain("FORTY-TWO");
    }

    [Fact]
    public async Task Convert_DuringRequest_ButtonIsDisabled()
    {
        var page = await NewPageAsync();
        var requestStarted = new TaskCompletionSource();
        var requestProceed = new TaskCompletionSource();

        await page.RouteAsync("**/api/show", async route =>
        {
            requestStarted.TrySetResult();
            await requestProceed.Task;
            await route.ContinueAsync();
        });

        await page.Locator("#Amount").FillAsync("1.00");

        var clickTask = page.Locator("#btnSubmit").ClickAsync();
        await requestStarted.Task.WaitAsync(TimeSpan.FromSeconds(5), TestContext.Current.CancellationToken);

        var isDisabled = await page.Locator("#btnSubmit").IsDisabledAsync();
        var btnText = await page.Locator("#btnSubmit .btn-text").TextContentAsync();

        requestProceed.TrySetResult();
        await clickTask;
        await page.Locator("#resultPanel").WaitForAsync(new() { State = WaitForSelectorState.Visible });

        isDisabled.Should().BeTrue();
        btnText.Should().Be("Converting…");
    }

    [Fact]
    public async Task Convert_ClearsPreviousResult_OnNewSubmit()
    {
        var page = await NewPageAsync();

        await page.Locator("#Amount").FillAsync("1.00");
        await page.Locator("#btnSubmit").ClickAsync();
        await page.Locator("#resultPanel").WaitForAsync(new() { State = WaitForSelectorState.Visible });

        await page.Locator("#Amount").FillAsync("2.00");
        await page.Locator("#btnSubmit").ClickAsync();
        await page.Locator("#resultPanel").WaitForAsync(new() { State = WaitForSelectorState.Visible });

        var text = await page.Locator("#responseAmount").TextContentAsync();
        text.Should().Contain("TWO DOLLARS");
        text.Should().NotContain("ONE DOLLAR");
    }
}
