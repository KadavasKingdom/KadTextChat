
using System.ComponentModel;

namespace KadTextChat;

public sealed class TextToyInfoStore
{
    public TextToy textToy = null;
    public AudioPlayer audioPlayer = null;
    public Speaker audioSpeaker = null;
    public TextComponent textComponent;
    public bool offCooldown = true;
}

