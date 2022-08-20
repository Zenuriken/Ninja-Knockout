using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FinishLine : MonoBehaviour
{
    private bool hasActivated;

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.tag == "Player" && !hasActivated) {
            PlayerController.singleton.SetTitleScreenMode(true);
            PlayerController.singleton.SetPlayerInput(false);
            //SceneManager.LoadScene("TitleScreen");
            UIManager.singleton.HidePlayerStatus(true);
            UIManager.singleton.DropBars(true);
            CameraController.singleton.SetFollowEnabled(false);
            CameraController.singleton.StartCoroutine("SwitchTime");
            hasActivated = true;

        }
    }
}
