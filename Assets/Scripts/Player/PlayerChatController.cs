using Fusion;
using TMPro;
using UnityEngine;

namespace Player
{
    public class PlayerChatController : NetworkBehaviour
    {
        public static bool IsTyping { get; private set; }
        public TMP_InputField inputField;
        public Animator bubbleAnimator;
        public TMP_Text bubbleText;
        private static readonly int Open = Animator.StringToHash("Open");

        public override void Spawned()
        {
            var isLocalPlayer = Object.InputAuthority == Runner.LocalPlayer;
            gameObject.SetActive(isLocalPlayer);

            if (isLocalPlayer)
            {
                inputField.onSelect.AddListener(_ => IsTyping = true);
                inputField.onSubmit.AddListener(_ => IsTyping = false);
                inputField.onSubmit.AddListener(OnSubmit);
            }
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