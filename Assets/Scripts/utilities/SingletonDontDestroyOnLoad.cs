﻿using UnityEngine;

namespace utilities
{
    public class SingletonDontDestroyOnLoad<T> : MonoBehaviour where T : MonoBehaviour
    {
        // Check to see if we're about to be destroyed.
        private static bool m_ShuttingDown = false;
        private static object m_Lock = new object();
        private static T m_Instance;

        /// <summary>
        /// Access singleton instance through this propriety.
        /// </summary>
        public static T Instance
        {
            get
            {
                if (m_ShuttingDown)
                    return null;

                lock (m_Lock)
                {
                    if (m_Instance == null)
                    {
                        // Search for existing instance.
                        m_Instance = (T)FindObjectOfType(typeof(T));

                        // Create new instance if one doesn't already exist.
                        if (m_Instance == null)
                        {
                            // Need to create a new GameObject to attach the singleton to.
                            var singletonObject = new GameObject();
                            m_Instance = singletonObject.AddComponent<T>();
                            singletonObject.name = typeof(T).ToString() + " (Singleton)";

                            // Make instance persistent.
                            DontDestroyOnLoad(singletonObject);
                        }
                    }

                    return m_Instance;
                }
            }
        }

        public virtual void Awake()
        {
            if (m_Instance != null && m_Instance != this)
            {
                Destroy(this.gameObject);
            }

            m_Instance = GetComponent<T>();
            DontDestroyOnLoad(gameObject);
        }

        private void OnApplicationQuit()
        {
            m_ShuttingDown = true;
        }

        private void OnDestroy()
        {
            m_ShuttingDown = true;
        }

        private void Log(object message)
        {
            Debug.Log($"[{gameObject.name}] {message}");
        }

        private void LogWarning(object message)
        {
            Debug.LogWarning($"[{gameObject.name}] {message}");
        }

        private void LogError(object message)
        {
            Debug.LogError($"[{gameObject.name}] {message}");
        }
    }
}