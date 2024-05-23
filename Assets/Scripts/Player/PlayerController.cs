using Fusion;
using UnityEngine;

namespace Player
{
    public class PlayerController : NetworkBehaviour, IBeforeUpdate
    {
        //for input
        public const string horizontalInputName = "Horizontal";

        public float moveSpeed = 6;
        public float horzontal;
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
                horzontal = Input.GetAxisRaw(horizontalInputName);
            }
        }

        public override void FixedUpdateNetwork()
        {
            // will return false if 
            // the clinet does not have state authority or input authority
            // the requested type of input data does not exist in the simulation
            if (Runner.TryGetInputForPlayer(Object.InputAuthority, out PlayerData input))
            {
                rigidbody.velocity = new Vector2(input.horzontal * moveSpeed, rigidbody.velocity.y);
            }
        }

        public PlayerData GetPlayerNetworkInput()
        {
            var data = new PlayerData
            {
                horzontal = horzontal
            };

            return data;
        }
    }
}