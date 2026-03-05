# QuickSettle

A [tModLoader](https://github.com/tModLoader/tModLoader) mod for Terraria that instantly settles all flowing liquids when a player sends `1` in the server chat.

## Features

- Type `1` in the server chat to instantly settle all flowing liquids in the world.
- A confirmation message is broadcast to all players when liquids are settled.

## Requirements

- Terraria 1.4.4
- tModLoader v2026.01+

## Building Locally

1. Install [tModLoader](https://store.steampowered.com/app/1281930/tModLoader/) via Steam.
2. Clone this repository into your tModLoader `ModSources` directory:
   ```
   ~/.local/share/Terraria/tModLoader/ModSources/QuickSettle   # Linux
   %USERPROFILE%\Documents\My Games\Terraria\tModLoader\ModSources\QuickSettle  # Windows
   ```
3. Build with:
   ```sh
   dotnet build QuickSettle.csproj
   ```
   The `.tmod` file will be placed in your tModLoader `Mods` folder automatically.

## CI / Workflows

| Workflow | Trigger | Description |
|----------|---------|-------------|
| **Build** | push / PR | Compile check — verifies the mod builds without errors |
| **Format Check** | push / PR | Ensures code matches `.editorconfig` formatting rules |
| **Release** | `v*` tag push | Packages the mod into a `.tmod` file and creates a GitHub Release |

To publish a release, push a tag:
```sh
git tag v1.0.0
git push origin v1.0.0
```
