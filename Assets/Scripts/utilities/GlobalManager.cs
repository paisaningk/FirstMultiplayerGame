using UnityEngine;

namespace utilities
{
    public class GlobalManager : SingletonDontDestroyOnLoad<GlobalManager>
    {
        [field: SerializeField] public NetworkRunnerController NetworkRunnerController { get; private set; }
        public const int maxCharForName = 2;
    }
}