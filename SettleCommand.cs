using Terraria.Localization;
using Terraria.ModLoader;

namespace QuickSettle;

/// <summary>
/// Registers the /settle chat command which instantly settles all flowing liquids.
/// </summary>
public class SettleCommand : ModCommand
{
    public override string Command => "settle";

    public override CommandType Type => CommandType.World;

    public override string Description =>
        Language.GetTextValue("Mods.QuickSettle.Commands.Settle.Description");

    public override void Action(CommandCaller caller, string input, string[] args)
    {
        QuickSettleSystem.DoSettle();
    }
}
