# AGENTS.md

## Repo Shape
- This is a single tModLoader/Terraria mod; there is no solution file, so target `QuickSettle.csproj` directly.
- `QuickSettleMod.cs` is only the `Mod` entry point. Runtime behavior is in `QuickSettleSystem.cs`; it hooks `ChatCommandProcessor.ProcessIncomingMessage`, consumes exact chat text `1`, and settles liquids from `PostUpdateEverything`.
- `/settle` is registered in `SettleCommand.cs` and calls `QuickSettleSystem.DoSettle()` regardless of `QuickSettleConfig.EnableTriggerByChat`.
- User-facing text is split across `Localization/en-US.hjson`, `Localization/zh-Hans.hjson`, `README.md`, `README_zh.md`, and bilingual `description.txt`; update paired English/Chinese surfaces together when behavior changes.

## Build And Verify
- Use .NET 8 and set `TMLInstallDir` to a tModLoader install containing `tMLMod.targets`; the project default is the Linux Steam path and is usually wrong on macOS or CI scratch dirs.
- CI pins tModLoader `v2026.03.3.0`. To mirror CI without a local install: `curl -sL https://github.com/tModLoader/tModLoader/releases/download/v2026.03.3.0/tModLoader.zip -o /tmp/tModLoader.zip && mkdir -p /tmp/tModLoader && unzip -q /tmp/tModLoader.zip -d /tmp/tModLoader`.
- Standard verification order: `TMLInstallDir=/tmp/tModLoader dotnet restore QuickSettle.csproj`, then `TMLInstallDir=/tmp/tModLoader dotnet build QuickSettle.csproj --no-restore -p:BuildMod=false`, then `TMLInstallDir=/tmp/tModLoader dotnet format QuickSettle.csproj --verify-no-changes --no-restore`.
- There is no test project; compile and format are the CI checks.
- Packaging is separate from compile-only validation: create `/tmp/tml_save/Mods`, then run `TMLInstallDir=/tmp/tModLoader dotnet build QuickSettle.csproj --no-restore -p:ExtraBuildModFlags="-tmlsavedirectory /tmp/tml_save -nosteam"` to write `.tmod` output under the supplied tML save dir.
- Keep the explicit `TargetFramework` in `QuickSettle.csproj`; the comment there documents why compile-only builds with `-p:BuildMod=false` need it.

## Versioning And Release
- Keep `TMLVERSION` aligned in `.github/workflows/build.yml`, `.github/workflows/format.yml`, and `.github/workflows/release.yml`; `renovate.json` depends on the `TMLVERSION: v...` regex shape.
- Use `.agents/skills/tmodloader-upgrade/SKILL.md` for tModLoader upgrades; it has stricter release-note, API-doc, and validation requirements.
- Releases are created only from `v*` tags and attach the packaged `.tmod`.

## Style
- C# uses file-scoped `namespace QuickSettle;`, nullable enabled, 4-space indentation, LF, UTF-8, and final newlines.
- HJSON localization files currently use tab indentation; preserve the local style when editing them.
- Prefer small changes to the existing root-level mod files; do not introduce a multi-project layout unless explicitly requested.
