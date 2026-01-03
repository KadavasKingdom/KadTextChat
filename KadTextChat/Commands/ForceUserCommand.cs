using CommandSystem;
using ICommand = CommandSystem.ICommand;

namespace KadTextChat.Commands;

[CommandHandler(typeof(RemoteAdminCommandHandler))]

public class ForceUserCommand : ICommand
{
    public string Command => "forcesay";
    public string[] Aliases => ["fs", "fspeak", "ftell", "ftext"];
    public string Description => "Use to speak in text chat!";
    public bool SanitizeResponse => true;


    public event EventHandler CanExecuteChanged;

    public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
    {
        if (arguments.Count == 0 || arguments == null)
        {
            response = ".forcesay [userID] [message]";
            return false;
        }

        Player target = Player.Get(int.Parse(arguments.ElementAt(0)));

        if (target == null)
        {
            CL.Info($"Target player not found {arguments.ElementAt(0)}");
            response = "Target invalid";
            return false;
        }

        string message = $"💬 {string.Join(" ", arguments)}";
        int messageLength = message.Length;

        if (messageLength >= PluginMain.Instance.Config.maxMessageLength)
        {
            response = $"Your message was too long! [{messageLength}/{PluginMain.Instance.Config.maxMessageLength} Characters]";
            return false;
        }

        message = message.Replace(arguments.ElementAt(0), "");

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
        if (PluginMain.Instance.makeText.CreateTextBox(target, message, MakeText.TextType.Normal))
        {
            response = $"You said:{message}";
            return true;
        }

        response = $"error";
        return false;
    }
}


