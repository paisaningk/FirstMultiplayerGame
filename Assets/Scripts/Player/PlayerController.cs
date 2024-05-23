using Fusion;
using UnityEngine;
using UnityEngine.Serialization;

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
        [Networked] public NetworkButtons buttonsPrev { get; set; }

        public float moveSpeed = 6;
        public float jumpForce = 1000;
        public float horizontal;
        public new Rigidbody2D rigidbody;

        public void OnValidate()
        {
            rigidbody = GetComponent<Rigidbody2D>();
        }

        //Happens before anything else in fusion does , network application , etc
        //called at start of Fusion update loop . before the Fusion simulation loop.
        //It call before fusion does any work , ever screen refresh.
        public void BeforeUpdate()
        {
            //check is we are local player
            if (Runner.LocalPlayer == Object.HasInputAuthority)
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
            var networkButtons = input.networkButtons.GetPressed(buttonsPrev);

            // if current state of the buttons not equal to previous state of the buttons , we add force to player 
            if (networkButtons.WasPressed(buttonsPrev, PlayerInputButton.Jump))
            {
                rigidbody.AddForce(Vector2.up * jumpForce, ForceMode2D.Force);
            }

            buttonsPrev = input.networkButtons;
        }

        public PlayerData GetPlayerNetworkInput()
        {
            var data = new PlayerData
            {
                horizontalInput = horizontal
            };

            data.networkButtons.Set(PlayerInputButton.Jump, Input.GetKey(KeyCode.Space));

            return data;
        }
    }
}