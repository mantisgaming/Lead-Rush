using Demo.Scripts.Runtime;
using Michsky.UI.Heat;
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

    float currentHealth;

    public ParticleSystem deathPE;

    public ParticleSystem explodePE;

    public CapsuleCollider largeCollider;

    public Transform headTransform;

    public float minAngleToPlayer;

    public GameObject manager;



    // Start is called before the first frame update
    void Start()
    {
        enemyAgent = gameObject.GetComponent<NavMeshAgent>();
        player = GameObject.FindGameObjectWithTag("Player");
        manager = GameObject.FindGameObjectWithTag("Manager");
        currentHealth = maxHealth;
        playerController = player.GetComponent<FPSController>();


        var relativePos = this.transform.position - player.transform.position;

        var forward = player.transform.forward;
        minAngleToPlayer = Vector3.Angle(relativePos, forward);

    }

    // Update is called once per frame
    void Update()
    {
        if (!playerController.isPlayerReady || !playerController.isQoeDisabled)
            return;
        enemyAgent.destination = player.transform.position;

       

        largeCollider.transform.localScale = new Vector3(1.5F + Mathf.PingPong(Time.time, 1.0f),1,1);

        Debug.Log("Min: " + minAngleToPlayer);
    }

    public void TakeDamage(float damage)
    {
        currentHealth-=damage;
        if(currentHealth < 0)
        {
            EnemyLog();

            FPSController fPSController = player.GetComponent<FPSController>();

            fPSController.degreeToTargetXCumulative += fPSController.degreeToTargetX;
            fPSController.degreeToShootXCumulative += fPSController.degreeToShootX;
            fPSController.minAnlgeToEnemyCumulative += minAngleToPlayer;
            fPSController.killCooldown = .3f;
            fPSController.targetMarked = false;
            fPSController.targetShot = false;
            fPSController.PlayKillSFX();
            Instantiate(deathPE, headTransform.position, headTransform.rotation);
            //Destroy the Instantiated ParticleSystem 

            fPSController.score += 100;
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


        String enemyLogLine =
           roundManager.sessionID.ToString() + "," +
           roundManager.currentRoundNumber.ToString() + "," +
           roundManager.sessionStartTime.ToString() + "," +
           System.DateTime.Now.ToString() + "," +
           roundManager.roundConfigs.roundFPS[roundManager.indexArray[roundManager.currentRoundNumber - 1]].ToString() + "," +
               roundManager.roundConfigs.spikeMagnitude[roundManager.indexArray[roundManager.currentRoundNumber - 1]].ToString() + "," +
               roundManager.roundConfigs.onAimSpikeEnabled[roundManager.indexArray[roundManager.currentRoundNumber - 1]].ToString() + "," +
               roundManager.roundConfigs.onEnemySpawnSpikeEnabled[roundManager.indexArray[roundManager.currentRoundNumber - 1]].ToString() + "," +
               roundManager.roundConfigs.onMouseSpikeEnabled[roundManager.indexArray[roundManager.currentRoundNumber - 1]].ToString() + "," +
               roundManager.roundConfigs.onReloadSpikeEnabled[roundManager.indexArray[roundManager.currentRoundNumber - 1]].ToString() + "," +
               roundManager.indexArray[roundManager.currentRoundNumber - 1].ToString() + "," +
           currentHealth.ToString() + "," +
           minAngleToPlayer.ToString() + "," +
           fPSController.degreeToTargetX.ToString() + "," +
           fPSController.degreeToTargetY.ToString() + "," +
           fPSController.degreeToShootX.ToString() + "," +
           fPSController.degreeToShootY.ToString() + "," +
           fPSController.targetMarked.ToString() + "," +
           fPSController.targetShot.ToString()
            ;
        textWriter.WriteLine(enemyLogLine);
        textWriter.Close();

        fPSController.degreeToTargetX = 0;
        fPSController.degreeToTargetY = 0;
        fPSController.degreeToShootX = 0;
        fPSController.degreeToShootY = 0;
    }

    
   
}
