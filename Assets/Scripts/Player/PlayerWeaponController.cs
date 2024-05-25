using Fusion;
using UnityEngine;
using utilities;

namespace Player
{
    public class PlayerWeaponController : NetworkBehaviour, IBeforeUpdate
    {
        [field: SerializeField] public Quaternion localQuaternionPivotRot { get; private set; }
        public Camera localCam;
        public Transform pivotToRotate;
        
        [Networked] private Quaternion CurrentPlayerPivotRotation { get; set; }

        public void BeforeUpdate()
        {
            if (GlobalManager.Instance.IsLocalPlayer(Object))
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
                CurrentPlayerPivotRotation = input.gunPivotRotation;
            }
            
            pivotToRotate.rotation = CurrentPlayerPivotRotation;
        }
    }
}