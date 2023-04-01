using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public static UIManager singleton;
    public bool detectionAllowed;
    public bool tutorialEnabled;

    private int health;
    private int shurikensRemaining;
    private int gold;
    private int lastTutorialPopUp;
    private List<string> shownTutorials; 

    #region UI Lists
    [Header("UI Lists")]
    [SerializeField][Tooltip("List of textures for health UI.")]
    private List<Texture> healthList;
    [SerializeField][Tooltip("List of textures for shuriken UI.")]
    private List<Texture> shurikenList;
    [SerializeField][Tooltip("List of textures backgrounds for shuriken UI.")]
    private List<Texture> shurikenBackgroundList;
    [SerializeField][Tooltip("List of textures for Tutorial Popup.")]
    private List<Texture> tutorialPopUps;
    [Space(5)]
    #endregion

    #region UI Components
    [Header("UI Components")]
    [SerializeField]
    private GameObject playerStatus;
    [SerializeField]
    private GameObject blackBars;
    [SerializeField]
    private GameObject titleButtons;
    [SerializeField]
    private GameObject title;
    [SerializeField]
    private GameObject tutorialPrompt;
    [SerializeField]
    private GameObject tutorialBackground;
    [SerializeField]
    private GameObject tutorialPopUp;
    [SerializeField]
    private GameObject enterPrompt;
    [SerializeField]
    private GameObject fadeOutScreen;
    [SerializeField]
    private GameObject detectionScreen;
    [SerializeField]
    private GameObject UISounds;
    [SerializeField]
    private GameObject dialogue;
    [Space(5)]
    #endregion

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
    [Space(5)]
    #endregion

    // #region Level Variables
    // [Header("Level Variables")]
    // [SerializeField]
    // [Tooltip("Number of enemies in this level")]
    // private float 

    private bool isFading;
    private bool isBlackingOutScreen;
    private bool isShowingTutorialPopUp;
    private bool isShowingTutorialPrompt;
    private bool hasDetectionScreen;
    private RawImage currHealthSprite;
    private RawImage currShurikenBackgroundSprite;
    private RawImage currShurikenSprite;
    private RawImage currTutorial;
    private RawImage goldSprite;
    private Image fadeOutScreenImg;
    private Image detectedScreenImg;
    private TMP_Text detectedTxt;
    private TMP_Text goldTxt;
    private float pickUpTime;

    private SoundManager sounds;

    private GameObject enemies;
    private GameObject breakables;
    private int totalEnemies;
    private int enemiesKilled;
    private int totalSupplies;
    private int suppliesLooted;

    private void Awake() {
        if (singleton != null && singleton != this) { 
            Destroy(this.gameObject); 
        } 
        else { 
            singleton = this;
            DontDestroyOnLoad(this.gameObject);
            currHealthSprite = playerStatus.transform.GetChild(2).GetComponent<RawImage>();
            currShurikenBackgroundSprite = playerStatus.transform.GetChild(3).GetComponent<RawImage>();
            currShurikenSprite = playerStatus.transform.GetChild(4).GetComponent<RawImage>();
            currTutorial = tutorialPopUp.GetComponent<RawImage>();
            fadeOutScreenImg = fadeOutScreen.GetComponent<Image>();
            detectedScreenImg = detectionScreen.GetComponent<Image>();
            detectedTxt = detectionScreen.transform.GetChild(0).GetComponent<TMP_Text>();

            goldSprite = playerStatus.transform.GetChild(5).GetComponent<RawImage>();
            goldTxt = goldSprite.transform.GetChild(0).GetComponent<TMP_Text>();

            sounds = UISounds.GetComponent<SoundManager>();
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
            playerStatus.SetActive(false);
            titleButtons.SetActive(true);
            title.SetActive(true);
            Button yesTutorialBttn = tutorialPrompt.transform.GetChild(1).GetComponent<Button>();
            Button noTutorialBttn = tutorialPrompt.transform.GetChild(2).GetComponent<Button>();
            yesTutorialBttn.onClick.AddListener(SceneController.singleton.LoadTutorialScene);
            noTutorialBttn.onClick.AddListener(SceneController.singleton.LoadTutorialScene);
        } else if (sceneName == "Tutorial") {
            playerStatus.SetActive(true);
            titleButtons.SetActive(false);
            title.SetActive(false);

            enemies = GameObject.Find("Enemies");
            breakables = GameObject.Find("Breakables");

            totalEnemies = enemies.transform.childCount;
            totalSupplies = breakables.transform.childCount;
        }
        currTutorial.texture = null;
        currTutorial.color = new Color(0f, 0f, 0f, 0f);
        tutorialBackground.SetActive(false);
        enterPrompt.SetActive(false);
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
    
    #region Public Function
    // Increases the player's score by amount.
    public void UpdateGold(int amount) {
        pickUpTime = Time.time;
        goldTxt.alpha = 1f;
        goldSprite.color = new Color(1f, 1f, 1f, 1f);
        gold = amount;
        goldTxt.text = gold.ToString();
        StartCoroutine("FadeGoldUI");
    }

    // Initializes the background for the shuriken UI.
    public void InitializeShurikenBackground(int maxShurikens) {
        currShurikenBackgroundSprite.texture = shurikenBackgroundList[maxShurikens - 2];
    }


    // Updates the shurikens remaining UI.
    public void UpdateShurikenNum(int newNum) {
        shurikensRemaining = newNum;
        if (shurikensRemaining <= 0) {
            currShurikenSprite.texture = null;
            currShurikenSprite.color = new Color(0f, 0f, 0f, 0f);
        } else {
            currShurikenSprite.color = new Color(1f, 1f, 1f, 1f);
            currShurikenSprite.texture = shurikenList[shurikensRemaining - 1];
        }
    }

    // Updates the player's health UI.
    public void UpdateHealth(int newHealth) {
        health = newHealth;
        if (health <= 0) {
            currHealthSprite.texture = null;
            currHealthSprite.color = new Color(0f, 0f, 0f, 0f);
        } else if (health > 0) {
            currHealthSprite.color = new Color(1f, 1f, 1f, 1f);
            currHealthSprite.texture = healthList[health - 1];
        }
    }

    // Fades in detection screen if detection is not allowed and player has not died.
    public void PlayerDetected() {
        if (!detectionAllowed && health > 0) {
            StartCoroutine("DetectionScreen");
        }
    }

    // Activates the black movie bars UI for cutscenes.
    public void DropBars(bool state) {
        blackBars.SetActive(state);
    }

    // Hides the Player status UI.
    public void HidePlayerStatus(bool state) {
        playerStatus.SetActive(!state);
    }

    // Toggles whether enemy detection is allowed.
    public void SetDetectionAllowed(bool state) {
        detectionAllowed = state;
    }

    // Changes the texture displayed for the tutorial prompt.
    public void ShowTutorialPopUp(string name) {
        isShowingTutorialPopUp = true;
        if (name == "Move") {
            currTutorial.texture = tutorialPopUps[0];
        } else if (name == "Jump") {
            currTutorial.texture = tutorialPopUps[1];
        } else if (name == "DoubleJump") {
            currTutorial.texture = tutorialPopUps[2];
        } else if (name == "Melee") {
            currTutorial.texture = tutorialPopUps[3];
        } else if (name == "WallClimb") {
            currTutorial.texture = tutorialPopUps[4];
        } else if (name == "StealthKill") {
            currTutorial.texture = tutorialPopUps[5];
        } else if (name == "Fire") {
            currTutorial.texture = tutorialPopUps[6];
        } else if (name == "Aim") {
            currTutorial.texture = tutorialPopUps[7];
        } else if (name == "Sneak") {
            currTutorial.texture = tutorialPopUps[8];
        } else if (name == "Hide") {
            currTutorial.texture = tutorialPopUps[9];
        } else if (name == "CampFire") {
            currTutorial.texture = tutorialPopUps[10];
        }
        currTutorial.color = new Color(1f, 1f, 1f, 1f);
        tutorialBackground.SetActive(true);
        enterPrompt.SetActive(true);
        Time.timeScale = 0f;
        shownTutorials.Add(name);
        PlayerController.singleton.SetPlayerInput(false);
        sounds.Play("TutorialPopUp");
    }

    // Exits whatever Pop Up or prompt is being displayed.
    public void ExitPopUp() {
        if (isShowingTutorialPopUp) {
            RemoveTutorialPopUp();
        } else if (isShowingTutorialPrompt) {
            ShowTutorialPrompt(false);
        }
    }

    // Removes the tutorial pop up if it's currently being displayed.
    public void RemoveTutorialPopUp() {
        currTutorial.texture = null;
        currTutorial.color = new Color(0f, 0f, 0f, 0f);
        tutorialBackground.SetActive(false);
        enterPrompt.SetActive(false);
        isShowingTutorialPopUp = false;
        Time.timeScale = 1f;
        PlayerController.singleton.SetPlayerInput(true);
    }

    // Shows the prompt asking if tutorial pop ups should be enabled.
    public void ShowTutorialPrompt(bool state) {
        isShowingTutorialPrompt = state;
        tutorialPrompt.SetActive(state); 
        tutorialBackground.SetActive(state);
    }

    // Sets the tutorialEnabled bool to the given state.
    public void SetTutorialEnabled(bool state) {
        tutorialEnabled = state;
    }

    // Sets the Title UI
    public void SetTitle(bool state) {
        title.SetActive(state);
    }

    // Returns whether the tutorial is enabled.
    public bool TutorialIsEnabled() {
        return tutorialEnabled;
    }

    // Disables the Title Buttons UI
    public void DisableTitleButtons() {
        titleButtons.SetActive(false);
    }

    // Returns whether the screen is currently fading.
    public bool IsFading() {
        return isFading;
    }

    // Returns whether the currTutorialNumber is less than or greater than the tutorial popup that wants to show
    public bool ShouldShow(string name) {
        return !shownTutorials.Contains(name);
    }

    // Returns whether the screen currently has detection screen.
    public bool HasDetectionScreen() {
        return hasDetectionScreen;
    }

    // Plays the dialogue at the end of the tutorial.
    public IEnumerator StartTutorialEndCinematic() {
        dialogue.SetActive(true);
        enemiesKilled = totalEnemies - enemies.transform.childCount;
        suppliesLooted = totalSupplies - breakables.transform.childCount;
        Dialogue dialogueScript = dialogue.GetComponent<Dialogue>();
        dialogueScript.InitializeVariables(enemiesKilled, suppliesLooted, enemies.transform.childCount);
        dialogueScript.InitializeDialogue();
        yield return dialogueScript.StartDialogue();
        dialogue.SetActive(false);
    }

    public void ClearFadeScreen() {
        fadeOutScreenImg.color = new Color(0f, 0f, 0f, 0f);
    }
    
    #endregion

    #region Coroutine Functions
    public IEnumerator FadeOut() {
        if(!isFading) {
            isFading = true;
            float speed = deathFadeAwaySpeed;
            for (float alpha = 0f; alpha < 1f; alpha += Time.deltaTime * speed) {
                fadeOutScreenImg.color = new Color(0f, 0f, 0f, alpha);
                yield return new WaitForEndOfFrame();
            }
            fadeOutScreenImg.color = new Color(0f, 0f, 0f, 1f);
            isFading = false;
        } else {
            Debug.Log("Currently fading");
        }
    }

    public IEnumerator FadeIn() {
        if(!isFading) {
            isFading = true;
            float speed = deathFadeAwaySpeed;
            yield return new WaitForSeconds(fadeAwayDelay);
            for (float alpha = 1f; alpha > 0f; alpha -= Time.deltaTime * speed) {
                fadeOutScreenImg.color = new Color(0f, 0f, 0f, alpha);
                yield return new WaitForEndOfFrame();
            }
            fadeOutScreenImg.color = new Color(0f, 0f, 0f, 0f);
            isFading = false;
        }
    }

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
        SceneController.singleton.UpdateCampFireList();
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

    // Fades the Gold UI after picking up gold.
    IEnumerator FadeGoldUI() {
        yield return new WaitForSeconds(fadeAwayDelay);
        if (pickUpTime > Time.time - fadeAwayDelay) {
            yield break;
        }
        for (float alpha = 1f; alpha > 0f; alpha -= Time.deltaTime * goldUIFadeSpeed) {
            if (pickUpTime > Time.time - fadeAwayDelay) {
                yield break;
            }
            goldSprite.color = new Color(1f, 1f, 1f, alpha);
            goldTxt.alpha = alpha;
            yield return new WaitForEndOfFrame();
        }
    }
    #endregion
}
