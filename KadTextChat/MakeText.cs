using LabApi.Features.Wrappers;
using MEC;
using System.Text.RegularExpressions;
using UnityEngine;

namespace KadTextChat;

public class MakeText
{
    public Dictionary<Player, TextToyInfoStore> playerTextBoxes = new Dictionary<Player, TextToyInfoStore>();

    private static readonly Regex NoParseRegex = new("/noparse", RegexOptions.IgnoreCase | RegexOptions.Compiled);

    public enum TextType
    {
        Normal,
        Whisper,
        Yelling
    }

    public bool CreateTextBox(Player talkingPlayer, string message, TextType type)
    {
        if (!playerTextBoxes.ContainsKey(talkingPlayer))
            playerTextBoxes.Add(talkingPlayer, new TextToyInfoStore());

        TextToy toy = playerTextBoxes[talkingPlayer].textToy;
        if (toy != null && !toy.IsDestroyed)
            toy.Destroy();

        playerTextBoxes[talkingPlayer].textToy = TextToy.Create(
            //Accounts for smaller player models
            position: new Vector3(0f, talkingPlayer.GameObject.transform.localScale.y + 0.15f, 0f),
            parent: talkingPlayer.GameObject.transform,
            rotation: new Quaternion(0, 0, 0, 0),
            //Assigning here so there isn't a bad pop-in
            scale: new Vector3(.05f, .05f, .05f));

        message = $"<noparse>{NoParseRegex.Replace(message.Replace(@"\", @"\\"), "").Replace("<>", "")}</noparse>";

        playerTextBoxes[talkingPlayer].textToy.GameObject.name = $"💬 {message}";
        playerTextBoxes[talkingPlayer].textToy.TextFormat = $"💬 {message}";

        CreateAudioPlayer(talkingPlayer, type);

        switch (type)
        {
            case TextType.Whisper:
                playerTextBoxes[talkingPlayer].audioSpeaker.Volume = 0.2f;
                playerTextBoxes[talkingPlayer].audioSpeaker.MaxDistance = 2f;
                playerTextBoxes[talkingPlayer].textToy.Scale = new Vector3(.08f, .08f, .08f);
                break;
            case TextType.Normal:
                playerTextBoxes[talkingPlayer].audioSpeaker.Volume = 1.0f;
                playerTextBoxes[talkingPlayer].audioSpeaker.MaxDistance = 10f;
                playerTextBoxes[talkingPlayer].textToy.Scale = new Vector3(.12f, .12f, .12f);
                break;
            case TextType.Yelling:
                playerTextBoxes[talkingPlayer].audioSpeaker.Volume = 1.7f;
                playerTextBoxes[talkingPlayer].audioSpeaker.MaxDistance = 20f;
                playerTextBoxes[talkingPlayer].textToy.Scale = new Vector3(.17f, .17f, .17f);
                break;
            default:
                playerTextBoxes[talkingPlayer].audioSpeaker.Volume = 1.0f;
                playerTextBoxes[talkingPlayer].audioSpeaker.MaxDistance = 10f;
                playerTextBoxes[talkingPlayer].textToy.Scale = new Vector3(.15f, .15f, .15f);
                CL.Info("Unassigned text type case!!! Fix me!!");
                break;
        }

        playerTextBoxes[talkingPlayer].textToy.Spawn();

        //Creating component - used to make text face players
        var comp = playerTextBoxes[talkingPlayer].textToy.GameObject.AddComponent<TextComponent>();
        comp.scale = playerTextBoxes[talkingPlayer].textToy.Scale;
        comp.transform.localScale = playerTextBoxes[talkingPlayer].textToy.Scale;
        comp.hostPlayer = talkingPlayer;
        comp.textToy = playerTextBoxes[talkingPlayer].textToy;

        playerTextBoxes[talkingPlayer].textComponent = comp;

        playerTextBoxes[talkingPlayer].recentMessages += 1;
        CooldownReset(talkingPlayer);

        ScheduleDestroy(talkingPlayer, message.Length);
        CL.Info($"{talkingPlayer.Nickname} {type}:\n{message}");

        foreach(Player observer in Player.ReadyList.Where(p => p != talkingPlayer && p.IsAlive))
        {
            if (Vector3.Distance(talkingPlayer.Position, observer.Position) <= playerTextBoxes[talkingPlayer].audioSpeaker.MaxDistance)
                observer.SendConsoleMessage($"<color=yellow>{talkingPlayer.Nickname} {type}:</color> {message}", "yellow");
        }

        return true;
    }

    private void CreateAudioPlayer(Player talkingPlayer, TextType type)
    {
        playerTextBoxes[talkingPlayer].audioPlayer = AudioPlayer.CreateOrGet($"SpeakingAudioPlayer{talkingPlayer.PlayerId}", onIntialCreation: p =>
        {
            p.transform.parent = talkingPlayer.GameObject.transform;
            playerTextBoxes[talkingPlayer].audioSpeaker = p.AddSpeaker($"Talking-Speaker{talkingPlayer.PlayerId}", isSpatial: true);
            playerTextBoxes[talkingPlayer].audioSpeaker.transform.parent = talkingPlayer.GameObject.transform;
            playerTextBoxes[talkingPlayer].audioSpeaker.transform.localPosition = Vector3.zero;
        });

        switch (talkingPlayer.Role)
        {
            case PlayerRoles.RoleTypeId.Scp049:
                playerTextBoxes[talkingPlayer].audioPlayer.AddClip("speaking049SFX");
                break;
            case PlayerRoles.RoleTypeId.Scp0492:
                playerTextBoxes[talkingPlayer].audioPlayer.AddClip("speaking0492SFX");
                break;
            default:
                playerTextBoxes[talkingPlayer].audioPlayer.AddClip("speakingSFX");
                break;
        }

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

                playerTextBoxes[talkingPlayer].textToy = null;
            }
        });
    }

    private void CooldownReset(Player talkingPlayer)
    {
        Player player = talkingPlayer;

        Timing.CallDelayed(6f, () =>
        {
            if (playerTextBoxes.TryGetValue(player, out var currentStore))
            {
                playerTextBoxes[player].recentMessages -= 1;
            }
        });
    }
}

