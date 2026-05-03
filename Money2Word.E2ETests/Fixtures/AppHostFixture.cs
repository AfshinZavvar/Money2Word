using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Hosting;
using System.Net;
using System.Net.Sockets;

namespace Money2Word.E2ETests.Fixtures;

/// <summary>
/// Starts the ASP.NET Core app on a real Kestrel port so Playwright can connect to it.
/// WebApplicationFactory alone uses an in-memory TestServer that browsers cannot reach.
/// </summary>
public sealed class AppHostFixture : WebApplicationFactory<Program>, IAsyncLifetime
{
    public string BaseUrl { get; private set; } = "";

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Development");
    }

    protected override IHost CreateHost(IHostBuilder builder)
    {
        // Build the standard in-memory host (required by WebApplicationFactory internals)
        var dummyHost = base.CreateHost(builder);

        // Find a free port first, then bind Kestrel to it directly.
        // IServerAddressesFeature.Addresses is only populated via UseUrls(), not via
        // the Listen() / KestrelServerOptions path, so we determine the port ourselves.
        var port = FindFreePort();
        BaseUrl = $"http://localhost:{port}";

        builder.ConfigureWebHost(b =>
            b.UseKestrel(opts => opts.Listen(IPAddress.Loopback, port)));

        var realHost = builder.Build();
        realHost.Start();

        return new DualHost(dummyHost, realHost);
    }

    ValueTask IAsyncLifetime.InitializeAsync()
    {
        _ = Server; // trigger lazy CreateHost / DualHost setup
        return ValueTask.CompletedTask;
    }

    // DisposeAsync() is inherited from WebApplicationFactory (IAsyncDisposable)
    // and satisfies IAsyncLifetime.DisposeAsync() — no override needed

    private static int FindFreePort()
    {
        using var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        socket.Bind(new IPEndPoint(IPAddress.Loopback, 0));
        return ((IPEndPoint)socket.LocalEndPoint!).Port;
    }
}

file sealed class DualHost(IHost primary, IHost secondary) : IHost
{
    public IServiceProvider Services => primary.Services;

    public void Dispose()
    {
        primary.Dispose();
        secondary.Dispose();
    }

    public Task StartAsync(CancellationToken cancellationToken = default) =>
        Task.WhenAll(primary.StartAsync(cancellationToken), secondary.StartAsync(cancellationToken));

    public Task StopAsync(CancellationToken cancellationToken = default) =>
        Task.WhenAll(primary.StopAsync(cancellationToken), secondary.StopAsync(cancellationToken));
}
