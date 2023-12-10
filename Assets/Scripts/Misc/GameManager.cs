using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager singleton;
    private static Dictionary<string, string> lvl2Music;

    [SerializeField][Tooltip("Whether tutorial popups are enabled")]
    private bool tutorialEnabled;
    [SerializeField][Tooltip("Whether a detection will reset the player to the last checkpoint.")]
    public bool detectionAllowed;

    [SerializeField][Tooltip("List of campfires in the scene")]
    private GameObject campFires;
    [SerializeField][Tooltip("The screen fade transition")]
    private Image fadeImg;

    #region Screen Variables
    [Header("Screen Fade Properties")]
    [SerializeField][Tooltip("How fast the screen fades away when damaged.")]
    private float fadeAwaySpeed;
    [SerializeField][Tooltip("How fast the screen fades when dying.")]
    private float deathFadeAwaySpeed;
    [SerializeField][Tooltip("How fast the gold UI will fade")]
    private float goldUIFadeSpeed;
    [SerializeField][Tooltip("The delay before fading the screen.")]
    private float fadeAwayDelay;
    [SerializeField][Tooltip("How fast the detection screen appears")]
    private float detectionScreenSpeed;
    [SerializeField][Tooltip("The delay before the detection screen appears")]
    private float detectionScreenDelay;
    [SerializeField]
    private GameObject blackBars;
    [Space(5)]
    #endregion

    private Dictionary<string, bool> campFireDict;
    private bool isFading;
    private bool isBlackingOutScreen;
    private bool hasDetectionScreen;
    private Image fadeOutScreenImg;
    private Image detectedScreenImg;
    private TMP_Text detectedTxt;

    // State variables
    private int gold;
    private int totalEnemies;
    private int enemiesKilled;
    private int totalSupplies;
    private int suppliesLooted;

    private GameObject enemies;
    private GameObject breakables;

    private void Awake() {
        if (singleton != null && singleton != this) { 
            Destroy(this.gameObject); 
        } else { 
            singleton = this;
            DontDestroyOnLoad(this.gameObject);
            InitializeMapping();
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
            MusicManager.singleton.PlayAudio("Adventure");
        } else {
            campFires = GameObject.Find("CampFires");
            if (campFireDict.Count > 0) {
                SetCampFires();
            } else {
                UpdateCampFireList();
            }
            enemies = GameObject.Find("Enemies");
            breakables = GameObject.Find("Breakables");

            totalEnemies = enemies.transform.childCount;
            totalSupplies = breakables.transform.childCount;
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

    void InitializeMapping() {
        lvl2Music = new Dictionary<string, string>();
        lvl2Music["TitleScreen"] = "Adventure";
        lvl2Music["Level0"] = "ForestWalk";
        lvl2Music["Level1"] = "WayFarer";
    }

    public void LoadLevel(string lvlName) {
        Debug.Log("Loading level");
        StartCoroutine(LoadLevelCoroutine(lvlName));
    }

    public void QuitGame() {
        Application.Quit();
    }

    public void SetTutorialStatus(bool status) {
        tutorialEnabled = status;
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

    public void IncreaseGoldBy(int num) {
        gold += num;
        LevelUI.singleton.UpdateGold(gold);
    }


    // Fades in detection screen if detection is not allowed and player has not died.
    public void PlayerDetected() {
        if (!detectionAllowed && PlayerController.singleton.GetPlayerHealth() > 0) {
            StartCoroutine("DetectionScreen");
        }
    }

    // Activates the black movie bars UI for cutscenes.
    public void DropBars(bool state) {
        blackBars.SetActive(state);
    }


    // // Activates the black movie bars UI for cutscenes.
    // public void DropBars(bool state) {
    //     blackBars.SetActive(state);
    // }


    IEnumerator LoadLevelCoroutine(string lvlName) {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        // PlayerController.singleton.SetPlayerInput(false);
        MusicManager.singleton.FadeOutAudio();
        yield return StartCoroutine("FadeOut");
        // PlayerController.singleton.Reset(true);
        SceneManager.LoadScene(lvlName);
        MusicManager.singleton.Stop();
        MusicManager.singleton.FadeInAudio(lvl2Music[lvlName]);
        yield return StartCoroutine("FadeIn");
        PlayerController.singleton.SetPlayerInput(true);
    }

    //////////////////////////


    // IEnumerator LoadInTitleScreen() {
    //     SceneManager.LoadScene("TitleScreen");
    //     PlayerController.singleton.transform.position = Vector2.zero;
    //     MusicManager.singleton.FadeInAudio("Adventure");
    //     yield return UIManager.singleton.FadeIn();
    //     Cursor.lockState = CursorLockMode.None;
    //     Cursor.visible = true;
    // }


    // IEnumerator LoadInTutorial() {
    //     Cursor.lockState = CursorLockMode.Locked;
    //     Cursor.visible = false;
    //     MusicManager.singleton.FadeOutAudio("Adventure");
    //     yield return UIManager.singleton.StartCoroutine("FadeOut");
    //     SceneManager.LoadScene("Tutorial");
    //     MusicManager.singleton.Stop("Adventure");
    //     MusicManager.singleton.FadeInAudio("Traveler");
    //     PlayerController.singleton.Reset(true);
    //     yield return UIManager.singleton.StartCoroutine("FadeIn");
    //     PlayerController.singleton.SetPlayerInput(true);
    // }

    // Fades in the detection screen.
    IEnumerator DetectionScreen() {
        hasDetectionScreen = true;
        Time.timeScale = 0.25f;
        // Fade in the detection screen
        yield return new WaitForSeconds(detectionScreenDelay);
        for (float alpha = 0f; alpha <= 1f; alpha += Time.deltaTime * detectionScreenSpeed) {
            detectedScreenImg.color = new Color(0.2f, 0f, 0f, alpha);
            detectedTxt.alpha = alpha;
            yield return new WaitForEndOfFrame();
        }
        detectedScreenImg.color = new Color(0.2f, 0f, 0f, 1f);
        detectedTxt.alpha = 1f;

        // While screen is covered, reset the player to last spawn point.
        PlayerController.singleton.Respawn();
        GameManager.singleton.UpdateCampFireList();
        SceneManager.LoadScene("Tutorial");


        // Fade out the detection screen
        yield return new WaitForSeconds(detectionScreenDelay * 2.5f);
        for (float alpha = 1f; alpha > 0f; alpha -= Time.deltaTime * detectionScreenSpeed) {
            detectedScreenImg.color = new Color(0.2f, 0f, 0f, alpha);
            detectedTxt.alpha = alpha;
            yield return new WaitForEndOfFrame();
        }
        detectedScreenImg.color = new Color(0f, 0f, 0f, 0f);
        detectedTxt.alpha = 0f;
        PlayerController.singleton.SetPlayerInput(true);
        Time.timeScale = 1f;
        hasDetectionScreen = false;
    }

    // Returns whether the screen currently has detection screen.
    public bool HasDetectionScreen() {
        return hasDetectionScreen;
    }

    // Returns whether the tutorial is enabled.
    public bool TutorialIsEnabled() {
        return tutorialEnabled;
    }

     // Returns whether the screen is currently fading.
    public bool IsFading() {
        return isFading;
    }

    public int TotalEnemies {get{return totalEnemies;}}
    public int EnemiesKilled {get{return enemiesKilled;}}
    public int TotalSupplies {get{return totalSupplies;}}
    public int SuppliesLooted {get{return suppliesLooted;}}


    public IEnumerator FadeOut() {
        if(!isFading) {
            isFading = true;
            float speed = deathFadeAwaySpeed;
            for (float alpha = 0f; alpha < 1f; alpha += Time.deltaTime * speed) {
                fadeImg.color = new Color(0f, 0f, 0f, alpha);
                yield return new WaitForEndOfFrame();
            }
            fadeImg.color = new Color(0f, 0f, 0f, 1f);
            isFading = false;
        }
    }

    public IEnumerator FadeIn() {
        if(!isFading) {
            isFading = true;
            float speed = deathFadeAwaySpeed;
            yield return new WaitForSeconds(fadeAwayDelay);
            for (float alpha = 1f; alpha > 0f; alpha -= Time.deltaTime * speed) {
                fadeImg.color = new Color(0f, 0f, 0f, alpha);
                yield return new WaitForEndOfFrame();
            }
            fadeImg.color = new Color(0f, 0f, 0f, 0f);
            isFading = false;
        }
    }
}
