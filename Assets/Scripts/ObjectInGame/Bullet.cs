using System.Collections.Generic;
using Fusion;
using Player;
using UnityEngine;

namespace ObjectInGame
{
    public class Bullet : NetworkBehaviour
    {
        public float moveSpeed = 20;
        public float lifeTime = 0.8f;
        public int damage = 10;
        public LayerMask groupLayer;
        public LayerMask playerLayer;
        private Collider2D colli;

        [Networked] private NetworkBool IsHitSomething { get; set; }
        [Networked] private TickTimer LifeTimeTimer { get; set; }

        private List<LagCompensatedHit> hits = new();

        public override void Spawned()
        {
            colli = GetComponent<Collider2D>();
            LifeTimeTimer = TickTimer.CreateFromSeconds(Runner, lifeTime);
        }

        public override void FixedUpdateNetwork()
        {
            if (!IsHitSomething)
            {
                CheckIfHitGround();
                CheckIfWeHitPlayer();
            }

            if (LifeTimeTimer.ExpiredOrNotRunning(Runner) == false && !IsHitSomething)
            {
                transform.Translate(transform.right * moveSpeed * Runner.DeltaTime, Space.World);
            }

            if (LifeTimeTimer.Expired(Runner) || IsHitSomething)
            {
                LifeTimeTimer = TickTimer.None;
                Runner.Despawn(Object);
            }
        }

        private void CheckIfHitGround()
        {
            var groundCollider = Runner.GetPhysicsScene2D()
                .OverlapBox(transform.position, colli.bounds.size, 0, groupLayer);

            if (groundCollider != default)
            {
                IsHitSomething = true;
            }
        }

        private void CheckIfWeHitPlayer()
        {
            Runner.LagCompensation.OverlapBox(transform.position, colli.bounds.size, Quaternion.identity,
                Object.InputAuthority, hits, playerLayer);

            if (hits.Count <= 0) return;

            foreach (var hit in hits)
            {
                if (hit.Hitbox == null) continue;
                var player = hit.Hitbox.GetComponentInParent<PlayerController>();

                var notHitOwnPlayer = player.Object.InputAuthority.PlayerId != Object.InputAuthority.PlayerId;

                if (notHitOwnPlayer && player.IsAlive)
                {
                    if (Runner.IsServer)
                    {
                        player.GetComponent<PlayerHealthController>().RPCReducePlayerHealth(damage);
                        Debug.Log($"Hit {player.Object.Name}");
                    }

                    IsHitSomething = true;
                    break;
                }
            }
        }
    }
}