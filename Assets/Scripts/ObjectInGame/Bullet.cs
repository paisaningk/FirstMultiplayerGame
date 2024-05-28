using Fusion;
using UnityEngine;

namespace ObjectInGame
{
    public class Bullet : NetworkBehaviour
    {
        public float moveSpeed = 20;
        public float lifeTime = 0.8f;
        public LayerMask groupLayer;
        private Collider2D colli;

        [Networked] private NetworkBool IsHitSomething { get; set; }
        [Networked] private TickTimer LifeTimeTimer { get; set; }

        public override void Spawned()
        {
            colli = GetComponent<Collider2D>();
            LifeTimeTimer = TickTimer.CreateFromSeconds(Runner, lifeTime);
        }

        public override void FixedUpdateNetwork()
        {
            CheckIfHitGround();

            if (LifeTimeTimer.ExpiredOrNotRunning(Runner) == false && !IsHitSomething)
            {
                transform.Translate(transform.right * moveSpeed * Runner.DeltaTime, Space.World);
            }

            if (LifeTimeTimer.Expired(Runner) || IsHitSomething)
            {
                Runner.Despawn(Object);
            }
        }

        private void CheckIfHitGround()
        {
            var groundCollider = Runner.GetPhysicsScene2D()
                .OverlapBox(transform.position, colli.bounds.size, 0, groupLayer);

            if (groundCollider)
            {
                IsHitSomething = true;
            }
        }
    }
}