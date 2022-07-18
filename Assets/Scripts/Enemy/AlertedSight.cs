using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlertedSight : MonoBehaviour
{
    private PlayerController playerScript;
    private bool playerIsInThrowingRange;
    private float lastCollisionTime;
    
    private void Start() {
        playerScript = GameObject.Find("Player").GetComponent<PlayerController>();
    }


    private void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.tag == "Player") {
            playerIsInThrowingRange = true;
            playerScript.IncreaseAlertedNumBy(1);
        }
    }

    private void OnTriggerExit2D(Collider2D other) {
        if (other.gameObject.tag == "Player") {
            playerIsInThrowingRange = false;
            playerScript.IncreaseAlertedNumBy(-1);
        }
    }

    // Returns whether the player is in contact with the enemy's alerted sight.
    public bool IsTouchingAlertedTrigger() {
        return playerIsInThrowingRange;
    }
}
