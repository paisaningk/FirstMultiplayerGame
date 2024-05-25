using Fusion;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using utilities;

namespace Player
{
    public enum PlayerInputButton
    {
        None,
        Jump
    }

    public class PlayerController : NetworkBehaviour, IBeforeUpdate
    {
        //for input
        public const string horizontalInputName = "Horizontal";

        //https://doc.photonengine.com/fusion/current/manual/data-transfer/networked-properties
        //การบอกให้ sever ว่าตัวแปรพวกนี้ต้อง sync นะ
        [Networked] private NetworkButtons ButtonsPrev { get; set; }

        [Networked(OnChanged = nameof(OnNameChange))]
        public NetworkString<_8> PlayerName { get; set; }

        [Header("LocalObject")]
        public GameObject camGameObject;

        [Header("Input And Movement")] [Space]
        public TMP_Text playerNameText;

        [Header("Input And Movement")] [Space]
        public float moveSpeed = 6;
        public float jumpForce = 1000;
        public float horizontal;
        public new Rigidbody2D rigidbody;
        public PlayerWeaponController playerWeaponController;

        public void OnValidate()
        {
            rigidbody = GetComponent<Rigidbody2D>();
            playerWeaponController = GetComponent<PlayerWeaponController>();
        }

        public override void Spawned()
        {
            SetLocalObject();
        }

        // this function call where player name have chang
        // for example
        // if set name "adc" on spawn and on fixed update network set name "banana" will change name
        private static void OnNameChange(Changed<PlayerController> changed)
        {
            changed.Behaviour.SetPlayerName(changed.Behaviour.PlayerName);
        }

        private void SetPlayerName(NetworkString<_8> playerName)
        {
            playerNameText.text = playerName + " " + Object.InputAuthority.PlayerId;
        }

        // Send RPC to the Host form Client
        // Sources define which Peer can send the Rpc 
        // Targets define on which it ti executed
        // this function player send rpc to sever 
        // StateAuthority = sever 
        [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
        private void RpcSetNickName(NetworkString<_8> playerName)
        {
            PlayerName = playerName;
        }

        private void SetLocalObject()
        {
            if (GlobalManager.Instance.IsLocalPlayer(Object))
            {
                camGameObject.SetActive(true);

                var playerName = GlobalManager.Instance.NetworkRunnerController.LocalPlayerName;

                RpcSetNickName(playerName);
            }
        }

        //Happens before anything else in fusion does , network application , etc
        //called at start of Fusion update loop . before the Fusion simulation loop.
        //It call before fusion does any work , ever screen refresh.
        public void BeforeUpdate()
        {
            //check is we are local player
            if (GlobalManager.Instance.IsLocalPlayer(Object))
            {
                horizontal = Input.GetAxisRaw(horizontalInputName);
            }
        }

        public override void FixedUpdateNetwork()
        {
            // will return false if 
            // the clinet does not have state authority or input authority
            // the requested type of input data does not exist in the simulation
            if (Runner.TryGetInputForPlayer(Object.InputAuthority, out PlayerData input))
            {
                rigidbody.velocity = new Vector2(input.horizontalInput * moveSpeed, rigidbody.velocity.y);
                CheckJumpInput(input);
            }
        }

        public void CheckJumpInput(PlayerData input)
        {
            // compare = เปรียบเทียบ https://doc.photonengine.com/fusion/current/manual/data-transfer/player-input
            // compare the current state of the buttons with previous state to evaluate
            // whether the buttons have just been pressed or released
            var networkButtons = input.networkButtons.GetPressed(ButtonsPrev);

            // if current state of the buttons not equal to previous state of the buttons , we add force to player 
            if (networkButtons.WasPressed(ButtonsPrev, PlayerInputButton.Jump))
            {
                rigidbody.AddForce(Vector2.up * jumpForce, ForceMode2D.Force);
            }

            ButtonsPrev = input.networkButtons;
        }

        public PlayerData GetPlayerNetworkInput()
        {
            var data = new PlayerData
            {
                horizontalInput = horizontal
            };

            data.networkButtons.Set(PlayerInputButton.Jump, Input.GetKey(KeyCode.Space));
            data.gunPivotRotation = playerWeaponController.localQuaternionPivotRot;

            return data;
        }
    }
}