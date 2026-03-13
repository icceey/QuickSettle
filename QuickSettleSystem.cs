using System;
using System.Diagnostics;
using System.Reflection;
using Microsoft.Xna.Framework;
using MonoMod.RuntimeDetour;
using Terraria;
using Terraria.Chat;
using Terraria.ID;
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
    private const int MaxTotalLoops = 100000;

    private delegate void ProcessMessageDelegate(
        Action<ChatCommandProcessor, ChatMessage, int> orig,
        ChatCommandProcessor self,
        ChatMessage message,
        int clientId);

    private Hook? _processMessageHook;
    private QuickSettleConfig? _config;
    private bool _isSettling;
    private int _totalLoops;
    private readonly Stopwatch _frameStopwatch = new();
    private readonly Stopwatch _settleStopwatch = new();

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
    /// Starts a frame-budgeted liquid settle pass unless one is already in progress.
    /// </summary>
    public void DoSettle()
    {
        if (_isSettling)
        {
            return;
        }

        _isSettling = true;
        _totalLoops = 0;
    }

    public override void PreUpdateEntities()
    {
        if (_isSettling)
        {
            _frameStopwatch.Restart();
        }
    }

    public override void PostUpdateEverything()
    {
        if (Main.netMode == NetmodeID.MultiplayerClient || !_isSettling)
        {
            return;
        }

        if (Liquid.numLiquid <= 0 || _totalLoops >= MaxTotalLoops)
        {
            _isSettling = false;
            ChatHelper.BroadcastChatMessage(
                NetworkText.FromKey("Mods.QuickSettle.LiquidsSettled"),
                Color.Cyan);
            return;
        }

        long remainingTimeMs = 16 - _frameStopwatch.ElapsedMilliseconds;
        long safeBudgetMs = Math.Max(1, remainingTimeMs);

        _settleStopwatch.Restart();

        while (Liquid.numLiquid > 0
               && _totalLoops < MaxTotalLoops
               && _settleStopwatch.ElapsedMilliseconds < safeBudgetMs)
        {
            Liquid.UpdateLiquid();
            _totalLoops++;
        }

        _settleStopwatch.Stop();
    }

    public override void ClearWorld()
    {
        _isSettling = false;
        _totalLoops = 0;
        _frameStopwatch.Reset();
        _settleStopwatch.Reset();
    }
}
