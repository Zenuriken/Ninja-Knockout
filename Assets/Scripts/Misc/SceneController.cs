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
        SceneManager.LoadScene("Tutorial");
    }

    public void QuitGame() {
        Application.Quit();
    }
}
