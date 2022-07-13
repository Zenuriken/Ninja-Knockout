using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlertedSight : MonoBehaviour
{
    private bool playerIsInThrowingRange;
    
    private void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.tag == "Player") {
            playerIsInThrowingRange = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other) {
        if (other.gameObject.tag == "Player") {
            playerIsInThrowingRange = false;
        }
    }

    // Returns whether the player is in contact with the enemy's alerted sight.
    public bool IsTouchingAlertedTrigger() {
        return playerIsInThrowingRange;
    }
}
