using Sirenix.OdinInspector;
using UnityEngine;
using utilities;

namespace Lobby
{
    public enum LobbyPanelType
    {
        None,
        CreateNickNamePanel,
        MiddleSectionPanel
    }
    public class LobbyPanelBase : MonoBehaviour
    {
        [Header("Base Data"), SerializeField]
        public LobbyPanelType lobbyPanelType;
        public Animator panelAnimator;
        [ReadOnly] protected LobbyUIManager lobbyUIManager;

        public virtual void InitPanel(LobbyUIManager manager)
        {
            lobbyUIManager = manager;
        }

        public virtual void ShowPanel()
        {
            gameObject.SetActive(true);
            panelAnimator.Play("In");
        }
        
        public virtual void ClosePanel()
        {
            panelAnimator.Play("Out");
            StartCoroutine(gameObject.CloseGameObjectWhenAnimEnd(panelAnimator , false));
        }
    }
}
