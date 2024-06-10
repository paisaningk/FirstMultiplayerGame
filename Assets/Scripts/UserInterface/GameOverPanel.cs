using System;
using UnityEngine;
using UnityEngine.UI;
using utilities;

namespace UserInterface
{
    public class GameOverPanel : MonoBehaviour
    {
        public Button returnLobbyButton;
        public GameObject childObj;

        public void Start()
        {
            GlobalManager.Instance.gameManager.OnGameIsOver += GameManagerOnOnGameIsOver;

            returnLobbyButton.onClick.AddListener(() =>
                GlobalManager.Instance.NetworkRunnerController.ShutDownRunner());
        }

        private void GameManagerOnOnGameIsOver()
        {
            childObj.SetActive(true);
        }

        private void OnDestroy()
        {
            GlobalManager.Instance.gameManager.OnGameIsOver -= GameManagerOnOnGameIsOver;
        }
    }
}