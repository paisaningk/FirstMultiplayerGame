using TMPro;
using UnityEngine;
using UnityEngine.UI;
using utilities;

namespace Lobby
{
    public class CreateNickNamePanel : LobbyPanelBase
    {
        [Header("Create Nick Name Data")]
        public TMP_InputField inputField;
        public Button crateNickNameButton;
        private const int maxCharForName = GlobalManager.maxCharForName;

        public void OnValidate()
        {
            lobbyPanelType = LobbyPanelType.CreateNickNamePanel;
        }

        public override void InitPanel(LobbyUIManager manager)
        {
            base.InitPanel(manager);

            crateNickNameButton.interactable = false;
            crateNickNameButton.onClick.AddListener(CreateNickName);
            inputField.onValueChanged.AddListener(OnInputValueChange);
        }

        private void OnInputValueChange(string s)
        {
            crateNickNameButton.interactable = s.Length >= maxCharForName;
        }

        private void CreateNickName()
        {
            var nickName = inputField.text;

            if (nickName.Length >= maxCharForName)
            {
                lobbyUIManager.ShowPenal(LobbyPanelType.MiddleSectionPanel);

                GlobalManager.Instance.NetworkRunnerController.SetLocalPlayerNickName(nickName);
            }
        }
    }
}