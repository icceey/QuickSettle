
# QuickSettle

[English](README.md) | **中文**

一个 [tModLoader](https://github.com/tModLoader/tModLoader) 的 Terraria Mod，在玩家于服务器聊天框中发送 `1` 时，立即结算世界中所有流动的液体，让所有流动中的液体快速流动到最终位置，消除大规模液体流动带来的卡顿，并加速你的地形改造工程。

## 为什么做这个 Mod？

在 Terraria 中，挖掘方块后导致大面积的液体流动往往需要花费很长时间（特别是岩浆和蜂蜜，它们的流动十分缓慢），这不仅需要玩家耐心等待，还会导致服务器卡顿。尤其是在多人游戏和大型模组服务器中，这种情况更为明显。

你可以通过重启服务器或是开启命令行之后使用 `settle` 命令来结算液体，但我不想让朋友们反复退出-重进游戏（这体验并不好），所以我做了这个mod，它通过触发 Terraria 内置的 `settle` 命令来解决这一问题，立即结算所有流动的液体，让它们快速流动到最终位置，你只需扣1就能做到，无需再退出游戏。

## 如何安装

### 方式一：Steam 创意工坊（推荐）

1. 在 [Steam 创意工坊](https://steamcommunity.com/app/1281930/workshop/) 中订阅本 [Mod](https://steamcommunity.com/sharedfiles/filedetails/?id=3679881040)（或手动在创意工坊搜索 **QuickSettle**）。
2. 启动 tModLoader，进入 **Mods**，找到 **QuickSettle** 并启用。
3. 重新加载 Mod，完成！

### 方式二：手动安装

1. 在 [Releases](../../releases) 页面下载最新的 `QuickSettle.tmod` 文件。
2. 将文件复制到 tModLoader 的 `Mods` 目录：
   ```
   ~/.local/share/Terraria/tModLoader/Mods/          # Linux
   %USERPROFILE%\Documents\My Games\Terraria\tModLoader\Mods\  # Windows
   ~/Library/Application Support/Terraria/tModLoader/Mods/  # macOS
   ```
3. 启动 tModLoader，进入 **Mods**，启用 **QuickSettle** 并重新加载 Mod。

## 如何使用

> 安装mod后，记得在模组列表中启用它，并重新加载模组。

在游戏内聊天框中扣 `1`，立即结算世界中所有流动的液体。
