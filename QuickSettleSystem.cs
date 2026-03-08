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
/// Hooks incoming server chat so the exact message "1" can trigger the settle routine
/// when the server-side config allows it. The trigger message is consumed instead of
/// continuing through normal chat processing.
/// </summary>
public class QuickSettleSystem : ModSystem
{
    private delegate void ProcessMessageDelegate(
        Action<ChatCommandProcessor, ChatMessage, int> orig,
        ChatCommandProcessor self,
        ChatMessage message,
        int clientId);

    private Hook? _processMessageHook;
    private QuickSettleConfig? _config;

    public override void Load()
    {
        _config = ModContent.GetInstance<QuickSettleConfig>();

        var method = typeof(ChatCommandProcessor).GetMethod(
            "ProcessIncomingMessage",
            BindingFlags.Public | BindingFlags.Instance)!;

        _processMessageHook = new Hook(
            method,
            (ProcessMessageDelegate)(
                (orig, self, message, clientId) =>
                {
                    if (_config.EnableTriggerByChat && message.Text == "1")
                    {
                        DoSettle();
                        return;
                    }

                    orig(self, message, clientId);
                }));
    }

    public override void Unload()
    {
        _processMessageHook?.Dispose();
        _processMessageHook = null;
        _config = null;
    }

    /// <summary>
    /// Runs Terraria's liquid update loop until no flowing liquids remain or a safety
    /// cap is reached, then broadcasts a completion message to players.
    /// </summary>
    public static void DoSettle()
    {
        const int maxLoop = 100000;
        int currentLoop = 0;

        while (Liquid.numLiquid > 0 && currentLoop < maxLoop)
        {
            Liquid.UpdateLiquid();
            currentLoop++;
        }

        ChatHelper.BroadcastChatMessage(
            NetworkText.FromKey("Mods.QuickSettle.LiquidsSettled"),
            Color.Cyan);
    }
}
