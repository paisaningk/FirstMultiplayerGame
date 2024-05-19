using System;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Lobby
{
    public class LobbyUIManager : MonoBehaviour
    {
        public LobbyPanelBase[] lobbyPanelBaseList;
        public LobbyPanelType currentPanelType;

        private void Start()
        {
            foreach (var lobbyPanelBase in lobbyPanelBaseList)
            {
                lobbyPanelBase.InitPanel(this);
            }
        }

        [Button]
        public void ShowPenal(LobbyPanelType lobbyPanelType)
        {
            currentPanelType = lobbyPanelType;
            foreach (var panelBase in lobbyPanelBaseList)
            {
                if (panelBase.lobbyPanelType == lobbyPanelType)
                {
                    panelBase.ShowPanel();
                }
                else
                {
                    panelBase.ClosePanel();
                }
            }
        }
    }
}