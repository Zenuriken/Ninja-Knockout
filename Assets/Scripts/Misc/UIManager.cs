using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public static UIManager singleton;
    public static bool detectionAllowed;
    public static bool tutorialEnabled;
    private static Vector2 spawnLocation;
    private static int spawnHealth;
    private static int spawnShurikens;

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

    private bool isBlackingOutScreen;
    private bool isShowingTutorial;
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
    public void IncreaseScoreBy(int amount) {
        score += amount;
    }

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

    public void FadeScreen() {
        if (!isBlackingOutScreen) {
            StartCoroutine("BlackOut");
        }
    }

    public void FadeOutScreen() {
        StartCoroutine("FadeOut");
    }

    public void FadeInScreen() {
        StartCoroutine("FadeIn");
    }

    public void SetSpawnLocation(Vector2 location) {
        spawnLocation = location;
        spawnHealth = health;
        spawnShurikens = shurikensRemaining;
    }

    public void PlayerDetected() {
        if (!detectionAllowed && health > 0) {
            StartCoroutine("DetectionScreen");
        }
    }

    public void DropBars(bool state) {
        blackBars.SetActive(state);
    }

    public void HidePlayerStatus(bool state) {
        playerStatus.SetActive(!state);
    }

    public void SetDetectionAllowed(bool state) {
        detectionAllowed = state;
    }

    public void ShowTutorialPopUp(string name) {
        isShowingTutorial = true;
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
        } else {
            currTutorial.texture = null;
            currTutorial.color = new Color(0f, 0f, 0f, 0f);
        }
        currTutorial.color = new Color(1f, 1f, 1f, 1f);
        tutorialBackground.SetActive(true);
        Time.timeScale = 0f;
    }

    public void RemoveTutorialPopUp() {
        if(isShowingTutorial) {
            currTutorial.texture = null;
            currTutorial.color = new Color(0f, 0f, 0f, 0f);
            tutorialBackground.SetActive(false);
            isShowingTutorial = false;
            Time.timeScale = 1f;
        }
    }

    public void ShowTutorialPrompt(bool state) {
        tutorialPrompt.SetActive(state); 
        tutorialBackground.SetActive(state);
        title.SetActive(state);
    }

    public void SetTutorialEnabled(bool state) {
        tutorialEnabled = state;
    }

    public bool TutorialIsEnabled() {
        return tutorialEnabled;
    }

    public void DisableTitleButtons() {
        titleButtons.SetActive(false);
    }
    #endregion

    #region Coroutine Functions
    IEnumerator FadeOut() {
        float speed = deathFadeAwaySpeed;
        for (float alpha = 0f; alpha < 1f; alpha += Time.deltaTime * speed) {
            fadeOutScreenImg.color = new Color(0f, 0f, 0f, alpha);
            yield return new WaitForEndOfFrame();
        }
        fadeOutScreenImg.color = new Color(0f, 0f, 0f, 1f);
    }

    IEnumerator FadeIn() {
        float speed = deathFadeAwaySpeed;
        yield return new WaitForSeconds(fadeAwayDelay);
            for (float alpha = 1f; alpha > 0f; alpha -= Time.deltaTime * speed) {
                fadeOutScreenImg.color = new Color(0f, 0f, 0f, alpha);
                yield return new WaitForEndOfFrame();
            }
        fadeOutScreenImg.color = new Color(0f, 0f, 0f, 0f);
    }

    // Fades the screen out to black.
    IEnumerator BlackOut() {
        isBlackingOutScreen = true;
        float speed;
        if (health <= 0) {
            speed = deathFadeAwaySpeed;
        } else {
            speed = fadeAwaySpeed;
        }
        yield return new WaitForSeconds(fadeAwayDelay);
        for (float alpha = 0f; alpha < 1f; alpha += Time.deltaTime * speed) {
            fadeOutScreenImg.color = new Color(0f, 0f, 0f, alpha);
            yield return new WaitForEndOfFrame();
        }
        fadeOutScreenImg.color = new Color(0f, 0f, 0f, 1f);

        // If the player is still alive, bring them to the last checkpoint. If not, then restart level.
        PlayerController playerScript = PlayerController.singleton;
        if (health > 0) {
            playerScript.SetPlayerInput(false);
            Rigidbody2D playerRB = playerScript.GetComponent<Rigidbody2D>();
            playerRB.velocity = new Vector2(0f, playerRB.velocity.y);
            playerScript.transform.position = spawnLocation;
            PlayerController.singleton.ResetAlertedNum();
            yield return new WaitForSeconds(fadeAwayDelay);
            for (float alpha = 1f; alpha > 0f; alpha -= Time.deltaTime * speed) {
                fadeOutScreenImg.color = new Color(0f, 0f, 0f, alpha);
                yield return new WaitForEndOfFrame();
            }
            fadeOutScreenImg.color = new Color(0f, 0f, 0f, 0f);
        } else {
            Debug.Log("Level reset");
            PlayerController.singleton.SetPlayerInput(false);
            Rigidbody2D playerRB = PlayerController.singleton.GetComponent<Rigidbody2D>();
            playerRB.velocity = new Vector2(0f, playerRB.velocity.y);
            PlayerController.singleton.Reset();
            UIManager.singleton.UpdateShurikenNum(PlayerController.singleton.GetNumShurikens());
            Health healthScript = PlayerController.singleton.GetComponent<Health>();
            healthScript.ResetHealth();
            PlayerController.singleton.ResetAlertedNum();
            SceneManager.LoadScene("Tutorial");
            yield return new WaitForSeconds(fadeAwayDelay);
            for (float alpha = 1f; alpha > 0f; alpha -= Time.deltaTime * speed) {
                fadeOutScreenImg.color = new Color(0f, 0f, 0f, alpha);
                yield return new WaitForEndOfFrame();
            }
            fadeOutScreenImg.color = new Color(0f, 0f, 0f, 0f);

        }
        isBlackingOutScreen = false;
        playerScript.SetPlayerInput(true);
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
        PlayerController.singleton.SetPlayerInput(false);
        Rigidbody2D playerRB = PlayerController.singleton.GetComponent<Rigidbody2D>();
        playerRB.velocity = new Vector2(0f, playerRB.velocity.y);
        PlayerController.singleton.transform.position = spawnLocation;
        PlayerController.singleton.SetNumShurikens(spawnShurikens);
        Health healthScript = PlayerController.singleton.GetComponent<Health>();
        healthScript.SetPlayerHealth(spawnHealth);
        PlayerController.singleton.ResetAlertedNum();
        SceneManager.LoadScene("Tutorial");
        // Fade out the detection screen
        yield return new WaitForSeconds(detectionScreenDelay * 2f);
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
