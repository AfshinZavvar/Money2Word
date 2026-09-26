# .NET Version Upgrade

## Strategy
**Selected**: All-At-Once
**Rationale**: A single SDK-style Razor Pages project on modern .NET with no package compatibility issues and only low-risk behavioral changes is best handled in one atomic pass.

### Execution Constraints
- Upgrade the scoped project in a single atomic pass.
- Verify SDK and any global.json constraints before applying the target framework change.
- Apply project and code updates before running full validation.
- Build the full solution and run relevant tests after the upgrade is complete.

## Preferences
- **Flow Mode**: Automatic
- **Target Framework**: net11.0
- **Scope**: Money2Word/Money2Word.csproj
- **Commit Strategy**: Single Commit at End

## Upgrade Options
**Source**: .github/upgrades/scenarios/dotnet-version-upgrade/upgrade-options.md

### Strategy
- Upgrade Strategy: All-at-Once

## Decisions
- Use All-at-Once for this single SDK-style Razor Pages project because the assessment shows low upgrade complexity.
- Continue the upgrade only after a compatible .NET 11 SDK is installed on this machine.
- A compatible .NET 11 SDK is now installed, so the upgrade can proceed and be validated in both the CLI and Visual Studio.

## Source Control
- **Source Branch**: develop
- **Working Branch**: upgrade-dotnet-11
- **Commit Strategy**: After Each Task
- **Branch Sync**: Auto (Merge)
