using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    public static SceneController singleton;
    
    private void Awake() {
        if (singleton != null && singleton != this) { 
            Destroy(this.gameObject); 
        } 
        else { 
            singleton = this;
            DontDestroyOnLoad(this.gameObject);
        }
    }

    public void LoadTutorialScene() {
        StartCoroutine("LoadInTutorial");
    }

    public void QuitGame() {
        Application.Quit();
    }


    IEnumerator LoadInTutorial() {
        MusicManager.singleton.FadeOutAudio("Adventure");
        yield return UIManager.singleton.StartCoroutine("FadeOut");
        SceneManager.LoadScene("Tutorial");
        MusicManager.singleton.Stop("Adventure");
        MusicManager.singleton.FadeInAudio("Traveler");
        PlayerController.singleton.Reset();
        UIManager.singleton.StartCoroutine("FadeIn");
    }
}
