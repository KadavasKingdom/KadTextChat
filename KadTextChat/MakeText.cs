using MEC;
using UnityEngine;

namespace KadTextChat;

public class MakeText
{
    public Dictionary<Player, TextToyInfoStore> playerTextBoxes = new Dictionary<Player, TextToyInfoStore>();
    public enum TextType
    {
        Normal,
        Whisper,
        Yelling
    }

    public bool CreateTextBox(Player talkingPlayer, string message, TextType type)
    {
        CL.Info("1");
        if (!playerTextBoxes.ContainsKey(talkingPlayer))
            playerTextBoxes.Add(talkingPlayer, new TextToyInfoStore());
        CL.Info("2");
        //Creating Text Toy

        TextToy toy = playerTextBoxes[talkingPlayer].textToy;
        if (toy != null && !toy.IsDestroyed)
            toy.Destroy();

        CL.Info("3");
        playerTextBoxes[talkingPlayer].textToy = TextToy.Create(
            position: new Vector3(0f, talkingPlayer.GameObject.transform.localScale.y + 0.3f, 0f),
            parent: talkingPlayer.GameObject.transform,
            rotation: new Quaternion(0, 0, 0, 0),
            scale: new Vector3(-.14f, .14f, .14f));



        CL.Info("4");
        playerTextBoxes[talkingPlayer].textToy.GameObject.name = $"{message}";
        playerTextBoxes[talkingPlayer].textToy.TextFormat = message;
        playerTextBoxes[talkingPlayer].textToy.Spawn();
        CL.Info("5");
        ScheduleDestroy(talkingPlayer, message.Length);
        CL.Info("6");

        CreateAudioPlayer(talkingPlayer, type);
        CL.Info($"{talkingPlayer.Nickname} {type}:\n{message}");
        return true;
    }

    private void CreateAudioPlayer(Player talkingPlayer, TextType type)
    {
        CL.Info("a");
        playerTextBoxes[talkingPlayer].audioPlayer = AudioPlayer.CreateOrGet($"SpeakingAudioPlayer{talkingPlayer.PlayerId}", onIntialCreation: p =>
        {
            p.transform.parent = talkingPlayer.GameObject.transform;
            playerTextBoxes[talkingPlayer].audioSpeaker = p.AddSpeaker($"Talking-Speaker{talkingPlayer.PlayerId}", isSpatial: true);
            playerTextBoxes[talkingPlayer].audioSpeaker.transform.parent = talkingPlayer.GameObject.transform;
            playerTextBoxes[talkingPlayer].audioSpeaker.transform.localPosition = Vector3.zero;
        });
        CL.Info("b");
        playerTextBoxes[talkingPlayer].audioPlayer.AddClip("speakingSFX");
        CL.Info("c");
        switch (type)
        {
            case TextType.Whisper:
                playerTextBoxes[talkingPlayer].audioSpeaker.Volume = 0.2f;
                playerTextBoxes[talkingPlayer].audioSpeaker.MaxDistance = 2f;
                break;
            case TextType.Normal:
                playerTextBoxes[talkingPlayer].audioSpeaker.Volume = 1.0f;
                playerTextBoxes[talkingPlayer].audioSpeaker.MaxDistance = 10f;
                break;
            case TextType.Yelling:
                playerTextBoxes[talkingPlayer].audioSpeaker.Volume = 1.7f;
                playerTextBoxes[talkingPlayer].audioSpeaker.MaxDistance = 20f;
                break;
            default:
                playerTextBoxes[talkingPlayer].audioSpeaker.Volume = 0f;
                playerTextBoxes[talkingPlayer].audioSpeaker.MaxDistance = 0f;
                break;
        }
        CL.Info("d");
    }

    private void ScheduleDestroy(Player talkingPlayer, int messageLength)
    {
        // Scheduling destruction of text toy, needs to check if it's still the same one.

        if (!playerTextBoxes.TryGetValue(talkingPlayer, out var store))
            return;

        TextToy createdToy = store.textToy;
        if (createdToy == null)
            return;

        float delay = Mathf.Clamp(messageLength / 2f, 2f, 7f);
        Timing.CallDelayed(delay, () =>
        {
            if (playerTextBoxes.TryGetValue(talkingPlayer, out var currentStore) && ReferenceEquals(currentStore.textToy, createdToy))
            {
                if (createdToy != null && !createdToy.IsDestroyed)
                    createdToy.Destroy();

                playerTextBoxes.Remove(talkingPlayer);
            }
        });
    }
}

