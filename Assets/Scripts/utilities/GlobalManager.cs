using Fusion;
using UnityEngine;

namespace utilities
{
    public class GlobalManager : SingletonDontDestroyOnLoad<GlobalManager>
    {
        [field: SerializeField] public NetworkRunnerController NetworkRunnerController { get; private set; }
        public const int maxCharForName = 2;

        public bool IsLocalPlayer(NetworkObject networkObject)
        {
            return NetworkRunnerController.networkRunnerInstance.LocalPlayer == networkObject.HasInputAuthority;
        }
    }
}