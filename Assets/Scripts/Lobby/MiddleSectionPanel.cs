using Fusion;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using utilities;

namespace Lobby
{
    public class MiddleSectionPanel : LobbyPanelBase
    {
        [Header("Button")]
        public Button joinRandomRoomBtn;
        public Button joinRoomByArgBtn;
        public Button createRoomBtn;

        public TMP_InputField joinRoomByArgInputField;
        public TMP_InputField createRoomInputField;

        private NetworkRunnerController networkRunnerController;
        
        public void OnValidate()
        {
            lobbyPanelType = LobbyPanelType.MiddleSectionPanel;
        }

        public override void InitPanel(LobbyUIManager manager)
        {
            base.InitPanel(manager);

            networkRunnerController = GlobalManager.Instance.NetworkRunnerController;
            joinRandomRoomBtn.onClick.AddListener(JoinRandomRoom);
            joinRoomByArgBtn.onClick.AddListener(() => {CreateRoom(GameMode.Client, joinRoomByArgInputField.text);});
            createRoomBtn.onClick.AddListener(() => {CreateRoom(GameMode.Host, createRoomInputField.text);});
        }

        private void CreateRoom(GameMode gameMode, string field)
        {
            if (field.Length < GlobalManager.maxCharForName) return;
            
            Debug.Log($"--------------------{gameMode}--------------------");
            networkRunnerController.StartGame(gameMode, field);
        }

        private void JoinRandomRoom()
        {
            Debug.Log($"--------------------Join-Random-Room--------------------");
            networkRunnerController.StartGame(GameMode.AutoHostOrClient, string.Empty);
        }
    }
}