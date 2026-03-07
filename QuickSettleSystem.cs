using System;
using System.Reflection;
using Microsoft.Xna.Framework;
using MonoMod.RuntimeDetour;
using Terraria;
using Terraria.Chat;
using Terraria.Localization;
using Terraria.ModLoader;

namespace QuickSettle;

/// <summary>
/// Intercepts server chat messages and runs the liquid-settle command
/// when any player sends "1".
/// </summary>
public class QuickSettleSystem : ModSystem
{
    private delegate void ProcessMessageDelegate(
        Action<ChatCommandProcessor, ChatMessage, int> orig,
        ChatCommandProcessor self,
        ChatMessage message,
        int clientId);

    private Hook? _processMessageHook;

    public override void Load()
    {
        var method = typeof(ChatCommandProcessor).GetMethod(
            "ProcessIncomingMessage",
            BindingFlags.Public | BindingFlags.Instance)!;

        _processMessageHook = new Hook(method, (ProcessMessageDelegate)OnProcessIncomingMessage);
    }

    public override void Unload()
    {
        _processMessageHook?.Dispose();
        _processMessageHook = null;
    }

    private static void OnProcessIncomingMessage(
        Action<ChatCommandProcessor, ChatMessage, int> orig,
        ChatCommandProcessor self,
        ChatMessage message,
        int clientId)
    {
        if (message.Text == "1")
        {
            int maxLoop = 100000;
            int currentLoop = 0;

            while (Liquid.numLiquid > 0 && currentLoop < maxLoop)
            {
                Liquid.UpdateLiquid();
                currentLoop++;
            }

            ChatHelper.BroadcastChatMessage(
                NetworkText.FromKey("Mods.QuickSettle.LiquidsSettling"),
                Color.Cyan);
            return;
        }

        orig(self, message, clientId);
    }
}
