using Fusion;
using UnityEngine;
using utilities;

namespace Player
{
    public class PlayerWeaponController : NetworkBehaviour, IBeforeUpdate
    {
        [field: SerializeField] public Quaternion localQuaternionPivotRot { get; private set; }
        public float delayBetweenShots = 0.1f;
        public ParticleSystem muzzleEffect;
        public Camera localCam;
        public Transform pivotToRotate;

        [Header("Bullet")]
        public Transform fireSpawnPoint;
        public NetworkPrefabRef bulletPrefabRef = NetworkPrefabRef.Empty;

        [Networked(OnChanged = nameof(OnMuzzleEffectStateChange))]
        private NetworkBool PlayerMuzzleEffect { get; set; }

        [Networked]
        public NetworkBool IsHoldingShootingKey { get; private set; }

        [Networked] private Quaternion CurrentPlayerPivotRotation { get; set; }
        [Networked] private NetworkButtons ButtonsPrev { get; set; }
        [Networked] private TickTimer ShootCoolDown { get; set; }

        private PlayerController playerController;

        public override void Spawned()
        {
            playerController = GetComponent<PlayerController>();
        }

        public void BeforeUpdate()
        {
            if (playerController.CanUseInput && Object.IsLocalPlayer())
            {
                var dir = localCam.ScreenToWorldPoint(Input.mousePosition) - transform.position;
                var angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

                localQuaternionPivotRot = Quaternion.AngleAxis(angle, Vector3.forward);
            }
        }

        public override void FixedUpdateNetwork()
        {
            // tell Current Player Pivot Rotation to host
            if (Runner.TryGetInputForPlayer(Object.InputAuthority, out PlayerData input))
            {
                if (playerController.CanUseInput)
                {
                    CheckShootInput(input);
                    CurrentPlayerPivotRotation = input.gunPivotRotation;

                    ButtonsPrev = input.networkButtons;
                }
                else
                {
                    IsHoldingShootingKey = false;
                    PlayerMuzzleEffect = false;
                    ButtonsPrev = default;
                }
            }

            pivotToRotate.rotation = CurrentPlayerPivotRotation;
        }

        private void CheckShootInput(PlayerData input)
        {
            var currentButton = input.networkButtons.GetPressed(ButtonsPrev);

            IsHoldingShootingKey = currentButton.WasReleased(ButtonsPrev, PlayerInputButton.Shoot);

            if (currentButton.WasReleased(ButtonsPrev, PlayerInputButton.Shoot) &&
                ShootCoolDown.ExpiredOrNotRunning(Runner))
            {
                PlayerMuzzleEffect = true;

                ShootCoolDown = TickTimer.CreateFromSeconds(Runner, delayBetweenShots);

                Runner.Spawn(bulletPrefabRef, fireSpawnPoint.position, fireSpawnPoint.rotation, Object.InputAuthority);
            }
            else
            {
                PlayerMuzzleEffect = false;
            }
        }

        private static void OnMuzzleEffectStateChange(Changed<PlayerWeaponController> changed)
        {
            var currentState = changed.Behaviour.PlayerMuzzleEffect;

            changed.LoadOld();

            var oldState = changed.Behaviour.PlayerMuzzleEffect;

            if (oldState != currentState)
            {
                changed.Behaviour.SetMuzzleEffect(currentState);
            }
        }

        private void SetMuzzleEffect(bool play)
        {
            if (play)
            {
                muzzleEffect.Play();
            }
            else
            {
                muzzleEffect.Stop();
            }
        }
    }
}