using Fusion;
using Lobby;
using ObjectInGame;
using Spawn;
using UnityEngine;

namespace utilities
{
    public class GlobalManager : SingletonDontDestroyOnLoad<GlobalManager>
    {
        [field: SerializeField] public NetworkRunnerController NetworkRunnerController { get; private set; }
        public const int maxCharForName = 2;
        public PlayerSpawnController PlayerSpawnController { get; set; }
        public ObjectPoolingManager ObjectPoolingManager { get; set; }

        public GameManager gameManager;

        public bool IsLocalPlayer(NetworkObject networkObject)
        {
            return networkObject.IsLocalPlayer();
        }
    }
}