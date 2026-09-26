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

        await page.Locator(".card").WaitForAsync();
        await page.Locator("#Amount").WaitForAsync();
        await page.Locator("#btnSubmit").WaitForAsync();

        var title = await page.Locator(".card-title").TextContentAsync();
        title.Should().Be("Money to Word");
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

    [Fact]
    public async Task Convert_PendingRequest_BlocksEnterAndKeepsGeometry()
    {
        var page = await NewPageAsync();
        var release = new TaskCompletionSource();
        var requests = 0;
        await page.RouteAsync("**/api/show", async route =>
        {
            Interlocked.Increment(ref requests);
            await release.Task;
            await route.ContinueAsync();
        });
        await page.Locator("#Amount").FillAsync("1234.56");
        var before = await page.Locator("#btnSubmit").BoundingBoxAsync();
        await page.Locator("#Amount").PressAsync("Enter");
        await Assertions.Expect(page.Locator("#btnSubmit")).ToBeDisabledAsync();
        await page.Locator("#Amount").PressAsync("Enter");
        var pending = await page.Locator("#btnSubmit").BoundingBoxAsync();
        var busy = await page.Locator("#btnSubmit").GetAttributeAsync("aria-busy");
        release.TrySetResult();
        await Assertions.Expect(page.Locator("#resultPanel")).ToBeVisibleAsync();
        requests.Should().Be(1);
        busy.Should().Be("true");
        pending.Should().BeEquivalentTo(before);
        (await page.Locator("#btnSubmit").BoundingBoxAsync()).Should().BeEquivalentTo(before);
        await Assertions.Expect(page.Locator(".btn-text")).ToHaveTextAsync("Convert to words");
    }

    [Fact]
    public async Task Convert_AmountEditedWhilePending_DiscardsOldResult()
    {
        var page = await NewPageAsync();
        var release = new TaskCompletionSource();
        await page.RouteAsync("**/api/show", async route =>
        {
            await release.Task;
            await route.ContinueAsync();
        });
        await page.Locator("#Amount").FillAsync("1");
        await page.Locator("#btnSubmit").ClickAsync();
        await page.Locator("#Amount").FillAsync("2");
        release.TrySetResult();
        await Assertions.Expect(page.Locator("#btnSubmit")).ToBeEnabledAsync();
        await Assertions.Expect(page.Locator("#resultPanel")).ToBeHiddenAsync();
        await page.Locator("#Amount").PressAsync("Enter");
        await Assertions.Expect(page.Locator("#responseAmount")).ToHaveTextAsync("TWO DOLLARS");
    }

    [Fact]
    public async Task Convert_ApiFailure_PreservesInputAndAllowsRetry()
    {
        var page = await NewPageAsync();
        await page.RouteAsync("**/api/show", route => route.FulfillAsync(new()
        {
            Status = 400,
            ContentType = "application/problem+json",
            Body = "{\"detail\":\"Conversion unavailable. Please try again.\"}"
        }));
        await page.Locator("#Amount").FillAsync("42");
        await page.Locator("#btnSubmit").ClickAsync();
        await Assertions.Expect(page.Locator("#responseError")).ToHaveTextAsync("Conversion unavailable. Please try again.");
        await Assertions.Expect(page.Locator("#Amount")).ToHaveValueAsync("42");
        await Assertions.Expect(page.Locator("#btnSubmit")).ToBeEnabledAsync();
        await page.UnrouteAsync("**/api/show");
        await page.Locator("#btnSubmit").ClickAsync();
        await Assertions.Expect(page.Locator("#responseAmount")).ToHaveTextAsync("FORTY-TWO DOLLARS");
    }

    [Fact]
    public async Task Validation_FocusesAmount_AndEditingClearsError()
    {
        var page = await NewPageAsync();
        await page.Locator("#btnSubmit").ClickAsync();
        await Assertions.Expect(page.Locator("#Amount")).ToBeFocusedAsync();
        await Assertions.Expect(page.Locator("#Amount")).ToHaveAttributeAsync("aria-invalid", "true");
        await page.Locator("#Amount").FillAsync("5");
        await Assertions.Expect(page.Locator("#responseError")).ToBeEmptyAsync();
        (await page.Locator("#Amount").GetAttributeAsync("aria-invalid")).Should().BeNull();
    }

    [Theory]
    [InlineData(320)]
    [InlineData(375)]
    [InlineData(1280)]
    public async Task LongResult_ReflowsWithoutHorizontalOverflow(int width)
    {
        var page = await NewPageAsync();
        await page.SetViewportSizeAsync(width, 800);
        await page.EmulateMediaAsync(new() { ReducedMotion = ReducedMotion.Reduce });
        await page.Locator("#Amount").FillAsync("999999999999999.99");
        await page.Locator("#Amount").PressAsync("Enter");
        await Assertions.Expect(page.Locator("#responseAmount")).ToContainTextAsync("TRILLION");
        var overflow = await page.EvaluateAsync<bool>("document.documentElement.scrollWidth > window.innerWidth");
        overflow.Should().BeFalse();
        var result = await page.Locator("#responseAmount").BoundingBoxAsync();
        result!.X.Should().BeGreaterThanOrEqualTo(0);
        (result.X + result.Width).Should().BeLessThanOrEqualTo(width);
    }
}
