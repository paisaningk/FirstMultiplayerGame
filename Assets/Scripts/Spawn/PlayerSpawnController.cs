using System.Collections.Generic;
using Fusion;
using Unity.Mathematics;
using UnityEngine;
using utilities;
using Random = UnityEngine.Random;

namespace Spawn
{
    public class PlayerSpawnController : NetworkBehaviour, IPlayerJoined, IPlayerLeft
    {
        [SerializeField] private NetworkPrefabRef playerNetworkPrefab = NetworkPrefabRef.Empty;
        [SerializeField] private List<GameObject> spawnPointList;

        private void Awake()
        {
            if (GlobalManager.Instance)
            {
                GlobalManager.Instance.PlayerSpawnController = this;
            }
        }

        public Vector2 GetRandomSpawnPoint()
        {
            return spawnPointList[Random.Range(0, spawnPointList.Count)].transform.position;
        }

        public void PlayerJoined(PlayerRef player)
        {
            SpawnPlayer(player);
        }

        public void PlayerLeft(PlayerRef player)
        {
            DeSpawnPlayer(player);
        }

        public override void Spawned()
        {
            if (Runner.IsServer)
            {
                foreach (var player in Runner.ActivePlayers)
                {
                    SpawnPlayer(player);
                }
            }
        }

        private void SpawnPlayer(PlayerRef playerRef)
        {
            if (Runner.IsServer)
            {
                var index = playerRef % spawnPointList.Count;
                var spawnPoint = spawnPointList[index].transform.position;
                var playerObject = Runner.Spawn(playerNetworkPrefab, spawnPoint, quaternion.identity, playerRef);

                Runner.SetPlayerObject(playerRef, playerObject);
            }
        }

        private void DeSpawnPlayer(PlayerRef playerRef)
        {
            if (Runner.IsServer)
            {
                if (Runner.TryGetPlayerObject(playerRef, out var playerNetworkObject))
                {
                    Runner.Despawn(playerNetworkObject);
                }

                Runner.SetPlayerObject(playerRef, null);
            }
        }
    }
}