using Fusion;
using Player;
using TMPro;
using UnityEngine;

namespace UserInterface
{
    // SimulationBehaviour every like networkBehaviour but can't use [networked]
    public class RespawnPanel : SimulationBehaviour
    {
        public PlayerController playerController;
        public TMP_Text respawnAmounText;
        public GameObject childObj;

        public override void FixedUpdateNetwork()
        {
            if (playerController.Object.HasInputAuthority == Runner.LocalPlayer.IsValid)
            {
                var respawnTimeTimer = playerController.RespawnTimeTimer;
                var isRunning = respawnTimeTimer.IsRunning;

                childObj.SetActive(isRunning);

                if (isRunning && respawnTimeTimer.RemainingTime(Runner).HasValue)
                {
                    var time = Mathf.RoundToInt(respawnTimeTimer.RemainingTime(Runner).Value);
                    respawnAmounText.SetText(time.ToString());
                }
            }
        }
    }
}