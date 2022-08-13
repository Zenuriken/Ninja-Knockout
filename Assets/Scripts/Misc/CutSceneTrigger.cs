using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutSceneTrigger : MonoBehaviour
{
    [SerializeField]
    [Tooltip("How long the player will be paused for")]
    private float pauseTime;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    private void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.tag == "Player") {
            PlayerController.singleton.SetPlayerInput(false);
            UIManager.singleton.HidePlayerStatus(true);
            UIManager.singleton.DropBars(true);
        }
        StartCoroutine("UnpausePlayer");
    }

    IEnumerator UnpausePlayer() {
        yield return new WaitForSeconds(pauseTime);
        PlayerController.singleton.SetPlayerInput(true);
        UIManager.singleton.DropBars(false);
        UIManager.singleton.HidePlayerStatus(false);
    }
}
