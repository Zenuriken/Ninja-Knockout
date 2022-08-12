using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager singleton;
    public static Vector2 spawnLocation;
    public static bool detectionAllowed;
    private static int health;
    private static int shurikensRemaining;
    private static int score;

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

    private void Awake() {
        if (singleton != null && singleton != this) { 
            Destroy(this.gameObject); 
        } 
        else { 
            singleton = this;
            DontDestroyOnLoad(this.gameObject);
            curHealthSprite = GameObject.Find("LifeUI").GetComponent<RawImage>();
            curShurikenSprite = GameObject.Find("ShurikenUI").GetComponent<RawImage>();
            fadeOutScreen = GameObject.Find("FadeOutScreen").GetComponent<Image>();
            detectedScreen = GameObject.Find("DetectedScreen").GetComponent<Image>();
            detectedTxt = detectedScreen.transform.GetChild(0).GetComponent<TMP_Text>();
        }
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
            curShurikenSprite.texture = shurikenList[shurikensRemaining - 1];
        }
    }

    public void UpdateHealth(int newHealth) {
        health = newHealth;
        if (health <= 0) {
            curHealthSprite.texture = null;
            curHealthSprite.color = new Color(0f, 0f, 0f, 0f);
        } else if (health > 0) {
            curHealthSprite.texture = healthList[health - 1];
        }
    }

    public void FadeScreen() {
        if (!isBlackingOutScreen) {
            StartCoroutine("BlackOut");
        }
    }

    public void SetSpawnLocation(Vector2 location) {
        spawnLocation = location;
        Debug.Log("Setting SpawnLocation: " + spawnLocation);
    }

    public void PlayerDetected() {
        if (!detectionAllowed) {
            StartCoroutine("DetectionScreen");
        }
    }
    #endregion

    #region Coroutine Functions
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
        PlayerController playerScript = GameObject.Find("Player").GetComponent<PlayerController>();
        if (health > 0) {
            playerScript.SetPlayerInput(false);
            Rigidbody2D playerRB = playerScript.GetComponent<Rigidbody2D>();
            playerRB.velocity = new Vector2(0f, playerRB.velocity.y);
            playerScript.transform.position = spawnLocation;
            yield return new WaitForSeconds(fadeAwayDelay);
            for (float alpha = 1f; alpha > 0f; alpha -= Time.deltaTime * speed) {
                fadeOutScreen.color = new Color(0f, 0f, 0f, alpha);
                yield return new WaitForEndOfFrame();
            }
            fadeOutScreen.color = new Color(0f, 0f, 0f, 0f);
        } else {
            SceneManager.LoadScene("Tutorial");
        }
        isBlackingOutScreen = false;
        playerScript.SetPlayerInput(true);
    }

    // Fades in the detection screen.
    IEnumerator DetectionScreen() {
        hasDetectionScreen = true;
        Time.timeScale = 0.25f;
        yield return new WaitForSeconds(detectionScreenDelay);
        for (float alpha = 0f; alpha <= 1f; alpha += Time.deltaTime * detectionScreenSpeed) {
            detectedScreen.color = new Color(0.2f, 0f, 0f, alpha);
            detectedTxt.alpha = alpha;
            yield return new WaitForEndOfFrame();
        }
        detectedScreen.color = new Color(0.2f, 0f, 0f, 1f);
        detectedTxt.alpha = 1f;
        
        SceneManager.LoadScene("Tutorial");
        PlayerController playerScript = GameObject.Find("Player").GetComponent<PlayerController>();
        playerScript.SetPlayerInput(false);
        Rigidbody2D playerRB = playerScript.GetComponent<Rigidbody2D>();
        playerRB.velocity = new Vector2(0f, playerRB.velocity.y);
        Debug.Log("SpawnLocation: " + spawnLocation);
        yield return new WaitForSeconds(detectionScreenDelay);
        playerScript.transform.position = spawnLocation;
        detectedScreen.color = new Color(0.2f, 0f, 0f, 0f);
        detectedTxt.alpha = 0f;
        Time.timeScale = 1f;
    }
    #endregion
}
