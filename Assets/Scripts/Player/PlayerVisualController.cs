using UnityEngine;

namespace Player
{
    public class PlayerVisualController : MonoBehaviour
    {
        [SerializeField] private Animator animator;
        private readonly int isWalking = Animator.StringToHash("IsWalking");

        public void RenderVisual(Vector2 velocity)
        {
            var isMoving = velocity.x is > 0.1f or < -0.1f;

            animator.SetBool(isWalking, isMoving);
        }
    }
}