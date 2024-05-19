using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace utilities
{
    public class Setup : MonoBehaviour
    {
        public void Start()
        {
            SceneManager.LoadScene("Lobby");
        }
    }
}