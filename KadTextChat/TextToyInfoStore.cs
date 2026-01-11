
using System.ComponentModel;

namespace KadTextChat;

public sealed class TextToyInfoStore
{
    public TextToy textToy = null;
    public AudioPlayer audioPlayer = null;
    public Speaker audioSpeaker = null;
    public TextComponent textComponent;
    //Made as a float so it takes longer to decrement than increment
    public float recentMessages = 0;
}

