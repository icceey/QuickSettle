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
    /// Enabled by default. When turned on, any player can settle liquids by sending the
    /// exact message "1" in chat. Disabling this does not affect the /settle command.
    /// </summary>
    [DefaultValue(true)]
    public bool EnableTriggerByChat { get; set; }
}
