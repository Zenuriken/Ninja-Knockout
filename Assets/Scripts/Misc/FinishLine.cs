using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FinishLine : MonoBehaviour
{
    private bool hasActivated;
    private CameraController cam;

    private void Start() {
        cam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraController>();
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.tag == "Player" && !hasActivated) {
            hasActivated = true;
            PlayerController.singleton.SetTitleScreenMode(true);
            PlayerController.singleton.SetPlayerInput(false);
            //SceneManager.LoadScene("TitleScreen");
            UIManager.singleton.HidePlayerStatus(true);
            cam.SetFollowEnabled(false);
            StartCoroutine("EndingCinematic");
        }
    }

    IEnumerator EndingCinematic() {
        MusicManager.singleton.FadeOutAudio("Traveler");
        yield return new WaitForSeconds(2f);
        MusicManager.singleton.Stop("Traveler");
        UIManager.singleton.DropBars(true);
        yield return cam.StartCoroutine("SwitchTime");
        yield return new WaitForSeconds(1f);
        yield return UIManager.singleton.FadeOut();
        yield return new WaitForSeconds(1f);
        UIManager.singleton.DropBars(false);
        yield return UIManager.singleton.StartTutorialEndCinematic();
        yield return new WaitForSeconds(1f);
        SceneController.singleton.LoadTitlescreen();
    }
}
