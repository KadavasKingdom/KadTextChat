using CommandSystem;
using PlayerRoles;
using RemoteAdmin;

namespace KadTextChat;
public class SendValidator
{
    public string CheckMessage(ArraySegment<string> arguments, ICommandSender sender)
    {
        if (!PluginMain.Instance.Config.clientCommandsEnabled)
            return "Client commands are disabled!";

        if (sender is not PlayerCommandSender playerSender)
            return "This command can only be ran by a player!";

        if (!playerSender.ReferenceHub.IsHuman() && playerSender.ReferenceHub.GetRoleId() != RoleTypeId.Scp049 && playerSender.ReferenceHub.GetRoleId() != RoleTypeId.Scp0492)
            return "This command is only supported for humans, SCP-049 and SCP-049-2!";

        if (arguments.Count == 0)
            return "You must provide a message to say!";

        if (!PluginMain.Instance.makeText.playerTextBoxes[Player.Get(playerSender)].offCooldown)
            return "You are sending messages too quickly! Please wait before sending another message.";

        return string.Empty;
    }

}

