using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Toolbox;
using Toolbox.Pooling;

public class EnemySpawner : MonoBehaviour{
        [SerializeField] LayerMask spawnableLayers;
        [SerializeField] bool debug;
        [SerializeField] GameObject prefab;

        [SerializeField] GameObject spawnFX;

        [SerializeField] Transform parent;
        Queue<GameObject> enemies = new();

        [SerializeField] TransformAnchor playerAnchor;

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
                StartCoroutine(SpawnGroup(prefab));
        }

        IEnumerator SpawnGroup(GameObject prefab){
            Vector3 spawnArea = GetSpawnPoint(playerAnchor.Value.position, 15);

            Pooler.Spawn(spawnFX, spawnArea, Quaternion.identity);

            int units = Random.Range(2, 6);
            float unitDistance = 3f; 

            List<Vector3> spawnPositions = new();
            for (int i = 0; i < units; i++){ 
                spawnPositions.Add(GetSpawnPoint(spawnArea, unitDistance));
            }

            yield return new WaitForSeconds(.5f);

            foreach (Vector3 position in spawnPositions){
                Quaternion rotation = Quaternion.Euler(0, Random.Range(0,360), 0);
                GameObject enemy = Pooler.Spawn(prefab, position, rotation, parent);
                enemies.Enqueue(enemy);
            }
        }

        Vector3 GetSpawnPoint(Vector3 origin, float radius){
            
            bool isWalkable = false;
            Vector3 position = default;

            int i = 0;

            while (!isWalkable && i < 100){
                Vector3 randomPosition = new Vector3(Random.Range(-radius, radius), 0, Random.Range(-radius, radius));
                position = origin + randomPosition;

                isWalkable = IsWalkable(position);
                i++;
            }

            if (position.Equals(default)){
                this.LogError("No valid Position found");
            }

            return position;
        }

        public bool IsWalkable(Vector3 position){
            float height = 10f;
            position.y = height;

            RaycastHit hit;

            if (Physics.Raycast(position, Vector3.down, out hit, height, spawnableLayers))
                return true;
            else
                return false;
        }

        void DespawnUnit()
        {
            if (enemies.Count <= 0)
                return;

            this.Log("Despawn", debug);

            Pooler.Despawn(enemies.Dequeue());
        }
    }

