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
    private int score;

    #region UI Lists
    [Header("UI Lists")]
    [SerializeField]
    [Tooltip("List of textures for health UI.")]
    private List<Texture> healthList;
    [SerializeField]
    [Tooltip("List of textures for shuriken UI.")]
    private List<Texture> shurikenList;
    [SerializeField]
    [Tooltip("List of textures for Tutorial Popup.")]
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
    private GameObject fadeOutScreen;
    [SerializeField]
    private GameObject detectionScreen;
    [Space(5)]
    #endregion

    #region Screen Variables
    [Header("Screen Fade Properties")]
    [SerializeField]
    [Tooltip("How fast the screen fades away when damaged.")]
    private float fadeAwaySpeed;
    [SerializeField]
    [Tooltip("How fast the screen fades when dying.")]
    private float deathFadeAwaySpeed;
    [SerializeField]
    [Tooltip("The delay before fading the screen.")]
    private float fadeAwayDelay;
    [SerializeField]
    [Tooltip("How fast the detection screen appears")]
    private float detectionScreenSpeed;
    [SerializeField]
    [Tooltip("The delay before the detection screen appears")]
    private float detectionScreenDelay;
    [Space(5)]
    #endregion

    private bool isFading;
    private bool isBlackingOutScreen;
    private bool isShowingTutorialPopUp;
    private bool isShowingTutorialPrompt;
    private bool hasDetectionScreen;
    private RawImage currHealthSprite;
    private RawImage currShurikenSprite;
    private RawImage currTutorial;
    private Image fadeOutScreenImg;
    private Image detectedScreenImg;
    private TMP_Text detectedTxt;

    private void Awake() {
        if (singleton != null && singleton != this) { 
            Destroy(this.gameObject); 
        } 
        else { 
            singleton = this;
            DontDestroyOnLoad(this.gameObject);
            currHealthSprite = playerStatus.transform.GetChild(2).GetComponent<RawImage>();
            currShurikenSprite = playerStatus.transform.GetChild(4).GetComponent<RawImage>();
            currTutorial = tutorialPopUp.GetComponent<RawImage>();
            fadeOutScreenImg = fadeOutScreen.GetComponent<Image>();
            detectedScreenImg = detectionScreen.GetComponent<Image>();
            detectedTxt = detectionScreen.transform.GetChild(0).GetComponent<TMP_Text>();
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
        } else if (sceneName == "Tutorial") {
            playerStatus.SetActive(true);
            titleButtons.SetActive(false);
            title.SetActive(false);
        }
        currTutorial.texture = null;
        currTutorial.color = new Color(0f, 0f, 0f, 0f);
        tutorialBackground.SetActive(false);
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
    public void IncreaseScoreBy(int amount) {
        score += amount;
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
        }
        currTutorial.color = new Color(1f, 1f, 1f, 1f);
        tutorialBackground.SetActive(true);
        Time.timeScale = 0f;
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
        isShowingTutorialPopUp = false;
        Time.timeScale = 1f;
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
        }
    }

    IEnumerator FadeIn() {
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
    }
    #endregion
}
