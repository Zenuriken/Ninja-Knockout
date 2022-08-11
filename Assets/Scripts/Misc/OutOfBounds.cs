using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutOfBounds : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.tag == "Player") {
            Health playerHealthScript = other.gameObject.GetComponent<Health>();
            playerHealthScript.TakeEnvironDmg(1);
        }
    }
}
