using Microsoft.Playwright;

namespace Money2Word.E2ETests.Fixtures;

public sealed class PlaywrightFixture : IAsyncLifetime
{
    public IPlaywright Playwright { get; private set; } = null!;
    public IBrowser Browser { get; private set; } = null!;

    public async ValueTask InitializeAsync()
    {
        Playwright = await Microsoft.Playwright.Playwright.CreateAsync();
        //The tests as written are CI-ready (headless). When you want to watch them run, flip Headless = false locally — no other change needed.
        Browser = await Playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions { Headless = true });
    }

    public async ValueTask DisposeAsync()
    {
        await Browser.CloseAsync();
        Playwright.Dispose();
    }
}