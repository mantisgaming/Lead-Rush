using Demo.Scripts.Runtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyManager : MonoBehaviour
{
    [SerializeField]
    private GameObject enemy;

    [SerializeField]
    private GameObject player;

    [SerializeField]
    private int maxEnemyCount;

    [SerializeField] private float minSpawnRadius = 10;
    [SerializeField] private float maxSpawnRadius = 15;

    [SerializeField] private float minSpawnDelay = 1;

    [SerializeField] private SpawnGroup spawnGroup;

    void Start()
    {
        SpawnEnemy();
    }

    void SpawnEnemy()
    {
        if (spawnGroup != null) {
            List<SpawnPoint> spawns = new(spawnGroup.SpawnPoints);

            for (int i = 0; i < spawns.Count; i++) {
                int spawnInd = Random.Range(0, spawns.Count);

                Vector3 spawnPos = spawns[spawnInd].transform.position;

                float distance = (spawnPos - player.transform.position).magnitude;

                if (distance < maxSpawnRadius && distance > minSpawnRadius) {
                    Instantiate(enemy, spawnPos, Quaternion.identity);
                    return;
                }

                spawns.RemoveAt(spawnInd);
            }

            Debug.Log("Failed to find spawn point within spawn radius range");
        }

        {
            float dist = Random.Range(minSpawnRadius, maxSpawnRadius);
            float angle = Random.Range(0, 360);

            Vector3 spawnPos = CalculateDistantPoint(player.transform.position, dist, angle);
        }
    }

    public Vector3 CalculateDistantPoint(Vector3 playerPosition, float distance, float angle)
    {
        float angleRad = Mathf.Deg2Rad * angle;
        float xOffset = distance * Mathf.Cos(angleRad);
        float zOffset = distance * Mathf.Sin(angleRad);

        return new Vector3(playerPosition.x + xOffset, playerPosition.y, playerPosition.z + zOffset);
    }
}
