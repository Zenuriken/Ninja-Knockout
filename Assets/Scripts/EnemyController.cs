using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    private GameObject player;
    private PlayerController playerScript;
    // Detecting the player
    private GameObject unalertedObj;
    private GameObject alertedObj;
    private SpriteRenderer unalertedSprite;
    private SpriteRenderer alertedSprite;
    private PolygonCollider2D unalertedCol;
    private PolygonCollider2D alertedCol; 
    private bool isAlerted;
    private bool playerDetected;

    private Melee meleeScript;
    private BoxCollider2D enemyCollider;
    private Rigidbody2D enemyRB;
    private int enemyHealth;


    // Getting Damaged
    public float knockBackDur;
    public float knockBackForce;

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

        unalertedObj = this.transform.GetChild(0).gameObject;
        alertedObj = this.transform.GetChild(1).gameObject;
        unalertedSprite = unalertedObj.GetComponent<SpriteRenderer>();
        alertedSprite = alertedObj.GetComponent<SpriteRenderer>();
        unalertedCol = unalertedObj.GetComponent<PolygonCollider2D>();
        alertedCol = alertedObj.GetComponent<PolygonCollider2D>();
        enemyCollider = this.GetComponent<BoxCollider2D>();
        enemyRB = this.GetComponent<Rigidbody2D>();
        isAlerted = false;
        enemyHealth = 5;
        hasDied = false;
    }

    private void Update() {
        
    }

    // Returns whether or not an enemy is alerted to the player's presence.
    private void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.tag == "Player" && other.IsTouching(unalertedCol) && !playerScript.GetHidingStatus()) {
            isAlerted = true;
            unalertedSprite.color = new Color(1f, 1f, 0f, 0);
            alertedSprite.color = new Color(1f, 0f, 0f, 0.18f);
        }
    }

    // Returns if enemy is in alerted state.
    public bool IsAlerted() {
        return isAlerted;
    }

    // Reduces the enemy's health by dmg.
    public void TakeDmg(int dmg) {
        enemyHealth -= dmg;
        StartCoroutine(KnockBack(new Vector2(playerScript.GetPlayerDir(), 0f)));
        if (enemyHealth <= 0) {
            Invoke("DestroyEnemy", destroyDelay);
        }
    }

    // Removes the enemy from the player's melee collider list and destroys the enemy.
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

    IEnumerator KnockBack(Vector2 playerDir) {
        enemyRB.velocity = new Vector2(0f, 0f);
        enemyRB.AddForce(playerDir * knockBackForce, ForceMode2D.Impulse);
        yield return new WaitForSeconds(knockBackDur);
    }





}
