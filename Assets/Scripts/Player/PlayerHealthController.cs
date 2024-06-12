using Fusion;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using utilities;

namespace Player
{
    public class PlayerHealthController : NetworkBehaviour
    {
        public Image fillAmount;
        public TMP_Text healthAmountText;
        public PlayerCameraController playerCameraController;
        public Animator bloodHitAnimation;
        public LayerMask deadGround;

        [Networked(OnChanged = nameof(HealthAmountChange))]
        private int CurrentHpAmount { get; set; }

        private const int maxHealthAmount = 100;
        private PlayerController playerController;
        public BoxCollider2D col;

        public override void Spawned()
        {
            col = GetComponent<BoxCollider2D>();
            playerController = GetComponent<PlayerController>();
            CurrentHpAmount = maxHealthAmount;
        }

        public override void FixedUpdateNetwork()
        {
            //call in sever because we need run one time if call on client it could be called multiple time
            if (Runner.IsServer && playerController.IsAlive)
            {
                var didHit = Runner.GetPhysicsScene2D().OverlapBox(transform.position, col.bounds.size, 0, deadGround);

                if (didHit != default)
                {
                    RPCReducePlayerHealth(maxHealthAmount);
                }
            }
        }

        [Rpc(RpcSources.StateAuthority, RpcTargets.StateAuthority)]
        public void RPCReducePlayerHealth(int damage)
        {
            CurrentHpAmount -= damage;
        }

        public static void HealthAmountChange(Changed<PlayerHealthController> changed)
        {
            var currentHealth = changed.Behaviour.CurrentHpAmount;

            changed.LoadOld();

            var oldHealth = changed.Behaviour.CurrentHpAmount;

            //only if the current health is not the same as the prev one
            if (currentHealth != oldHealth)
            {
                changed.Behaviour.UpdateVisuals(currentHealth);

                //we did not respawn or just spawn
                if (currentHealth != maxHealthAmount)
                {
                    changed.Behaviour.PlayerGotHit(currentHealth);
                }
            }
        }

        private void UpdateVisuals(int healthAmount)
        {
            var num = (float)healthAmount / maxHealthAmount;

            fillAmount.fillAmount = num;
            healthAmountText.SetText($"{healthAmount}/{maxHealthAmount}");
        }

        private void PlayerGotHit(int healthAmount)
        {
            if (Object.IsLocalPlayer())
            {
                Debug.Log("Local player got hit");

                var shank = new Vector3(0.2f, 0.1f);

                playerCameraController.ShankCamera(shank);

                bloodHitAnimation.Play("Hit");
            }

            if (healthAmount <= 0)
            {
                playerController.KillPlayer();
                Debug.Log("Player is dead");
            }
        }

        public void ResetHealth()
        {
            CurrentHpAmount = maxHealthAmount;
        }
    }
}