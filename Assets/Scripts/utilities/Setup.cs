using System;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace utilities
{
    public class Setup : MonoBehaviour
    {
        [Obsolete("Obsolete")]
        public void Start()
        {
            var any = SceneManager.GetAllScenes().Any(T => T.name == "Lobby");

            if (any)
            {
                return;
            }

            SceneManager.LoadScene("Lobby", LoadSceneMode.Additive);
        }
    }
}