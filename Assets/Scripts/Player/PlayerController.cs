using Fusion;
using Lobby;
using TMPro;
using UnityEngine;
using utilities;

namespace Player
{
    public class PlayerController : NetworkBehaviour, IBeforeUpdate
    {
        //for input
        private const string horizontalInputName = "Horizontal";

        //https://doc.photonengine.com/fusion/current/manual/data-transfer/networked-properties
        //การบอกให้ sever ว่าตัวแปรพวกนี้ต้อง sync นะแล้วก็เก็บเป็นของใครของมันด้วย
        [Networked] private NetworkButtons ButtonsPrev { get; set; }
        [Networked] private Vector2 SeverNextSpawnPoint { get; set; }
        [Networked] public TickTimer RespawnTimeTimer { get; private set; }

        [Networked(OnChanged = nameof(OnNameChange))]
        public NetworkString<_8> PlayerName { get; set; }

        [Networked] public NetworkBool IsAlive { get; private set; }

        [Header("LocalObject")]
        public GameObject camGameObject;

        [Header("Text")] [Space]
        public TMP_Text playerNameText;

        [Header("RespawnTime")] [Space]
        public float respawnTime = 5;

        [Header("Input And Movement")] [Space]
        public float moveSpeed = 6;
        public float jumpForce = 1000;
        public float horizontal;
        public bool CanUseInput => IsAlive && !GameManager.MatchIsOver;
        public new Rigidbody2D rigidbody;
        public PlayerWeaponController playerWeaponController;
        public PlayerVisualController playerVisualController;
        public PlayerHealthController playerHealthController;

        public void OnValidate()
        {
            rigidbody = GetComponent<Rigidbody2D>();
            playerWeaponController = GetComponent<PlayerWeaponController>();
            playerVisualController = GetComponent<PlayerVisualController>();
            playerHealthController = GetComponent<PlayerHealthController>();
        }

        public override void Spawned()
        {
            SetLocalObject();

            IsAlive = true;
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
            if (Runner.LocalPlayer == Object.HasInputAuthority)
            {
                camGameObject.SetActive(true);

                var playerName = GlobalManager.Instance.NetworkRunnerController.LocalPlayerName;

                RpcSetNickName(playerName);
            }
            else
            {
                //Snapshots = get verify form sever
                //if this is not proxy aka inputAuthority player
                //we want to make to set this NRB2D InterpolationDataSources to Snapshots
                //as it will automatically set NRB2D to be predicted regardless if it's proxy or not 
                //as we are doing fully physics prediction
                //and setting it back to Snapshots for proxies will also make that lag compensation will work properly + be more cost efficient
                GetComponent<NetworkRigidbody2D>().InterpolationDataSource = InterpolationDataSources.Snapshots;
            }
        }

        //Happens before anything else in fusion does , network application , etc
        //called at start of Fusion update loop . before the Fusion simulation loop.
        //It call before fusion does any work , ever screen refresh.
        public void BeforeUpdate()
        {
            //check is we are local player
            if (Runner.LocalPlayer == Object.HasInputAuthority && CanUseInput)
            {
                horizontal = Input.GetAxisRaw(horizontalInputName);
            }
        }

        public override void FixedUpdateNetwork()
        {
            //check is player should respawn
            CheckRespawnTime();


            // will return false if 
            // the clinet does not have state authority or input authority
            // the requested type of input data does not exist in the simulation
            if (CanUseInput && Runner.TryGetInputForPlayer(Object.InputAuthority, out PlayerData input))
            {
                rigidbody.velocity = new Vector2(input.horizontalInput * moveSpeed, rigidbody.velocity.y);
                CheckJumpInput(input);
            }

            playerVisualController.UpdateScalePlayer(rigidbody.velocity);
        }

        public override void Render()
        {
            playerVisualController.RenderVisual(rigidbody.velocity, playerWeaponController.IsHoldingShootingKey);
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
            data.networkButtons.Set(PlayerInputButton.Shoot, Input.GetKey(KeyCode.Mouse0));
            data.gunPivotRotation = playerWeaponController.localQuaternionPivotRot;

            return data;
        }

        public void KillPlayer()
        {
            if (Runner.IsServer)
            {
                SeverNextSpawnPoint = GlobalManager.Instance.PlayerSpawnController.GetRandomSpawnPoint();
            }

            IsAlive = false;
            rigidbody.simulated = false;
            playerVisualController.TriggerDieAnimation();

            RespawnTimeTimer = TickTimer.CreateFromSeconds(Runner, respawnTime);
        }

        private void CheckRespawnTime()
        {
            if (IsAlive) return;

            if (RespawnTimeTimer.Expired(Runner))
            {
                RespawnTimeTimer = TickTimer.None;
                RespawnPlayer();
            }
        }

        public override void Despawned(NetworkRunner runner, bool hasState)
        {
            Destroy(gameObject);

            GlobalManager.Instance.ObjectPoolingManager.RemoveObjectFromDictionary(Object);
        }

        public void RespawnPlayer()
        {
            IsAlive = true;
            rigidbody.simulated = true;
            rigidbody.position = SeverNextSpawnPoint;
            playerVisualController.TriggerRespawnAnimation();
            playerHealthController.ResetHealth();
        }
    }
}