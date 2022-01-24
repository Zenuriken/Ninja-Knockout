using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    private GameObject player;
    private PlayerController playerScript;
    // Detecting the player
    private PolygonCollider2D unalertedSight;
    private PolygonCollider2D alertedSight; 
    private bool isAlerted;
    private bool playerDetected;

    private Melee meleeScript;
    private BoxCollider2D enemyCollider;
    private int enemyHealth;
    // Attacking the player
    public float attackSpeed;
    public float dmg;

    public float destroyDelay;

    private bool hasDied;

    // The player's meleeCounter that damaged the enemy
    private int damageCounter;

    
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        playerScript = player.GetComponent<PlayerController>();
        meleeScript = player.transform.GetChild(1).GetComponent<Melee>();
        unalertedSight = this.transform.GetChild(0).GetComponent<PolygonCollider2D>();
        alertedSight = this.transform.GetChild(1).GetComponent<PolygonCollider2D>();
        enemyCollider = this.GetComponent<BoxCollider2D>();
        isAlerted = false;
        enemyHealth = 5;
        hasDied = false;
    }

    // Returns whether or not an enemy is alerted to the player's presence.
    private void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.tag == "Player" && other.IsTouching(unalertedSight)) {
            //Debug.Log("Alerted!");
            isAlerted = true;
        }
    }

    // Returns if enemy is in alerted state.
    public bool IsAlerted() {
        return isAlerted;
    }

    // Reduces the enemy's health by dmg.
    public void TakeDmg(int dmg) {
        enemyHealth -= dmg;
        if (enemyHealth <= 0) {
            Invoke("DestroyEnemy", destroyDelay);
        }
    }

    private void DestroyEnemy() {
        meleeScript.RemoveEnemyFromList(enemyCollider);
        Destroy(this.gameObject);
    }

    // Returns the enemy's current health.
    public int GetHealth() {
        return this.enemyHealth;
    }
    
    public bool HasDied() {
        return this.hasDied;
    }

    // Assigns the player's melee counter to the enemy after being damaged.
    public void SetDamagedCounter(int counter) {
        this.damageCounter = counter;
    }

    // Checks to see if enemy has already been damaged by player's current meleeCounter.
    public bool HasBeenDamaged(int counter) {
        return this.damageCounter == counter;
    }




}
