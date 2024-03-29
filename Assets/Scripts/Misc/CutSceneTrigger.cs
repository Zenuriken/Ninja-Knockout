using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutSceneTrigger : MonoBehaviour
{
    [SerializeField]
    [Tooltip("How long the player will be paused for")]
    private float pauseTime;
    [SerializeField]
    [Tooltip("The enemy that will appear")]
    private GameObject enemy;

    private bool hasTriggered;

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.tag == "Player" && !hasTriggered) {
            hasTriggered = true;
            InputManager.singleton.PlayerInputEnabled = false;
            LevelUI.singleton.HidePlayerStatus(true);
            GameManager.singleton.DropBars(true);
            // GameManager.singleton.SetDetectionAllowed(true);
            enemy.SetActive(true);
        }
        StartCoroutine("UnpausePlayer");
    }

    IEnumerator UnpausePlayer() {
        yield return new WaitForSeconds(pauseTime);
        InputManager.singleton.PlayerInputEnabled = true;
        GameManager.singleton.DropBars(false);
        LevelUI.singleton.HidePlayerStatus(false);
    }
}
