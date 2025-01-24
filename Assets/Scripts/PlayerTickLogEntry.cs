
using UnityEngine;

[System.Serializable]
public struct PlayerTickLogEntry {
    public string time;
    public float roundTimer;
    public float mouseX;
    public float mouseY;

    public float playerX;
    public float playerY;
    public float playerZ;

    public Quaternion playerRot;

    public Vector3 enemyPos;
    public bool isADS;

    public float scorePerSec;
    public double frameTimeMS;
}
