# .NET Version Upgrade Plan

## Overview

**Target**: Upgrade `Money2Word/Money2Word.csproj` from `net10.0` to `net11.0`
**Scope**: Single SDK-style Razor Pages project, ~558 LOC, with 3 low-risk behavioral-change findings and no package updates required

### Selected Strategy
**All-At-Once** — All projects upgraded simultaneously in a single operation.
**Rationale**: 1 SDK-style project, already on modern .NET, no package compatibility issues, and only a few low-impact behavioral changes.

## Tasks

### 01-upgrade-money2word: Upgrade Money2Word to .NET 11

This task upgrades the scoped Razor Pages application to `net11.0`, verifies SDK and `global.json` alignment, and applies the code changes required by the assessment. The scope is centered on `Money2Word/Money2Word.csproj` and the small set of source files affected by the reported .NET behavioral changes, especially URI behavior and exception-handler pipeline usage.

Because the assessment found no incompatible packages and no project-to-project dependencies, this remains a single atomic upgrade task. Research should focus on the project file, startup/pipeline configuration, and the three reported behavioral-change locations before validating the full solution and related tests against the upgraded app.

**Done when**: `Money2Word/Money2Word.csproj` targets `net11.0`, any `global.json` constraints no longer block the upgrade, the reported behavioral-change locations are reviewed and updated as needed, and the solution builds and relevant tests pass without warnings.
