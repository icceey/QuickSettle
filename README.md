# QuickSettle

**[中文](README_zh.md)** | English

A [tModLoader](https://github.com/tModLoader/tModLoader) mod for Terraria. When a player sends `1` in the in-game chat, it instantly settles all flowing liquids in the world, letting them rush to their final positions at once — eliminating lag from large-scale liquid movement and speeding up your terraforming projects.

## Why This Mod?

In Terraria, digging blocks that expose large bodies of liquid can trigger extensive liquid flow that takes a very long time to settle — especially lava and honey, which flow extremely slowly. This forces players to wait and causes server-side lag. The problem is even more noticeable in multiplayer and large modded servers.

You can work around this by restarting the server or running the `settle` command from the server console — but I didn't want my friends to keep quitting and rejoining (terrible experience). So I made this mod. It invokes Terraria's built-in `settle` command to instantly resolve all flowing liquids and let them reach their final positions. All it takes is typing `1` in chat — no one has to leave the game.

## Installation

### Method 1: Steam Workshop (Recommended)

1. Subscribe to this [Mod](https://steamcommunity.com/sharedfiles/filedetails/?id=3679881040) on the [Steam Workshop](https://steamcommunity.com/app/1281930/workshop/) (or search **QuickSettle** manually).
2. Launch tModLoader, go to **Mods**, find **QuickSettle**, and enable it.
3. Reload mods. Done!

### Method 2: Manual Installation

1. Download the latest `QuickSettle.tmod` from the [Releases](../../releases) page.
2. Copy it to your tModLoader `Mods` folder:
   ```
   ~/.local/share/Terraria/tModLoader/Mods/          # Linux
   %USERPROFILE%\Documents\My Games\Terraria\tModLoader\Mods\  # Windows
   ~/Library/Application Support/Terraria/tModLoader/Mods/  # macOS
   ```
3. Launch tModLoader, go to **Mods**, enable **QuickSettle**, and reload mods.

## Usage

> After installing the mod, remember to enable it in the mod list and reload mods.

Send `1` in the in-game chat to instantly settle all flowing liquids in the world.
