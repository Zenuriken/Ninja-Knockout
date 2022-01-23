using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shuriken : MonoBehaviour
{
    public float shurikenSpeed = 20;
    public float spinSpeed = 1000;
    private Rigidbody2D rb;
    private int throwDir;
    private PlayerController playerScript;

    // private void Awake()
    // {
    //     rb = this.GetComponent<Rigidbody2D>();
    //     playerScript = GameObject.Find("Player").GetComponent<PlayerController>();
    //     throwDir = playerScript.GetPlayerDir();
    // }

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
            Destroy(this.gameObject);
        }
    }

    // Sets the shuriken's velocity direction
    public void SetThrowDir(int dir) {
        throwDir = dir;
    }
}
