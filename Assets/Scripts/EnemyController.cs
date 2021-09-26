using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    private GameObject player;
    private PlayerController playerScript;
    // Detecting the player
    public GameObject unalertedSight;
    public GameObject alertedSight; 
    private float lastDir;
    private bool isAlerted;
    private bool playerDetected;
    // Attacking the player
    public float attackSpeed;
    public float dmg;

    
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        playerScript = player.GetComponent<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        FindPlayer();
        Move();
    }

    // Controls the movement of the enemy
    private void Move() {
        // if (alerted) {
        //     // Move towards the player
        // } else {
        //     // Idle movement

        // }
    }


    // Returns if the player is within detection range.
    private bool FindPlayer() {
        return false;
    }

    private void OnCollisionStay2D(Collision2D other) {
        if (other.gameObject.CompareTag("Player")) {
            // Damage the player
        }
    }




}
