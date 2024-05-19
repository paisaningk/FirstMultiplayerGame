using UnityEngine;

namespace utilities
{
    public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        public static T Instance { get; private set; }

        public virtual void Awake()
        {
            if (Instance == null)
            {
                Instance = GetComponent<T>();
            }
        }

        public virtual void OnDestroy()
        {
            Instance = null;
        }
    }
}