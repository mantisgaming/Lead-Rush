using Demo.Scripts.Runtime;
using System.Collections;
using System.Collections.Generic;
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



    // Start is called before the first frame update
    void Start()
    {
        enemyAgent = gameObject.GetComponent<NavMeshAgent>();
        player = GameObject.FindGameObjectWithTag("Player");
        currentHealth = maxHealth;
        playerController = player.GetComponent<FPSController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!playerController.isPlayerReady || !playerController.isQoeDisabled)
            return;
        enemyAgent.destination = player.transform.position;

       

        largeCollider.transform.localScale = new Vector3(1.5F + Mathf.PingPong(Time.time, 1.0f),1,1);
    }

    public void TakeDamage(float damage)
    {
        currentHealth-=damage;
        if(currentHealth < 0)
        {
            player.GetComponent<FPSController>().killCooldown = .3f;
            player.GetComponent<FPSController>().PlayKillSFX();
            Instantiate(deathPE, headTransform.position, headTransform.rotation);
            //Destroy the Instantiated ParticleSystem 

            Destroy(gameObject);

            player.GetComponent<FPSController>().score += 100;
            player.GetComponent<FPSController>().roundKills++;
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

    
   
}
