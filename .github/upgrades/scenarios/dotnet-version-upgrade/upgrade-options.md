# Upgrade Options — Money2WordCore

Assessment: 1 SDK-style ASP.NET Core project on net10.0 with low-complexity behavioral changes and no package compatibility issues.

## Strategy

### Upgrade Strategy
A single modern .NET project with no dependency graph or incompatible packages fits a direct upgrade.

| Value | Description |
|-------|-------------|
| **All-at-Once** (selected) | Upgrade the project in a single atomic pass. |
| Top-Down | Upgrade applications first and keep shared components buildable incrementally. |
