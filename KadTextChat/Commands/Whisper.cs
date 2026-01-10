using CommandSystem;
using PlayerRoles;
using RemoteAdmin;
using ICommand = CommandSystem.ICommand;

namespace KadTextChat.Commands;

[CommandHandler(typeof(ClientCommandHandler))]
[CommandHandler(typeof(GameConsoleCommandHandler))]

public class Whisper : ICommand
{
    public string Command => "whisper";
    public string[] Aliases => ["w", "whisp", "silent", "silenttalk"];
    public string Description => "Use to whisper in text chat!";
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
            response = $"Your message was too long! [{messageLength}/{PluginMain.Instance.Config.maxWhisperLength} Characters] - use .s to talk louder and use more characters";
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
        if (PluginMain.Instance.makeText.CreateTextBox(talkingPlayer, message, MakeText.TextType.Whisper))
        {
            response = $"You said:{message}";
            return true;
        }

        response = $"error";
        return false;
    }
}


