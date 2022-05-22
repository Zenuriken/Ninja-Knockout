using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cover : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other) {
        if (other.tag == "PlayerCenter") {
            //Debug.Log("hiding in bush");
            PlayerController playerScript= other.gameObject.GetComponentInParent<PlayerController>();
            playerScript.SetCoverStatus(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other) {
        if (other.tag == "PlayerCenter") {
            PlayerController playerScript= other.gameObject.GetComponentInParent<PlayerController>();
            playerScript.SetCoverStatus(false);
        }
    }
}
