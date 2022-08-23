using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hazard : MonoBehaviour
{
    Health healthScript;

    private void Start() {
        healthScript = PlayerController.singleton.GetComponent<Health>();
    }
    
    private void OnCollisionStay2D(Collision2D other) {
        if (other.gameObject.tag == "Player") {
            ContactPoint2D contact = other.contacts[0];
            healthScript.TakeDmg(1, contact.point);
        } else if (other.gameObject.tag == "Enemy") {
            EnemyController enemyScript = other.gameObject.GetComponent<EnemyController>();
            if (!enemyScript.HasDied()) {
                enemyScript.TakeDmg(100);
            }
        }
    }
}
