using System.Collections.Generic;
using UnityEngine;
using Toolbox;
using Toolbox.Pooling;

public class EnemySpawner : MonoBehaviour{
        [SerializeField] bool debug;
        [SerializeField] GameObject prefab;
        Queue<GameObject> enemies = new();

        [SerializeField] TransformAnchor playerAnchor;

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
                SpawnCube();

            if (Input.GetKeyDown(KeyCode.O))
                DespawnCube();
        }
        void SpawnCube()
        {
            if (!playerAnchor.IsSet)
                return;

            this.Log("Spawn", debug);
            Vector3 position = RandomPosition(15) + playerAnchor.Value.position;
            Quaternion rotation = Quaternion.identity;
            enemies.Enqueue(Pooler.Spawn(prefab, position + playerAnchor.Value.position, rotation));
        }

        void DespawnCube()
        {
            if (enemies.Count <= 0)
                return;

            this.Log("Despawn", debug);

            Pooler.Despawn(enemies.Dequeue());
        }

        Vector3 RandomPosition(float range) => new Vector3(Random.Range(-range, range), 0, Random.Range(-range, range));
    }

