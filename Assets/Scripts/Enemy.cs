using Demo.Scripts.Runtime;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Audio;

public class Enemy : MonoBehaviour
{

    GameObject player;
    public FPSController playerController;
    NavMeshAgent enemyAgent;

    public float maxHealth;

    private float currentHealth;

    public ParticleSystem deathPE;

    public ParticleSystem explodePE;

    public Transform headTransform;

    public float minAngleToPlayer;

    public GameObject manager;

    public GameObject enemyHead;

    public float angularSizeOnSpawn;



    // Start is called before the first frame update
    void Start()
    {
        enemyAgent = gameObject.GetComponent<NavMeshAgent>();
        player = GameObject.FindGameObjectWithTag("Player");
        manager = GameObject.FindGameObjectWithTag("Manager");
        playerController = player.GetComponent<FPSController>();

        maxHealth = playerController.enemyHealthGlobal;
        currentHealth = maxHealth;
        


        var relativePos = this.transform.position - player.transform.position;

        var forward = player.transform.forward;
        minAngleToPlayer = Vector3.Angle(relativePos, forward);
        angularSizeOnSpawn = playerController.CalculateAngularSize(enemyHead, playerController.mainCamera.position);

        enemyAgent.speed = playerController.enemySpeedGlobal;
    }

    // Update is called once per frame
    void Update()
    {
        if (!playerController.isPlayerReady || !playerController.isQoeDisabled || !playerController.isAcceptabilityDisabled)
            return;
        enemyAgent.destination = player.transform.position;
    }

    public void TakeDamage(float damage)
    {
        currentHealth-=damage;
        if(currentHealth < 0)
        {
            FPSController fPSController = player.GetComponent<FPSController>();

            fPSController.degreeToTargetXCumulative += fPSController.degreeToTargetX;
            fPSController.degreeToShootXCumulative += fPSController.degreeToShootX;

            fPSController.timeToTargetEnemyCumulative += fPSController.timeToTargetEnemy;
            fPSController.timeToHitEnemyCumulative += fPSController.timeToHitEnemy;
            fPSController.timeToKillEnemyCumulative += fPSController.timeToKillEnemy;

            fPSController.minAnlgeToEnemyCumulative += minAngleToPlayer;
            fPSController.enemySizeCumulative += angularSizeOnSpawn;

            EnemyLog();
            
            fPSController.killCooldown = .3f;
            fPSController.targetMarked = false;
            fPSController.targetShot = false;
            fPSController.PlayKillSFX();
            Instantiate(deathPE, headTransform.position, headTransform.rotation);
            //Destroy the Instantiated ParticleSystem 

            fPSController.score += fPSController.onKillScore;
            fPSController.roundKills++;

            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log("Enemycol: " + other.gameObject.name);
        if (other.gameObject.tag == "Player")
        {
            Instantiate(explodePE, headTransform.position, headTransform.rotation);
            player.GetComponent<FPSController>().PlayDeathSFX();
            player.GetComponent<FPSController>().RespawnPlayer();
        }
    }

    public void EnemyLog()
    {

        RoundManager roundManager = manager.GetComponent<RoundManager>();

        FPSController fPSController = player.GetComponent<FPSController>();

        TextWriter textWriter = null;
        string filenameEnemyLog = "Data\\Logs\\EnemyData_" + roundManager.fileNameSuffix + "_" + roundManager.sessionID + "_" + ".csv";

        while (textWriter == null)
            textWriter = File.AppendText(filenameEnemyLog);


        string enemyLogLine =
            $"{roundManager.sessionID}," +
            $"{roundManager.latinRow}," +
            $"{roundManager.currentRoundNumber}," +
            $"{roundManager.sessionStartTime}," +
            $"{DateTime.Now}," +
            $"{roundManager.currentRoundConfig.roundFPS}," +
            $"{roundManager.currentRoundConfig.spikeMagnitude}," +
            $"{roundManager.currentRoundConfig.onAimSpikeEnabled}," +
            $"{roundManager.currentRoundConfig.onEnemySpawnSpikeEnabled}," +
            $"{roundManager.currentRoundConfig.onMouseSpikeEnabled}," +
            $"{roundManager.currentRoundConfig.onReloadSpikeEnabled}," +
            $"{roundManager.indexArray[roundManager.currentRoundNumber - 1]}," +
            $"{currentHealth}," +
            $"{minAngleToPlayer}," +
            $"{angularSizeOnSpawn}," +
            $"{fPSController.degreeToTargetX}," +
            $"{fPSController.degreeToTargetY}," +
            $"{fPSController.degreeToShootX}," +
            $"{fPSController.degreeToShootY}," +
            $"{fPSController.timeToTargetEnemy}," +
            $"{fPSController.timeToHitEnemy}," +
            $"{fPSController.timeToKillEnemy}," +
            $"{fPSController.targetMarked}," +
            $"{fPSController.targetShot}";

        textWriter.WriteLine(enemyLogLine);
        textWriter.Close();

        fPSController.degreeToTargetX = 0;
        fPSController.degreeToTargetY = 0;
        fPSController.degreeToShootX = 0;
        fPSController.degreeToShootY = 0;

        fPSController.timeToKillEnemy = 0;
        fPSController.timeToHitEnemy = 0;
        fPSController.timeToTargetEnemy = 0;
    }
}
