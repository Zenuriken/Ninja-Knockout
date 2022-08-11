using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hazard : MonoBehaviour
{
    Health healthScript;

    private void Awake() {
        healthScript = GameObject.Find("Player").GetComponent<Health>();
    }
    
    private void OnCollisionEnter2D(Collision2D other) {
        if (other.gameObject.tag == "Player") {
            healthScript.TakeEnvironDmg(1);
        }
    }
}
