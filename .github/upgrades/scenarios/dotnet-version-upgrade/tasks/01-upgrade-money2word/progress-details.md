# 01-upgrade-money2word Progress Details

## What changed
- Updated `Directory.Build.props` to target `net11.0` centrally for the app and both test projects.
- Updated `global.json` to pin the installed `.NET 11` SDK (`11.0.100-rc.1.26425.128`) with `rollForward: latestPatch`, which aligns Visual Studio and CLI builds on the same toolchain.
- Reviewed the assessment’s behavioral-change hotspots in `Money2Word/Program.cs` and `Money2Word/Telemetry/RequestFilterProcessor.cs`; no source changes were required.
- Updated task research notes and scenario decisions to reflect the installed SDK and successful upgrade validation.

## Validation
- `validate_dotnet_sdk_installation(net11.0)` reported a compatible SDK is installed.
- `validate_dotnet_sdk_in_globaljson(net11.0)` reported no additional changes were needed before the final pin.
- `dotnet --version` resolves to `11.0.100-rc.1.26425.128`.
- `run_build` succeeded for the full solution after the `global.json` and target-framework updates.
- `dotnet test D:\Workspase\Money2WordCore\Money2WordCore.sln --no-build` passed: 44 tests, 0 failures.

## Findings
- The original run issue was caused by SDK resolution, not by source-level build errors.
- The repo required both the framework upgrade and an explicit `global.json` SDK pin so Visual Studio would stop using the 10.0 SDK for IDE builds.
- The assessed behavioral changes did not require code modifications for this upgrade path.
