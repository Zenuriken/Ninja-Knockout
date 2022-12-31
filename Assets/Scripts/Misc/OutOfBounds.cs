using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutOfBounds : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.tag == "Player") {
            Health playerHealthScript = other.gameObject.GetComponent<Health>();
            playerHealthScript.TakeEnvironDmg(1);
        } else if (other.gameObject.tag == "Enemy") {
            EnemyController enemyScript = other.gameObject.GetComponent<EnemyController>();
            if (!enemyScript.HasDied()) {
                enemyScript.TakeDmg(100);
            }
        }
    }
}
