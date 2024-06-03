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

        [Networked(OnChanged = nameof(HealthAmountChange))]
        private int CurrentHpAmount { get; set; }

        private const int maxHealthAmount = 100;

        public override void Spawned()
        {
            CurrentHpAmount = maxHealthAmount;
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
            if (GlobalManager.Instance.IsLocalPlayer(Object))
            {
                Debug.Log("Local player got hit");
            }

            if (healthAmount <= 0)
            {
                Debug.Log("Player is dead");
            }
        }
    }
}