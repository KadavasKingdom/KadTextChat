using CommandSystem;
using ICommand = CommandSystem.ICommand;

namespace KadTextChat.Commands;

[CommandHandler(typeof(RemoteAdminCommandHandler))]

public class ForceUserCommand : ICommand
{
    public string Command => "forcesay";
    public string[] Aliases => ["fspeak", "ftell", "ftext"];
    public string Description => "forcesay [userID] [1 - Whisper/2 - Normal/3 - Yelling] [message]";
    public bool SanitizeResponse => true;


    public event EventHandler CanExecuteChanged;

    public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
    {
        if (arguments.Count == 0 || arguments == null)
        {
            response = "forcesay [userID] [1 - Whisper/2 - Normal/3 - Yelling] [message]";
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
        message = message.Replace(arguments.ElementAt(0), "");

        int textTypeInt = 0;

        if (int.Parse(arguments.ElementAt(1)) <= 3)
        {
            textTypeInt = int.Parse(arguments.ElementAt(1));
            message = message.Replace(arguments.ElementAt(1), "");
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

        MakeText.TextType textType = textTypeInt switch
        {
            1 => MakeText.TextType.Whisper,
            2 => MakeText.TextType.Normal,
            3 => MakeText.TextType.Yelling,
            _ => MakeText.TextType.Normal,
        };

        if (PluginMain.Instance.makeText.CreateTextBox(target, message, textType))
        {
            response = $"You said:{message}";
            return true;
        }

        response = "Something went wrong!";
        return false;
    }
}


