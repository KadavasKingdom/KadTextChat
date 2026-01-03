using CommandSystem;
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


    public event EventHandler CanExecuteChanged;

    public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
    {
        if (sender is not PlayerCommandSender playerSender)
        {
            response = "This command can only be ran by a player!";
            return false;
        }

        if (arguments.Count == 0)
        {
            response = "You must provide a message to say!";
            return false;
        }

        Player talkingPlayer = Player.Get(playerSender);
        string message = $"💬 {string.Join(" ", arguments)}";
        int messageLength = message.Length;

        if (messageLength >= PluginMain.Instance.Config.maxMessageLength)
        {
            response = $"Your message was too long! [{messageLength}/{PluginMain.Instance.Config.maxYellLength} Characters] - use .s to talk quieter and use more characters";
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
        if (PluginMain.Instance.makeText.CreateTextBox(talkingPlayer, message, MakeText.TextType.Yelling))
        {
            response = $"You said:{message}";
            return true;
        }

        response = $"error";
        return false;
    }
}


