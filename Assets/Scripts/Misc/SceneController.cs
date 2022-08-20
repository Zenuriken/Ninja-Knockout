using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    public static SceneController singleton;

    [SerializeField]
    [Tooltip("List of campfires in the scene")]
    private GameObject campFires;

    private Dictionary<string, bool> campFireDict;
    
    private void Awake() {
        if (singleton != null && singleton != this) { 
            Destroy(this.gameObject); 
        } 
        else { 
            singleton = this;
            DontDestroyOnLoad(this.gameObject);
            campFireDict = new Dictionary<string, bool>();
        }
    }

    // called first
    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    // called second
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        string sceneName = scene.name;
        if (sceneName == "TitleScreen") {
            
        } else if (sceneName == "Tutorial") {
            campFires = GameObject.Find("CampFires");
            if (campFireDict.Count > 0) {
                SetCampFires();
            } else {
                UpdateCampFireList();
            }
        }
        
    }

    // called third
    void Start()
    {
    }

    // called when the game is terminated
    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
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
        yield return UIManager.singleton.StartCoroutine("FadeIn");
        PlayerController.singleton.SetPlayerInput(true);
    }

    // Updates the camp fire dictionary with the activation status of all the campfires.
    public void UpdateCampFireList() {
        foreach (Transform child in campFires.transform) {
            campFireDict[child.name] = child.GetComponent<CampFire>().HasActivated();
        }
    }

    // Sets the activation status of all the campfires in the scene.
    public void SetCampFires() {
        foreach (Transform child in campFires.transform) {
            CampFire campFire = child.GetComponent<CampFire>();
            campFire.SetHasActivated(campFireDict[child.name]);
        }
    }
}
