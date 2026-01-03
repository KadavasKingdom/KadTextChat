using MEC;
using UnityEngine;

namespace KadTextChat;

public class MakeText
{
    private Dictionary<Player, TextToy> playerTextBoxes = new();
    private AudioPlayer talkerAudioPlayer;
    private Speaker audioSpeaker;
    public enum TextType
    {
        Normal,
        Whisper,
        Yelling
    }

    public bool CreateTextBox(Player talkingPlayer, string message, TextType type)
    {
        int messageLength = message.Length;

        if (!playerTextBoxes.ContainsKey(talkingPlayer))
            playerTextBoxes.Add(talkingPlayer, null);

        if (playerTextBoxes[talkingPlayer] != null)
        {
            playerTextBoxes[talkingPlayer].Destroy();
        }

        playerTextBoxes[talkingPlayer] = TextToy.Create(
            position: new Vector3(0f, talkingPlayer.GameObject.transform.localScale.y + 0.3f, 0f),
            parent: talkingPlayer.GameObject.transform,
            rotation: new Quaternion(0, 0, 0, 0),
            scale: new Vector3(-.14f, .14f, .14f));

        playerTextBoxes[talkingPlayer].GameObject.name = $"{message}";
        playerTextBoxes[talkingPlayer].TextFormat = message;

        playerTextBoxes[talkingPlayer].Spawn();


        //Scheduling destruction of text toy, needs to check if its still the same one.
        var createdToy = playerTextBoxes[talkingPlayer];
        float delay = Mathf.Clamp(messageLength / 2f, 2f, 7f);
        Timing.CallDelayed(delay, () =>
        {
            if (playerTextBoxes.TryGetValue(talkingPlayer, out var current) && ReferenceEquals(current, createdToy))
            {
                createdToy.Destroy();
                playerTextBoxes.Remove(talkingPlayer);
            }
        });

        if(type == TextType.Whisper)
        {
            CL.Info($"{talkingPlayer.Nickname} whispered:\n{string.Join(" ", message)}");
            return true;
        }

        talkerAudioPlayer = AudioPlayer.CreateOrGet("SpeakingAudioPlayer", onIntialCreation: p =>
        {
            p.transform.parent = talkingPlayer.GameObject.transform;
            audioSpeaker = p.AddSpeaker("Talking-Speaker", isSpatial: true, maxDistance: 15f);
            audioSpeaker.transform.parent = talkingPlayer.GameObject.transform;
            audioSpeaker.transform.localPosition = Vector3.zero;
        });

        talkerAudioPlayer.AddClip("speakingSFX");

        if (type == TextType.Normal)
        {
            audioSpeaker.Volume = 1.0f;
            audioSpeaker.MaxDistance = 10f;
            CL.Info($"{talkingPlayer.Nickname} said:\n{string.Join(" ", message)}");
            return true;
        }

        if (type == TextType.Yelling)
        {
            audioSpeaker.Volume = 1.5f;
            audioSpeaker.MaxDistance = 18f;
            playerTextBoxes[talkingPlayer].Scale = new Vector3(-.25f, .25f, .25f);
            CL.Info($"{talkingPlayer.Nickname} yelled:\n{string.Join(" ", message)}");
            return true;
        }

    }

}

