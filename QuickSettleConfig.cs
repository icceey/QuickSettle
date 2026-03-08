using System.ComponentModel;
using Terraria.ModLoader.Config;

namespace QuickSettle;

/// <summary>
/// Server-side configuration for QuickSettle.
/// </summary>
public class QuickSettleConfig : ModConfig
{
    public override ConfigScope Mode => ConfigScope.ServerSide;

    /// <summary>
    /// When enabled, any player can trigger liquid settling by sending "1" in chat.
    /// </summary>
    [DefaultValue(true)]
    public bool EnableTriggerByChat { get; set; }
}
