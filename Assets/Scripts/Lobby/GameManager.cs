using Fusion;
using UnityEngine;

namespace Lobby
{
    public class GameManager : NetworkBehaviour
    {
        public new Camera camera;

        public override void Spawned()
        {
            camera.gameObject.SetActive(false);
        }
    }
}