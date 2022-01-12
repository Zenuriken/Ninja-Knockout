using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shuriken : MonoBehaviour
{
    //private float lastDir;
    public float spinSpeed;
    private GameObject player;
    private PlayerController playerScript;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        playerScript = player.GetComponent<PlayerController>();
        // lastDir = playerScript.getPlayerDir();
        // if (lastDir == -1) {
        //     this.transform.Rotate(0, 0, spinSpeed); 
        // } else {
        //    this.transform.Rotate(0, 0, -spinSpeed);   
        // } 
    }

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

    // Update is called once per frame
    void Update()
    {
        // float lastDir = playerScript.getPlayerDir();
        // if (lastDir == -1) {
        //     this.transform.Rotate(0, 0, spinSpeed); 
        // } else {
        //    this.transform.Rotate(0, 0, -spinSpeed);   
        // }  
    }
}
