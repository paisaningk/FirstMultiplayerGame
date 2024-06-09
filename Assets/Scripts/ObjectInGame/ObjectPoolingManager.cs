using System;
using System.Collections.Generic;
using System.Linq;
using Fusion;
using Sirenix.OdinInspector;
using UnityEngine;
using utilities;

namespace ObjectInGame
{
    public class ObjectPoolingManager : MonoBehaviour, INetworkObjectPool
    {
        // the key is network object , the value is network object that had been spawned
        [ShowInInspector] private Dictionary<NetworkObject, List<NetworkObject>> prefabDictionary = new();

        public void Start()
        {
            if (GlobalManager.Instance)
            {
                GlobalManager.Instance.ObjectPoolingManager = this;
            }
        }

        //call once runner.spawn is called
        public NetworkObject AcquireInstance(NetworkRunner runner, NetworkPrefabInfo info)
        {
            NetworkObject networkObject = null;
            NetworkProjectConfig.Global.PrefabTable.TryGetPrefab(info.Prefab, out var prefab);
            prefabDictionary.TryGetValue(prefab, out var networkObjects);

            var foundMatch = false;

            if (networkObjects?.Count > 0)
            {
                foreach (var item in networkObjects)
                {
                    if (item && item.gameObject.activeSelf == false)
                    {
                        networkObject = item;

                        foundMatch = true;
                    }
                }
            }

            if (foundMatch == false)
            {
                networkObject = CreateNetworkObject(prefab);
            }

            return networkObject;
        }

        private NetworkObject CreateNetworkObject(NetworkObject networkObject)
        {
            var obj = Instantiate(networkObject);

            if (prefabDictionary.TryGetValue(networkObject, out var listData))
            {
                listData.Add(obj);
            }
            else
            {
                var list = new List<NetworkObject> { obj };
                prefabDictionary.Add(networkObject, list);
            }

            return obj;
        }

        //called once runner.despawn is called
        public void ReleaseInstance(NetworkRunner runner, NetworkObject instance, bool isSceneObject)
        {
            instance.gameObject.SetActive(false);
        }

        public void RemoveObjectFromDictionary(NetworkObject obj)
        {
            foreach (var keyValuePair in prefabDictionary)
            {
                foreach (var networkObject in keyValuePair.Value.Where(T => T == obj))
                {
                    keyValuePair.Value.Remove(networkObject);
                    break;
                }
            }
        }
    }
}