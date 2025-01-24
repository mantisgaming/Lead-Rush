
using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

[CreateAssetMenu(fileName = "New Spawn Group", menuName = "Game/Spawn Group")]
public class SpawnGroup : ScriptableObject {

    private List<SpawnPoint> spawnPoints = new();

    public SpawnPoint[] SpawnPoints { get { return spawnPoints.ToArray(); } }

    [SerializeField]
    private Color iconColor = Color.white;

    public Color IconColor { get { return iconColor; } }

    public SpawnPoint GetRandomSpawnPoint() {
        if (spawnPoints == null)
            return null;

        if (spawnPoints.Count == 0)
            return null;

        return spawnPoints[Random.Range(0, spawnPoints.Count-1)];
    }

    public void RegisterSpawnPoint(SpawnPoint spawnPoint) {
        if (spawnPoint.spawnGroup != this) {
            Debug.LogWarning($"Spawn point registered with incorrect spawn group", spawnPoint);
            return;
        }
    }

    public void DeregisterSpawnPoint(SpawnPoint spawnPoint) {
        if (spawnPoint.spawnGroup != this) {
            Debug.LogWarning($"Spawn point deregistered with incorrect spawn group", spawnPoint);
            return;
        }
    }
}
