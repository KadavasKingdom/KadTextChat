namespace KadTextChat;

public class Config
{
    public bool Debug { get; set; }
    public int maxMessageLength { get; set; } = 80;
    public int maxWhisperLength { get; set; } = 20;
    public int maxYellLength { get; set; } = 40;
    public string speakSFX { get; set; } = "talking";
    public string speak0492SFX { get; set; } = "talkingzombie";
    public string speak049SFX { get; set; } = "talkingdoctor";
    public List<string> bannedWords { get; set; }

}

