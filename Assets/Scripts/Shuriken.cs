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
    private int throwDir;
    private GameObject player;
    private PlayerController playerScript;
    private Melee meleeScript;

    private void Awake()
    {
        rb = this.GetComponent<Rigidbody2D>();
        collider = this.GetComponent<CircleCollider2D>();
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
            //Invoke("DestroyShuriken", destroyDelay);
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
        rb.velocity = Vector2.zero;
        if (throwDir == -1) {
            throwDir = 1;
            rb.AddTorque(-spinSpeed);
        } else {
            throwDir = -1;
            rb.AddTorque(spinSpeed);  
        }  
        //rb.velocity = new Vector2(throwDir * shurikenSpeed, 0);
    }
}
