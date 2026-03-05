using System;
using System.Reflection;
using Microsoft.Xna.Framework;
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

    private static readonly ProcessMessageDelegate ProcessMessageHook = OnProcessIncomingMessage;

    public override void Load()
    {
        var method = typeof(ChatCommandProcessor).GetMethod(
            "ProcessIncomingMessage",
            BindingFlags.Public | BindingFlags.Instance);

        MonoModHooks.Add(method!, ProcessMessageHook);
    }

    private static void OnProcessIncomingMessage(
        Action<ChatCommandProcessor, ChatMessage, int> orig,
        ChatCommandProcessor self,
        ChatMessage message,
        int clientId)
    {
        if (message.Text == "1")
        {
            Liquid.QuickWater(2, -1, -1);
            ChatHelper.BroadcastChatMessage(
                NetworkText.FromLiteral("All liquids have been settled."),
                Color.Cyan);
            return;
        }

        orig(self, message, clientId);
    }
}
