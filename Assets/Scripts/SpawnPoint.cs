using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPoint : MonoBehaviour {

    [SerializeField]
    private SpawnGroup m_spawnGroup;

    public SpawnGroup spawnGroup {
        get { return m_spawnGroup; }
    }

    private void OnEnable() {
        spawnGroup.RegisterSpawnPoint(this);
    }

    private void OnDisable() {
        spawnGroup.DeregisterSpawnPoint(this);
    }

    private void OnDrawGizmos() {

        Gizmos.DrawIcon(transform.position, "SpawnPoint.png", true, spawnGroup != null ? spawnGroup.IconColor : Color.white);
    }
}
