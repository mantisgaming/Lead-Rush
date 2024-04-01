using Demo.Scripts.Runtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public GameObject enemy;

    public GameObject player;
    public FPSController playerController;
    public List<Transform> sapwnPoints;
    public List<Enemy> enemies;

    public float spawnDuration;
    public float spawnTimer;
    int enemiesInScene;
    public int maxEnemyCount;

    void Start()
    {
        TimerReset();
        SpawnEnemy();
        playerController = player.GetComponent<FPSController>();
    }

    void TimerReset()
    {
        spawnTimer = spawnDuration;
    }

    // Update is called once per frame
    void Update()
    {
        if (!playerController.isPlayerReady || !playerController.isQoeDisabled)
            return;

        spawnTimer -= Time.deltaTime;
        enemiesInScene = GameObject.FindGameObjectsWithTag("Enemy").Length;
        if (spawnTimer < 0 && enemiesInScene< maxEnemyCount)
        {
            SpawnEnemy();
            spawnTimer = spawnDuration;
        }
    }

    public void DestroyAllEnemy()
    {
       foreach (GameObject enemy in GameObject.FindGameObjectsWithTag("Enemy"))
            Destroy(enemy);
    }

    public GameObject GetClosestEnemy()
    {
        float minDist = 9999999;
        GameObject closestEnemyGO = null;


        foreach (GameObject enemy in GameObject.FindGameObjectsWithTag("Enemy"))
        {
            if (Vector3.Distance(player.transform.position, enemy.transform.position) < minDist) {
                minDist = Vector3.Distance(player.transform.position, enemy.transform.position);
                closestEnemyGO = enemy;
            }
        }
        return closestEnemyGO;
    }

    void SpawnEnemy()
    {
        int spawnIndex = Random.Range(0, sapwnPoints.Count - 1);
        Instantiate(enemy, sapwnPoints[spawnIndex].position, sapwnPoints[spawnIndex].rotation);
    }
}
