using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shuriken : MonoBehaviour
{
    public float shurikenSpeed = 20;
    public float spinSpeed = 1000;
    public float destroyDelay = 0.0001f;
    private Rigidbody2D rb;
    private CircleCollider2D collider;
    private TrailRenderer trailRen;
    private int throwDir;
    private GameObject player;
    private PlayerController playerScript;
    private Melee meleeScript;

    private float deflectedMultiplier = 1.5f;

    private int numDeflections = 0;

    public Gradient deflectedOnceColor;
    public Gradient deflectedTwiceColor;

    private int deflectCounter;

    private void Awake()
    {
        rb = this.GetComponent<Rigidbody2D>();
        collider = this.GetComponent<CircleCollider2D>();
        trailRen = this.GetComponent<TrailRenderer>();
        player = GameObject.Find("Player");
        playerScript = player.GetComponent<PlayerController>();
        meleeScript = player.transform.GetChild(1).GetComponent<Melee>();
        throwDir = playerScript.GetPlayerDir();
    }

    // private void Start() {
    //     if (throwDir == -1) {
    //         rb.AddTorque(spinSpeed);
    //     } else {
    //         rb.AddTorque(-spinSpeed);  
    //     }  
    //     rb.velocity = new Vector2(throwDir * shurikenSpeed, 0);
    // }

    // When the shuriken hits an enemy
    private void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.tag == "Enemy") {
            EnemyController enemyScript = other.gameObject.GetComponent<EnemyController>();
            if (!enemyScript.IsAlerted()) {
                enemyScript.TakeDmg(5);
            } else {
                enemyScript.TakeDmg(1);
            }
        }
        if (other.gameObject.tag == "Enemy" || other.gameObject.tag == "Platform") {
            Invoke("DestroyShuriken", destroyDelay);
        }
    }

    // Destroys Shuriken
    public void DestroyShuriken() {
        meleeScript.RemoveProjFromList(collider);
        Destroy(this.gameObject);
    }

    // Sets the shuriken's velocity direction
    public void SetShurikenDir(int dir) {
        throwDir = dir;
    }

    public void Deflected() {
        numDeflections += 1;
        Debug.Log(numDeflections);
        rb.velocity = Vector2.zero;
        if (throwDir == -1) {
            throwDir = 1;
            this.transform.rotation = Quaternion.identity;
            //rb.AddTorque(-spinSpeed);
        } else {
            throwDir = -1;
            //rb.AddTorque(spinSpeed);  
            this.transform.rotation = Quaternion.identity;
        }
        if (numDeflections == 1) {
           // Change trail color orange
           trailRen.colorGradient = deflectedOnceColor;
        }  else if (numDeflections == 2) {
            // Change trail color red
            trailRen.colorGradient = deflectedTwiceColor;
            deflectedMultiplier += 0.5f;
        }
        rb.velocity = new Vector2(throwDir * shurikenSpeed * deflectedMultiplier, 0);
    }

    // Checks to see if shuriken has already been deflected by player's current meleeCounter.
    public bool HasBeenDeflected(int counter) {
        return this.deflectCounter == counter;
    }

    // Assigns the player's melee counter to the shuriken after being deflected.
    public void SetDeflectedCounter(int counter) {
        this.deflectCounter = counter;
    }
}
