using CommandSystem;
using InventorySystem.Items.Firearms.Modules.Scp127;
using PlayerRoles;
using RemoteAdmin;

namespace KadTextChat;
public class SendValidator
{
    public string CheckMessage(ArraySegment<string> arguments, ICommandSender sender)
    {
        if (sender is not PlayerCommandSender playerSender)
            return "This command can only be ran by a player!";

        if (!playerSender.ReferenceHub.IsHuman() && playerSender.ReferenceHub.GetRoleId() != RoleTypeId.Scp049 && playerSender.ReferenceHub.GetRoleId() != RoleTypeId.Scp0492)
            return "This command is only supported for humans, SCP-049 and SCP-049-2!";

        if (arguments.Count == 0)
            return "You must provide a message to say!";

        //Ensures that if a player is trying to spam a message, even if they've reached the spam cap, they'll be more severely punished
        if (PluginMain.Instance.makeText.playerTextBoxes.ContainsKey(Player.Get(playerSender)))
        {
            if (PluginMain.Instance.makeText.playerTextBoxes[Player.Get(playerSender)].recentMessages >= PluginMain.Instance.Config.messageSpamCap)
                return "You are sending messages too quickly! Please wait before sending another message.";
        }

        return string.Empty;
    }
}

