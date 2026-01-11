using LabApiExtensions.FakeExtension;
using MEC;
using UnityEngine;

namespace KadTextChat
{
    public class TextComponent : MonoBehaviour
    {
        //Component that makes the text face towards other players
        public Player hostPlayer;
        private Transform textTransform;
        public TextToy textToy;
        public Vector3 scale;

        public void Awake()
        {   
            if (scale != Vector3.zero)
            {
                transform.localScale = scale;
            }

            textTransform = transform;
        }

        public void Update()
        {
            if (textToy.IsDestroyed)
            {
                Destroy(this);
                return;
            }

            foreach (Player player in Player.ReadyList.Where(p => p != hostPlayer))
            {
                if (Vector3.Distance(textTransform.position, player.Position) > 20)
                {
                    player.SendFakeSyncVar(textToy.Base, 4, Vector3.zero);
                    continue;
                }

                player.SendFakeSyncVar(textToy.Base, 4, scale);
                FaceTowardsPlayer(player, textTransform, textToy);
            }

        }

        public void FaceTowardsPlayer(Player observer, Transform transform, TextToy textToy)
        {
            Vector3 direction = observer.Position - transform.position;
            direction.y = 0;
            Quaternion rotation = Quaternion.LookRotation(-direction);
            textTransform.rotation = rotation;

            observer.SendFakeSyncVar(textToy.Base, 2, transform.localRotation);
        }
    }
}
