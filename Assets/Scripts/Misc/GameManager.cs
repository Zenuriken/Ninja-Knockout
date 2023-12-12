using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager singleton;
    private static Dictionary<string, string> lvlToMusic;

    #region Game Variables
    [Header("Game Variables")]
    [SerializeField][Tooltip("Whether tutorial popups are enabled")]
    private bool tutorialEnabled;
    [SerializeField][Tooltip("Whether a detection will reset the player to the last checkpoint.")]
    public bool detectionAllowed;
    [Space(5)]
    #endregion

    #region Components
    [Header("Components")]
    [SerializeField][Tooltip("The black bars for cutscenes")]
    private GameObject blackBars;
    [SerializeField][Tooltip("The screen fade transition")]
    private Image fadeImg;
    [SerializeField][Tooltip("The detection screen")]
    private Image detectedScreenImg;
    [SerializeField][Tooltip("The text displaying detection")]
    private TMP_Text detectedTxt;
    [Space(5)]
    #endregion

    #region Screen Variables
    [Header("Screen Fade Properties")]
    [SerializeField][Tooltip("How fast the screen fades when dying.")]
    private float deathFadeAwaySpeed;

    [SerializeField][Tooltip("The delay before fading the screen.")]
    private float fadeAwayDelay;
    [SerializeField][Tooltip("How fast the detection screen appears")]
    private float detectionScreenSpeed;
    [SerializeField][Tooltip("The delay before the detection screen appears")]
    private float detectionScreenDelay;
    [Space(5)]
    #endregion

    private GameObject campFires;
    private GameObject enemies;
    private GameObject breakables;
    private List<string> shownTutorials; 
    private Dictionary<string, bool> campFireDict;
    private bool isFading;
    private bool hasDetectionScreen;

    // State variables
    private int gold;
    private int totalEnemies;
    private int enemiesKilled;
    private int totalSupplies;
    private int suppliesLooted;
    private int numDetectionsAllowed;
    private int numDetections;

    private float inGameTime;
    private bool timerActive;
    private bool showTimer;

    private int lvlNum;

    private void Awake() {
        if (singleton != null && singleton != this) { 
            Destroy(this.gameObject); 
        } else { 
            singleton = this;
            DontDestroyOnLoad(this.gameObject);
            InitializeMapping();
            campFireDict = new Dictionary<string, bool>();
            shownTutorials = new List<string>();
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
            DefineStates();
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

            numDetectionsAllowed = 3;
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

    private void FixedUpdate() {
        if (!timerActive) return;
        inGameTime += Time.deltaTime;
        if (!showTimer) return;
        string time = CalculateTime(inGameTime);
        LevelUI.singleton.UpdateTimer(time);
    }

    public string CalculateTime(float timeInSeconds) {
        int hours = Mathf.FloorToInt(timeInSeconds / 3600);
        int minutes = Mathf.FloorToInt(timeInSeconds % 3600 / 60);
        int seconds = Mathf.FloorToInt(timeInSeconds % 60);

        // Format the time as hours:minutes:seconds
        string formattedTime = string.Format("{0:D2}:{1:D2}:{2:D2}", hours, minutes, seconds);

        return formattedTime;
    }

    public void ShowTimer(bool state) {
        showTimer = state;
        PlayerPrefs.SetInt("showTimer", state ? 1 : 0);
        Debug.Log("Show timer: " + showTimer);
    }
    
    public void SetVolume(float vol) {
        MusicManager.singleton.AdjustMusicVolume();
        SoundManager[] allSounds = FindObjectsOfType<SoundManager>();
        foreach (SoundManager manager in allSounds) {
            manager.AdjustSoundVolume();
        }
        PlayerPrefs.SetFloat("volume", vol);
    }

    private void DefineStates() {
        if (!PlayerPrefs.HasKey("coins")) PlayerPrefs.SetInt("coins", 0);
        if (!PlayerPrefs.HasKey("currLvl")) PlayerPrefs.SetInt("currLvl", 0);
        if (!PlayerPrefs.HasKey("volume")) PlayerPrefs.SetFloat("volume", 1f);
        if (!PlayerPrefs.HasKey("showTimer")) PlayerPrefs.SetInt("showTimer", 0);

        if (!PlayerPrefs.HasKey("lvl0_stars")) PlayerPrefs.SetInt("lvl0_stars", 0);
        if (!PlayerPrefs.HasKey("lvl0_time")) PlayerPrefs.SetFloat("lvl0_time", 0f);

        if (!PlayerPrefs.HasKey("lvl1_stars")) PlayerPrefs.SetInt("lvl1_stars", 0);
        if (!PlayerPrefs.HasKey("lvl1_time")) PlayerPrefs.SetFloat("lvl1_time", 0f);

        if (!PlayerPrefs.HasKey("lvl2_stars")) PlayerPrefs.SetInt("lvl2_stars", 0);
        if (!PlayerPrefs.HasKey("lvl2_time")) PlayerPrefs.SetFloat("lvl2_time", 0f);
    }

    public void DeleteAllStates() {
        PlayerPrefs.DeleteAll();
        StartCoroutine(LoadLevelCoroutine("TitleScreen"));
    }

    void InitializeMapping() {
        lvlToMusic = new Dictionary<string, string>();
        lvlToMusic["TitleScreen"] = "Adventure";
        lvlToMusic["Level0"] = "Traveler";
        lvlToMusic["Level1"] = "Traveler";
        lvlToMusic["Level2"] = "Traveler";
    }

    public void LoadLevel(string lvlName) {
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
        numDetections += 1;
    }

    // Sets the detectionAllowed variable to true.
    public void SetDetectionAllowed(bool state) {
        detectionAllowed = state;
    }

    // Activates the black movie bars UI for cutscenes.
    public void DropBars(bool state) {
        blackBars.SetActive(state);
    }

    // Adds this tutorial to the list of tutorials that have been shown.
    public void AddTutorial(string name) {
        shownTutorials.Add(name);
    }

    // Returns whether the tutorialImgNumber is less than or greater than the tutorial popup that wants to show
    public bool ShouldShow(string name) {
        return !shownTutorials.Contains(name);
    }

    // Starts the in game timer.
    public void StartTimer() {
        timerActive = true;
        inGameTime = 0f;
    }

    // Stops the in game timer.
    public void StopTimer() {
        timerActive = false;
    }

    // Calculates the stats to display at the end of the level.
    public void CalculateStats() {
        enemiesKilled = totalEnemies - enemies.transform.childCount;
        suppliesLooted = totalSupplies - breakables.transform.childCount;
    }

    // Stores the relevants stats
    public void SaveStats(int numSatisfiedStats) {
        if (PlayerPrefs.GetInt("currLvl") == lvlNum) {
            PlayerPrefs.SetInt("currLvl", lvlNum + 1);
        }
        if (numSatisfiedStats > PlayerPrefs.GetInt($"lvl{lvlNum}_stars")) {
            PlayerPrefs.SetInt($"lvl{lvlNum}_stars", numSatisfiedStats);
        }
        if (inGameTime < PlayerPrefs.GetFloat($"lvl{lvlNum}_time")) {
            PlayerPrefs.SetFloat($"lvl{lvlNum}_time", inGameTime);
        }
    }

    IEnumerator LoadLevelCoroutine(string lvlName) {
        GameObject canvas = GameObject.FindGameObjectWithTag("Canvas");
        canvas.SetActive(false);
    
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        MusicManager.singleton.FadeOutAudio();
        yield return StartCoroutine("FadeOut");
        // PlayerController.singleton.Reset(true);
        SceneManager.LoadScene(lvlName);
        MusicManager.singleton.Stop();
        MusicManager.singleton.FadeInAudio(lvlToMusic[lvlName]);
        yield return StartCoroutine("FadeIn");

        if (lvlName == "TitleScreen") {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        } else {
            InputManager.singleton.PlayerInputEnabled = true;
            lvlNum = int.Parse(lvlName.Substring(lvlName.Length - 1));
            StartTimer();
        }
    }

    //////////////////////////

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
        UpdateCampFireList();

        // TODO: Instead of loading the screen I should reset the player and have some sort of mapping to keep track of 
        // what enemies have been killed already.
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);


        // Fade out the detection screen
        yield return new WaitForSeconds(detectionScreenDelay * 2.5f);
        for (float alpha = 1f; alpha > 0f; alpha -= Time.deltaTime * detectionScreenSpeed) {
            detectedScreenImg.color = new Color(0.2f, 0f, 0f, alpha);
            detectedTxt.alpha = alpha;
            yield return new WaitForEndOfFrame();
        }
        detectedScreenImg.color = new Color(0f, 0f, 0f, 0f);
        detectedTxt.alpha = 0f;
        InputManager.singleton.PlayerInputEnabled = true;
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
    
    public int TotalEnemies {get{return totalEnemies;}}
    public int EnemiesKilled {get{return enemiesKilled;}}

    public int TotalSupplies {get{return totalSupplies;}}
    public int SuppliesLooted {get{return suppliesLooted;}}

    public int NumDetectionsAllowed {get{return numDetectionsAllowed;}}
    public int NumDetections {get{return numDetections;}}

    public int Gold {get{return gold;}}
    public float InGameTime {get{return inGameTime;}}
}
