using System;
using UnityEngine;

namespace Player
{
    public class PlayerVisualController : MonoBehaviour
    {
        [SerializeField] private Animator animator;
        [SerializeField] private Transform pivotGun;
        [SerializeField] private Transform healthCanvas;

        private bool init;
        private Vector3 originalPlayerScale;
        private Vector3 originalPivotGun;
        private Vector3 originalHealthCanvas;
        private readonly int isWalking = Animator.StringToHash("IsWalking");
        private readonly int isShooting = Animator.StringToHash("IsShooting");
        private readonly int die = Animator.StringToHash("Die");
        private const int shootingLayerIndex = 1;

        private void Start()
        {
            originalPlayerScale = transform.localScale;
            originalPivotGun = pivotGun.localScale;
            originalHealthCanvas = healthCanvas.localScale;

            animator.SetLayerWeight(shootingLayerIndex, 1);

            init = true;
        }

        public void RenderVisual(Vector2 velocity, bool isShoot)
        {
            if (!init) return;

            var isMoving = velocity.x is > 0.1f or < -0.1f;

            animator.SetBool(isWalking, isMoving);
            animator.SetBool(isShooting, isShoot);
        }

        public void UpdateScalePlayer(Vector2 velocity)
        {
            if (!init) return;

            if (velocity.x > 0.1f)
            {
                transform.localScale = originalPlayerScale;
                pivotGun.localScale = originalPivotGun;
                healthCanvas.localScale = originalHealthCanvas;
            }
            else if (velocity.x < -0.1f)
            {
                transform.localScale = GetFlip(originalPlayerScale);
                pivotGun.localScale = GetFlip(originalPivotGun);
                healthCanvas.localScale = GetFlip(originalHealthCanvas);
            }
        }

        public void TriggerDieAnimation()
        {
            animator.SetTrigger(die);
        }

        private Vector3 GetFlip(Vector3 vector3)
        {
            return new Vector3(-vector3.x, vector3.y, vector3.z);
        }
    }
}