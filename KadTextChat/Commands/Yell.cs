using CommandSystem;
using PlayerRoles;
using RemoteAdmin;
using ICommand = CommandSystem.ICommand;

namespace KadTextChat.Commands;

[CommandHandler(typeof(ClientCommandHandler))]
[CommandHandler(typeof(GameConsoleCommandHandler))]

public class Yell : ICommand
{
    public string Command => "yell";
    public string[] Aliases => ["y", "yelling", "shout", "loud", "scream"];
    public string Description => "Use to yell in text chat!";
    public bool SanitizeResponse => true;

    public SendValidator sendValidator = new SendValidator();
    public event EventHandler CanExecuteChanged;

    public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
    {
        string failReason = sendValidator.CheckMessage(arguments, sender);

        if (failReason != string.Empty || failReason == null)
        {
            response = failReason;
            return false;
        }

        Player talkingPlayer = Player.Get(sender);
        string message = $"💬 {string.Join(" ", arguments)}";
        int messageLength = message.Length;

        if (messageLength >= PluginMain.Instance.Config.maxYellLength)
        {
            response = $"Your message was too long! [{messageLength}/{PluginMain.Instance.Config.maxYellLength} Characters] - use .talk to talk quieter and use more characters";
            return false;
        }

        foreach (string word in PluginMain.Instance.Config.bannedWords)
        {
            if (message.Contains(word))
            {
                CL.Info($"Censored word detected: {word}");
                message = message.Replace(word, new string('*', word.Length));
            }
        }

        //All failure checks passed, create text toy
        if (PluginMain.Instance.makeText.CreateTextBox(talkingPlayer, message, MakeText.TextType.Yelling))
        {
            response = $"You said:{message}";
            return true;
        }

        response = $"error";
        return false;
    }
}


