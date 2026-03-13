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
    private const double FrameTimeBudgetMs = 16;
    private const int MaxTotalIterations = 100000;
    private static readonly TimeSpan FrameTimeBudget = TimeSpan.FromMilliseconds(FrameTimeBudgetMs);

    private delegate void ProcessMessageDelegate(
        Action<ChatCommandProcessor, ChatMessage, int> orig,
        ChatCommandProcessor self,
        ChatMessage message,
        int clientId);

    private Hook? _processMessageHook;
    private QuickSettleConfig? _config;
    private readonly Stopwatch _frameBudgetStopwatch = new();
    private bool _isSettling;
    private int _totalLoops;

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
        ResetSettleState();
    }

    public override void OnWorldLoad()
    {
        ResetSettleState();
    }

    public override void OnWorldUnload()
    {
        ResetSettleState();
    }

    public override void PreUpdateEntities()
    {
        if (_isSettling)
        {
            _frameBudgetStopwatch.Restart();
        }
    }

    /// <summary>
    /// Marks the world to continue liquid settling during update hooks without blocking
    /// the main thread with a single synchronous loop.
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

    public override void PostUpdateEverything()
    {
        if (Main.netMode == NetmodeID.MultiplayerClient || !_isSettling)
        {
            return;
        }

        if (Liquid.numLiquid == 0)
        {
            FinishSettling();
            return;
        }

        if (_totalLoops >= MaxTotalIterations)
        {
            AbortSettling();
            return;
        }

        TimeSpan remainingFrameBudget =
            FrameTimeBudget - _frameBudgetStopwatch.Elapsed;

        if (remainingFrameBudget <= TimeSpan.Zero)
        {
            RunSingleFallbackUpdate();
            return;
        }

        long sliceStartTimestamp = Stopwatch.GetTimestamp();

        while (Liquid.numLiquid > 0
               && _totalLoops < MaxTotalIterations
               && Stopwatch.GetElapsedTime(sliceStartTimestamp) <= remainingFrameBudget)
        {
            Liquid.UpdateLiquid();
            _totalLoops++;
        }

        if (Liquid.numLiquid == 0)
        {
            FinishSettling();
        }
        else if (_totalLoops >= MaxTotalIterations)
        {
            AbortSettling();
        }
    }

    private void FinishSettling()
    {
        ResetSettleState();
        ChatHelper.BroadcastChatMessage(
            NetworkText.FromKey("Mods.QuickSettle.LiquidsSettled"),
            Color.Cyan);
    }

    private void AbortSettling()
    {
        int remainingLiquid = Liquid.numLiquid;
        ResetSettleState();
        ChatHelper.BroadcastChatMessage(
            NetworkText.FromKey("Mods.QuickSettle.LiquidsSettleAborted", remainingLiquid),
            Color.Orange);
    }

    private void RunSingleFallbackUpdate()
    {
        Liquid.UpdateLiquid();
        _totalLoops++;

        if (Liquid.numLiquid == 0)
        {
            FinishSettling();
        }
        else if (_totalLoops >= MaxTotalIterations)
        {
            AbortSettling();
        }
    }

    private void ResetSettleState()
    {
        _isSettling = false;
        _totalLoops = 0;
        _frameBudgetStopwatch.Reset();
    }
}
