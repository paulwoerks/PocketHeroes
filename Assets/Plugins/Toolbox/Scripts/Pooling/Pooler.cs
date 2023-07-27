using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Toolbox.Singleton;
using Toolbox.Coroutines;

// Author: Paul Woerner

namespace Toolbox.Pooling
{
    public interface ISpawn { void OnSpawn(); }
    public interface IDespawn { void OnDespawn(); }

    ///<summary>Pools Objects dynamically</summary>
    public class Pooler : Singleton<Pooler>{
        [SerializeField] Pool[] preloadedPools = new Pool[] {}; // Pools that get preloaded on Awake
        Dictionary<string, Pool> pools = new();
        Transform cachedInstances;
        Transform activeInstances;

        public override void Awake(){
            base.Awake();
            CreateContainers();
            Preload();
        }

        private void CreateContainers(){
            Instance.cachedInstances = new GameObject("[-]Cache").transform;
            Instance.cachedInstances.SetParent(transform);
            Instance.cachedInstances.gameObject.SetActive(false);

            Instance.activeInstances = new GameObject("[+]Active").transform;
            Instance.activeInstances.SetParent(transform);
        }

        private void Preload(){
            foreach (Pool pool in preloadedPools)
            {
                pool.Preload();
                pools.Add($"{pool.Prefab.name}(Clone)", pool);
            }
            this.Log(preloadedPools.Length + " Pools preloaded", debug);
        }

        ///<summary>Spawn Instance of a Prefab. Optional set Parent</summary>
        public static GameObject Spawn(GameObject prefab, Vector3 position, Quaternion rotation, Transform parent = null) => Instance.SpawnInstance(prefab, position, rotation, parent);
        GameObject SpawnInstance(GameObject prefab, Vector3 position, Quaternion rotation, Transform parent = null){
            Transform pooledObject = GetPool(prefab).GetInstance().transform;

            parent ??= activeInstances;

            pooledObject.SetParent(parent, true);
            pooledObject.SetPositionAndRotation(position, rotation);

            if (pooledObject.TryGetComponent<ISpawn>(out ISpawn ISpawn))
                ISpawn.OnSpawn();

            this.Log($"[+] '{prefab.name}'", debug);

            return pooledObject.gameObject;
        }

        ///<summary>Despawn existing Instance.</summary>
        public static void Despawn(GameObject instance, float delay = 0f) {
            if (instance == null)
                return;
            if (delay > 0f)
                CoroutineRunner.Start(Instance.WaitForDelay(delay, Instance.DespawnInstance, instance));
            else
                Instance.DespawnInstance(instance);
        }
        void DespawnInstance(GameObject instance)
        {
            if (pools.TryGetValue(instance.name, out Pool pool))
            {
                if (instance.TryGetComponent<IDespawn>(out IDespawn IDespawn))
                    IDespawn.OnDespawn();

                pool.Despawn(instance);
                instance.transform.SetParent(cachedInstances);
                this.Log($"[-] '{pool.Prefab.name}'", debug);
            }
            else
                this.LogWarning($"Pool '{instance.name.Replace("(Clone)", "")}' not found.", debug);
        }

        IEnumerator WaitForDelay(float delay, Action<GameObject> call, GameObject instance) {
            yield return new WaitForSeconds(delay);
            call.Invoke(instance);
        }

        private Pool GetPool(GameObject prefab) {
            string key = $"{prefab.name}(Clone)";
            Pool existingPool = null;

            return pools.TryGetValue(key, out existingPool) ? existingPool : pools[key] = new Pool(prefab);
        }
    }
}