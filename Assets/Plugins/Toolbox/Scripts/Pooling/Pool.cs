using UnityEngine;
using System.Collections.Generic;

namespace Toolbox.Pooling
{
    [System.Serializable]
    ///<summary>A Pool of objects based on a Queue</summary>///
    public class Pool
    {
        [SerializeField] GameObject prefab;
        public GameObject Prefab => prefab;
        [SerializeField] int preloadSize;
        Queue<GameObject> instances = new();

        public Pool(GameObject prefab) => this.prefab = prefab;

        public void Preload()
        {
            for (int i = 0; i < preloadSize; i++)
                instances.Enqueue(GameObject.Instantiate(prefab));
        }

        public GameObject GetInstance() => (instances.Count > 0 ? instances.Dequeue() : GameObject.Instantiate(prefab));

        public void Despawn(GameObject instance) => instances.Enqueue(instance);
    }
}