using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FinishLine : MonoBehaviour
{
    private bool hasActivated;
    private CameraController cam;

    public GameObject titleScreenPlayer;

    private void Start() {
        cam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraController>();
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.tag == "Player" && !hasActivated) {
            hasActivated = true;
            PlayerController.singleton.gameObject.SetActive(false);
            GameObject titlePlayer = Instantiate(titleScreenPlayer, PlayerController.singleton.transform.position, Quaternion.identity);
            LevelUI.singleton.HidePlayerStatus(true);
            cam.SetFollowEnabled(false);
            StartCoroutine("EndingCinematic");
        }
    }

    IEnumerator EndingCinematic() {
        GameManager.singleton.DropBars(true);
        MusicManager.singleton.FadeOutAudio();
        yield return new WaitForSeconds(3f);
        GameManager.singleton.DropBars(true);
        //yield return cam.StartCoroutine("SwitchTime");
        yield return new WaitForSeconds(1f);
        yield return GameManager.singleton.FadeOut();
        yield return new WaitForSeconds(1f);
        GameManager.singleton.DropBars(false);
        yield return LevelUI.singleton.StartTutorialEndCinematic();
        yield return new WaitForSeconds(1f);
        //SceneController.singleton.LoadTitlescreen();
    }
}
