# 01-upgrade-money2word: Upgrade Money2Word to .NET 11

This task upgrades the scoped Razor Pages application to `net11.0`, verifies SDK and `global.json` alignment, and applies the code changes required by the assessment. The scope is centered on `Money2Word/Money2Word.csproj` and the small set of source files affected by the reported .NET behavioral changes, especially URI behavior and exception-handler pipeline usage.

Because the assessment found no incompatible packages and no project-to-project dependencies, this remains a single atomic upgrade task. Research should focus on the project file, startup/pipeline configuration, and the three reported behavioral-change locations before validating the full solution and related tests against the upgraded app.

## Research Findings

- The target framework is centrally defined in `Directory.Build.props`, so the framework upgrade applies to `Money2Word`, `Money2Word.Tests`, and `Money2Word.E2ETests` together.
- `global.json` originally pinned SDK `10.0.203` with `rollForward: latestPatch`, which blocked the project from running on the installed SDKs until it was updated.
- After the .NET 11 SDK was installed, `dotnet --version` resolved to `11.0.100-rc.1.26425.128`, but Visual Studio builds still used the 10.0 SDK until `global.json` was explicitly pinned to the installed 11.0 SDK.
- The assessment’s behavioral-change hotspots map to `Money2Word/Program.cs` (`UseExceptionHandler("/Home/Error")`) and `Money2Word/Telemetry/RequestFilterProcessor.cs` (`request.Url?.AbsolutePath`); these were reviewed and required no source changes for this upgrade.
- Full solution build and tests now pass on `net11.0`, confirming the SDK/tooling alignment and framework upgrade are working end to end.

**Done when**: `Money2Word/Money2Word.csproj` targets `net11.0`, any `global.json` constraints no longer block the upgrade, the reported behavioral-change locations are reviewed and updated as needed, and the solution builds and relevant tests pass without warnings.
