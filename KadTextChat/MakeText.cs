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

        if (playerTextBoxes[talkingPlayer].textToy != null || !playerTextBoxes[talkingPlayer].textToy.IsDestroyed)
            playerTextBoxes[talkingPlayer].textToy.Destroy();

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
        switch (type)
        {
            case TextType.Whisper:
                CL.Info($"{talkingPlayer.Nickname} whispered:\n{string.Join(" ", message)}");
                return true;
            case TextType.Normal:
                CreateAudioPlayer(talkingPlayer);
                playerTextBoxes[talkingPlayer].audioSpeaker.Volume = 1.0f;
                playerTextBoxes[talkingPlayer].audioSpeaker.MaxDistance = 10f;
                CL.Info($"{talkingPlayer.Nickname} said:\n{string.Join(" ", message)}");
                return true;
            case TextType.Yelling:
                CreateAudioPlayer(talkingPlayer);
                playerTextBoxes[talkingPlayer].audioSpeaker.Volume = 1.5f;
                playerTextBoxes[talkingPlayer].audioSpeaker.MaxDistance = 18f;
                playerTextBoxes[talkingPlayer].textToy.Scale = new Vector3(-.25f, .25f, .25f);
                CL.Info($"{talkingPlayer.Nickname} yelled:\n{string.Join(" ", message)}");
                return true;
            default:
                return false;
        }
        CL.Info("7");
    }

    private void CreateAudioPlayer(Player talkingPlayer)
    {
        playerTextBoxes[talkingPlayer].audioPlayer = AudioPlayer.CreateOrGet("SpeakingAudioPlayer", onIntialCreation: p =>
        {
            p.transform.parent = talkingPlayer.GameObject.transform;
            playerTextBoxes[talkingPlayer].audioSpeaker = p.AddSpeaker("Talking-Speaker", isSpatial: true, maxDistance: 15f);
            playerTextBoxes[talkingPlayer].audioSpeaker.transform.parent = talkingPlayer.GameObject.transform;
            playerTextBoxes[talkingPlayer].audioSpeaker.transform.localPosition = Vector3.zero;
        });

        playerTextBoxes[talkingPlayer].audioPlayer.AddClip("speakingSFX");
    }

    private void ScheduleDestroy(Player talkingPlayer, int messageLength)
    {
        //Scheduling destruction of text toy, needs to check if its still the same one.
        TextToy createdToy = playerTextBoxes[talkingPlayer].textToy;
        float delay = Mathf.Clamp(messageLength / 2f, 2f, 7f);
        Timing.CallDelayed(delay, () =>
        {
            if (playerTextBoxes.TryGetValue(talkingPlayer, out var current) && ReferenceEquals(current, createdToy))
            {
                createdToy.Destroy();
                playerTextBoxes.Remove(talkingPlayer);
            }
        });
    }
}

