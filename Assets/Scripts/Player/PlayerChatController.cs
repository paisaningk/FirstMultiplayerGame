using Fusion;
using TMPro;
using UnityEngine;
using utilities;

namespace Player
{
    public class PlayerChatController : NetworkBehaviour
    {
        [Networked] public bool IsTyping { get; private set; }
        public TMP_InputField inputField;
        public Animator bubbleAnimator;
        public TMP_Text bubbleText;
        private static readonly int Open = Animator.StringToHash("Open");

        public override void Spawned()
        {
            var isLocalPlayer = Object.IsLocalPlayer();
            gameObject.SetActive(isLocalPlayer);

            if (isLocalPlayer)
            {
                inputField.onSelect.AddListener(_ => RPCUpdateIsTyping(true));
                inputField.onSubmit.AddListener(_ => RPCUpdateIsTyping(false));
                inputField.onSubmit.AddListener(OnSubmit);
            }
        }

        [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
        public void RPCUpdateIsTyping(bool tying)
        {
            IsTyping = tying;
        }

        private void OnSubmit(string s)
        {
            if (!string.IsNullOrEmpty(s))
            {
                Debug.Log(s);
                RPCSetBubbleSpeech(s);
            }
        }

        [Rpc(RpcSources.InputAuthority, RpcTargets.All)]
        private void RPCSetBubbleSpeech(NetworkString<_64> text)
        {
            bubbleText.SetText(text.Value);
            bubbleAnimator.SetTrigger(Open);
        }
    }
}