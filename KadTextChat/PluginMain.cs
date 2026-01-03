using LabApi.Loader.Features.Plugins;
using LabApi.Loader.Features.Plugins.Enums;

namespace KadTextChat;

public class PluginMain : Plugin<Config>
{
    public static PluginMain Instance;
    public override string Name => "KadTextChat";
    public override string Author => "KadavaSmile";
    public override Version Version => new(0, 1);
    public override Version RequiredApiVersion => LabApi.Features.LabApiProperties.CurrentVersion;
    public override LoadPriority Priority => LoadPriority.Lowest;
    public override string Description => "Allows players to use text chat, appearing above their heads in real space";
    public MakeText makeText = new MakeText();
    public override void Enable()
    {
        Instance = this;
        AudioClipStorage.LoadClip(CustomAudioHub.Hub.MakeFilePath(PluginMain.Instance.Config.speakSFX), "speakingSFX");
        AudioClipStorage.LoadClip(CustomAudioHub.Hub.MakeFilePath(PluginMain.Instance.Config.speak049SFX), "speaking049SFX");
        AudioClipStorage.LoadClip(CustomAudioHub.Hub.MakeFilePath(PluginMain.Instance.Config.speak0492SFX), "speaking0492SFX");
    }

    public override void Disable()
    {
        Instance = null;
    }
}

