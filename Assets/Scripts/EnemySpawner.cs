using System.Collections.Generic;
using UnityEngine;
using Toolbox;
using Toolbox.Pooling;

public class EnemySpawner : MonoBehaviour{
        [SerializeField] bool debug;
        [SerializeField] GameObject prefab;

        [SerializeField] Transform parent;
        Queue<GameObject> enemies = new();

        [SerializeField] TransformAnchor playerAnchor;

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
                SpawnGroup(prefab);
        }

        void SpawnGroup(GameObject prefab){
            Vector3 spawnArea = RandomPosition(15) + playerAnchor.Value.position;

            int units = Random.Range(2, 6);
            float unitDistance = 3f; 

            for (int i = 0; i < units; i++){
                Vector3 position = RandomPosition(unitDistance, spawnArea);
                Quaternion rotation = Quaternion.Euler(0, Random.Range(0,360), 0);
                GameObject enemy = Pooler.Spawn(prefab, position, rotation, parent);
                enemies.Enqueue(enemy);
            }
            
        }

        void DespawnUnit()
        {
            if (enemies.Count <= 0)
                return;

            this.Log("Despawn", debug);

            Pooler.Despawn(enemies.Dequeue());
        }

        Vector3 RandomPosition(float range, Vector3 center = default) => new Vector3(Random.Range(-range, range), 0, Random.Range(-range, range)) + center;
    }

