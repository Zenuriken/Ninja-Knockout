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
    private static Vector2 spawnLocation;
    private static int spawnHealth;
    private static int spawnShurikens;

    private int health;
    private int shurikensRemaining;
    private int score;

    [SerializeField]
    [Tooltip("List of textures for health UI.")]
    private List<Texture> healthList;
    [SerializeField]
    [Tooltip("List of textures for shuriken UI.")]
    private List<Texture> shurikenList;
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

    private bool isBlackingOutScreen;
    private bool hasDetectionScreen;
    private RawImage curHealthSprite;
    private RawImage curShurikenSprite;
    private Image fadeOutScreen;
    private Image detectedScreen;
    private TMP_Text detectedTxt;

    private GameObject blackBars;
    private GameObject playerStatus;
    // private GameObject playButton;
    // private GameObject optionButton;
    // private GameObject quitButton;
    private GameObject buttons;
    private GameObject title;

    private void Awake() {
        if (singleton != null && singleton != this) { 
            Destroy(this.gameObject); 
        } 
        else { 
            singleton = this;
            DontDestroyOnLoad(this.gameObject);

            playerStatus = this.transform.GetChild(0).gameObject;
            blackBars = this.transform.GetChild(1).gameObject;
            buttons = this.transform.GetChild(3).gameObject;
            title = this.transform.GetChild(4).gameObject;
            curHealthSprite = playerStatus.transform.GetChild(2).GetComponent<RawImage>();
            curShurikenSprite = playerStatus.transform.GetChild(3).GetComponent<RawImage>();
            fadeOutScreen = this.transform.GetChild(1).GetComponent<Image>();
            detectedScreen = this.transform.GetChild(2).GetComponent<Image>();
            detectedTxt = detectedScreen.transform.GetChild(0).GetComponent<TMP_Text>();
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
            buttons.SetActive(true);
            title.SetActive(true);
        } else if (sceneName == "Tutorial") {
            playerStatus.SetActive(true);
            buttons.SetActive(false);
            title.SetActive(false);
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
    
    #region Public Function
    public void IncreaseScoreBy(int amount) {
        score += amount;
    }

    public void UpdateShurikenNum(int newNum) {
        shurikensRemaining = newNum;
        if (shurikensRemaining <= 0) {
            curShurikenSprite.texture = null;
            curShurikenSprite.color = new Color(0f, 0f, 0f, 0f);
        } else {
            curShurikenSprite.color = new Color(1f, 1f, 1f, 1f);
            curShurikenSprite.texture = shurikenList[shurikensRemaining - 1];
        }
    }

    public void UpdateHealth(int newHealth) {
        health = newHealth;
        if (health <= 0) {
            curHealthSprite.texture = null;
            curHealthSprite.color = new Color(0f, 0f, 0f, 0f);
        } else if (health > 0) {
            curHealthSprite.color = new Color(1f, 1f, 1f, 1f);
            curHealthSprite.texture = healthList[health - 1];
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
    #endregion

    #region Coroutine Functions
    IEnumerator FadeOut() {
        float speed = deathFadeAwaySpeed;
        for (float alpha = 0f; alpha < 1f; alpha += Time.deltaTime * speed) {
            fadeOutScreen.color = new Color(0f, 0f, 0f, alpha);
            yield return new WaitForEndOfFrame();
        }
        fadeOutScreen.color = new Color(0f, 0f, 0f, 1f);
    }

    IEnumerator FadeIn() {
        float speed = deathFadeAwaySpeed;
        yield return new WaitForSeconds(fadeAwayDelay);
            for (float alpha = 1f; alpha > 0f; alpha -= Time.deltaTime * speed) {
                fadeOutScreen.color = new Color(0f, 0f, 0f, alpha);
                yield return new WaitForEndOfFrame();
            }
        fadeOutScreen.color = new Color(0f, 0f, 0f, 0f);
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
            fadeOutScreen.color = new Color(0f, 0f, 0f, alpha);
            yield return new WaitForEndOfFrame();
        }
        fadeOutScreen.color = new Color(0f, 0f, 0f, 1f);

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
                fadeOutScreen.color = new Color(0f, 0f, 0f, alpha);
                yield return new WaitForEndOfFrame();
            }
            fadeOutScreen.color = new Color(0f, 0f, 0f, 0f);
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
                fadeOutScreen.color = new Color(0f, 0f, 0f, alpha);
                yield return new WaitForEndOfFrame();
            }
            fadeOutScreen.color = new Color(0f, 0f, 0f, 0f);

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
            detectedScreen.color = new Color(0.2f, 0f, 0f, alpha);
            detectedTxt.alpha = alpha;
            yield return new WaitForEndOfFrame();
        }
        detectedScreen.color = new Color(0.2f, 0f, 0f, 1f);
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
            detectedScreen.color = new Color(0.2f, 0f, 0f, alpha);
            detectedTxt.alpha = alpha;
            yield return new WaitForEndOfFrame();
        }
        detectedScreen.color = new Color(0f, 0f, 0f, 0f);
        detectedTxt.alpha = 0f;
        PlayerController.singleton.SetPlayerInput(true);
        Time.timeScale = 1f;
    }
    #endregion
}
