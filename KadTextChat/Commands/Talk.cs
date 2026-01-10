using CommandSystem;
using PlayerRoles;
using RemoteAdmin;
using static PlayerRoles.PlayableScps.VisionInformation;
using ICommand = CommandSystem.ICommand;

namespace KadTextChat.Commands;

[CommandHandler(typeof(ClientCommandHandler))]
[CommandHandler(typeof(GameConsoleCommandHandler))]

public class Talk : ICommand
{
    public string Command => "say";
    public string[] Aliases => ["s", "speak", "tell", "text", "t", "talk"];
    public string Description => "Use to speak in text chat!";
    public bool SanitizeResponse => true;

    public SendValidator sendValidator = new SendValidator();
    public event EventHandler CanExecuteChanged;

    public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
    {
        string failReason = sendValidator.CheckMessage(arguments, sender);

        if (failReason != string.Empty)
        {
            response = failReason;
            return false;
        }

        Player talkingPlayer = Player.Get(sender);
        string message = $"💬 {string.Join(" ", arguments)}";
        int messageLength = message.Length;

        if (messageLength >= PluginMain.Instance.Config.maxMessageLength)
        {
            response = $"Your message was too long! [{messageLength}/{PluginMain.Instance.Config.maxMessageLength} Characters]";
            return false;
        }

        foreach (string word in PluginMain.Instance.Config.bannedWords)
        {
            if (message.Contains(word))
            {
                CL.Info($"Censored word detected: {word}");
                message = message.Replace(word, new string('*', word.Length));
            }
            CL.Info($"Censored word not detected");
        }

        //All failure checks passed, create text toy
        if (PluginMain.Instance.makeText.CreateTextBox(talkingPlayer, message, MakeText.TextType.Normal))
        {
            response = $"You said:{message}";
            return true;
        }

        response = $"error";
        return false;

    }


}


