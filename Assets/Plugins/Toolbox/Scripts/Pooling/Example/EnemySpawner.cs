using System.Collections.Generic;
using UnityEngine;
using Toolbox.Pooling;

namespace Toolbox.Examples
{
    public class EnemySpawner : MonoBehaviour
    {
        [SerializeField] bool debug;
        [SerializeField] GameObject prefab;
        Queue<GameObject> enemies = new();

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
                SpawnCube();

            if (Input.GetKeyDown(KeyCode.O))
                DespawnCube();
        }
        void SpawnCube()
        {
            this.Log("Spawn", debug);
            Vector3 position = RandomPosition(5);
            Quaternion rotation = Quaternion.identity;
            enemies.Enqueue(Pooler.Spawn(prefab, position, rotation));
        }

        void DespawnCube()
        {
            if (enemies.Count <= 0)
                return;

            this.Log("Despawn", debug);

            Pooler.Despawn(enemies.Dequeue());
        }

        Vector3 RandomPosition(float range) => new Vector3(Random.Range(-range, range), Random.Range(-range, range), 0);
    }
}
