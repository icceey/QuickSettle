---
name: tmodloader-upgrade
description: Use when upgrading the QuickSettle Terraria mod to a newer tModLoader version, reviewing TMLVERSION pins, choosing stable or requested preview targets, checking official release notes/API docs, adapting C# mod code/workflows, and restoring restore/build/format validation.
---

# tModLoader Upgrade

## Overview

Use this repository-specific skill when moving QuickSettle to a newer tModLoader release. Treat the work as a compatibility upgrade, not just a version-string bump.

Default to the latest stable tModLoader release. Target a preview or pre-release only when the user explicitly asks for it or the repository is clearly already mid-upgrade to that channel.

## Repository Facts

- The currently pinned tModLoader version is stored as `TMLVERSION` in `.github/workflows/build.yml`, `.github/workflows/format.yml`, and `.github/workflows/release.yml`.
- Keep those three workflow pins aligned. `renovate.json` depends on the `TMLVERSION: v...` regex shape.
- The mod builds from `QuickSettle.csproj`; there is no solution file.
- `QuickSettle.csproj` imports `tMLMod.targets` from `TMLInstallDir`.
- Runtime behavior is mainly in `QuickSettleSystem.cs`; `/settle` command behavior is in `SettleCommand.cs`.
- User-facing behavior changes may require paired updates in `Localization/en-US.hjson`, `Localization/zh-Hans.hjson`, `README.md`, `README_zh.md`, and `description.txt`.

## Authoritative Sources

- Determine release numbers and release-channel status from the upstream `tModLoader/tModLoader` GitHub Releases page.
- Read release entries between the current and target versions. Use upstream release notes or source evidence for breaking changes, not third-party summaries.
- For API uncertainty, consult the official stable docs at `https://docs.tmodloader.net/docs/stable/index.html` before editing code.
- If the user requested a preview upgrade and stable docs do not cover it, state the documentation gap and rely on preview release notes plus upstream source evidence.

## Required Workflow

1. Read every current `TMLVERSION` pin and identify all files that depend on the tModLoader version.
2. Select the target release. Use latest stable by default; use preview or pre-release only when explicitly requested or already implied by the repo state.
3. Inspect upstream release entries across the version gap and collect upgrade-relevant changes.
4. Produce a short impact assessment focused on breaking changes and files likely to need edits.
5. Consult official docs before changing uncertain API calls, hook signatures, properties, or recommended patterns.
6. If the current pinned version already equals the selected target version, continue with compatibility review and validation instead of stopping.
7. Update workflow pins, project configuration, source code, localization, and docs only where the upgrade requires it.
8. Build against the selected target version and use compiler or formatter output to drive additional fixes.
9. Re-run validation until the repository is passing or blocked by a concrete external limitation.
10. Summarize what changed, why it changed, validation results, and remaining risk.

## Validation

Use .NET 8 and set `TMLInstallDir` to a tModLoader install containing `tMLMod.targets`.

If a local install is unavailable, download the selected release into `/tmp/tModLoader` before validating:

```bash
curl -sL https://github.com/tModLoader/tModLoader/releases/download/<target-version>/tModLoader.zip -o /tmp/tModLoader.zip
mkdir -p /tmp/tModLoader
unzip -q /tmp/tModLoader.zip -d /tmp/tModLoader
```

Run compile-only validation in this order:

```bash
TMLInstallDir=/tmp/tModLoader dotnet restore QuickSettle.csproj
TMLInstallDir=/tmp/tModLoader dotnet build QuickSettle.csproj --no-restore -p:BuildMod=false
TMLInstallDir=/tmp/tModLoader dotnet format QuickSettle.csproj --verify-no-changes --no-restore
```

Treat packaging as separate from compile-only validation. Run it when the user asks for release validation or when upgrade changes may affect packaging:

```bash
mkdir -p /tmp/tml_save/Mods
TMLInstallDir=/tmp/tModLoader dotnet build QuickSettle.csproj --no-restore -p:ExtraBuildModFlags="-tmlsavedirectory /tmp/tml_save -nosteam"
```

## Constraints

- Do not guess about release status, breaking changes, or API signatures.
- Do not use preview or pre-release targets unless requested or already implied by the repo state.
- Do not stop at version-string changes; complete required code, workflow, and documentation adaptations.
- Do not change mod behavior unless the new tModLoader version requires it for compatibility.
- Keep the explicit `TargetFramework` in `QuickSettle.csproj` unless the new tModLoader version makes a documented change necessary.
- Keep edits scoped to the upgrade and avoid unrelated refactors.
- Do not report success unless restore, build, and format checks have run, or the exact blocker is documented.

## Output Format

Return a concise upgrade report with these sections:

### Version Summary

- Current pinned version
- Target version
- Release channel used: stable, preview, or pre-release
- Source used to determine the target version

### Release Evidence

- The tModLoader release entries consulted
- Whether the repository was already pinned to the target version before review

### Breaking Changes

- Upgrade-relevant breaking or risky changes
- How each one affects this repository

### Applied Changes

- Each modified file
- Purpose of the change in that file

### Validation

- Restore, build, format, and optional packaging results
- Exact blocker for any command that could not be executed

### Remaining Risk

- Residual uncertainty, manual follow-up, or upstream issue
