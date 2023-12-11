using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LevelUI : MonoBehaviour
{
    public static LevelUI singleton;
    
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
    [SerializeField][Tooltip("The player's upper left")]
    private GameObject playerStatus;
    [SerializeField][Tooltip("Tutorial Popup GameObject")]
    private GameObject tutorialPopUp;
    [SerializeField][Tooltip("Dialogue GameObject")]
    private GameObject dialogue;
    [SerializeField][Tooltip("Level Stats GameObject")]
    private GameObject levelStats;
    [SerializeField]
    private GameObject UISounds;
    #endregion

    #region UI Variables
    [SerializeField][Tooltip("How fast the gold UI will fade")]
    private float goldUIFadeSpeed;
    [SerializeField][Tooltip("The delay before something starts fading.")]
    private float fadeAwayDelay;
    [SerializeField][Tooltip("How fast the tutorial popup fades in")]
    private float tutorialFadeSpeed;
    #endregion
   
    private SoundManager sounds;
    private RawImage currHealthSprite;
    private RawImage currShurikenBackgroundSprite;
    private RawImage currShurikenSprite;
    private RawImage tutorialImg;
    private Image enterPromptImg;
    private GameObject enterPrompt;
    private RawImage goldSprite;
    private TMP_Text goldTxt;

    private float pickUpTime;
    private int health;
    private int shurikensRemaining;
    private int gold;
    bool isShowingTutorialPopUp;
    bool isFadingInTutorialPopUp;

    private void Awake() {
        if (singleton != null && singleton != this) { 
            Destroy(this.gameObject); 
        } 
        else { 
            singleton = this;
            currHealthSprite = playerStatus.transform.GetChild(2).GetComponent<RawImage>();
            currShurikenBackgroundSprite = playerStatus.transform.GetChild(3).GetComponent<RawImage>();
            currShurikenSprite = playerStatus.transform.GetChild(4).GetComponent<RawImage>();
            tutorialImg = tutorialPopUp.transform.GetChild(0).GetComponent<RawImage>();
            enterPromptImg = tutorialPopUp.transform.GetChild(1).GetComponent<Image>();
            enterPrompt = tutorialPopUp.transform.GetChild(1).gameObject;

            goldSprite = playerStatus.transform.GetChild(5).GetComponent<RawImage>();
            goldTxt = goldSprite.transform.GetChild(0).GetComponent<TMP_Text>();

            sounds = UISounds.GetComponent<SoundManager>();
        }
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

    // Changes the texture displayed for the tutorial prompt.
    public void ShowTutorialPopUp(string name) {
        isShowingTutorialPopUp = true;
        if (name == "Move") {
            tutorialImg.texture = tutorialPopUps[0];
        } else if (name == "Jump") {
            tutorialImg.texture = tutorialPopUps[1];
        } else if (name == "DoubleJump") {
            tutorialImg.texture = tutorialPopUps[2];
        } else if (name == "Melee") {
            tutorialImg.texture = tutorialPopUps[3];
        } else if (name == "WallClimb") {
            tutorialImg.texture = tutorialPopUps[4];
        } else if (name == "StealthKill") {
            tutorialImg.texture = tutorialPopUps[5];
        } else if (name == "Fire") {
            tutorialImg.texture = tutorialPopUps[6];
        } else if (name == "Aim") {
            tutorialImg.texture = tutorialPopUps[7];
        } else if (name == "Sneak") {
            tutorialImg.texture = tutorialPopUps[8];
        } else if (name == "Hide") {
            tutorialImg.texture = tutorialPopUps[9];
        } else if (name == "CampFire") {
            tutorialImg.texture = tutorialPopUps[10];
        }
        GameManager.singleton.AddTutorial(name);
        Time.timeScale = 0f;
        InputManager.singleton.PlayerInputEnabled = false;
        tutorialPopUp.SetActive(true);
        sounds.Play("TutorialPopUp");

        StartCoroutine("FadeInTutorial");
    }

    // Exits whatever Pop Up or prompt is being displayed.
    public void ExitPopUp() {
        if (isShowingTutorialPopUp && !isFadingInTutorialPopUp) {
            RemoveTutorialPopUp();
        }
    }

    // Removes the tutorial pop up if it's currently being displayed.
    public void RemoveTutorialPopUp() {
        StopAllCoroutines();
        tutorialImg.color = new Color(1f, 1f, 1f, 0f);
        // enterPromptImg.color = new Color(1f, 1f, 1f, 0f);
        tutorialPopUp.SetActive(false);
        tutorialImg.texture = null;
        isShowingTutorialPopUp = false;
        Time.timeScale = 1f;
        InputManager.singleton.PlayerInputEnabled = true;
    }

    // Hides the Player status UI.
    public void HidePlayerStatus(bool state) {
        playerStatus.SetActive(!state);
    }


    // Plays the dialogue at the end of the tutorial.
    public IEnumerator StartTutorialEndCinematic() {
        dialogue.SetActive(true);
        Dialogue dialogueScript = dialogue.GetComponent<Dialogue>();
        yield return dialogueScript.StartDialogue();
        dialogue.SetActive(false);
        levelStats.SetActive(true);
        LevelStats levelStatsScript = levelStats.GetComponent<LevelStats>();
        levelStatsScript.DisplayLevelStats();
    }
    #endregion

    #region Coroutine Functions
 
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

    // Fades in the tutorial.
    IEnumerator FadeInTutorial() {
        isFadingInTutorialPopUp = true;
        float speed = tutorialFadeSpeed;
        for (float alpha = 0f; alpha < 1f; alpha += Time.unscaledDeltaTime * speed) {
            tutorialImg.color = new Color(1f, 1f, 1f, alpha);
            yield return new WaitForEndOfFrame();
        }
        tutorialImg.color = new Color(1f, 1f, 1f, 1f);
        isFadingInTutorialPopUp = false;

        yield return new WaitForSecondsRealtime(1f);

        enterPrompt.SetActive(true);

        // Create a blinking effect for the press enter prompt.
        // speed *= 1.5f;

        // while (true) {
        //     for (float alpha = 0f; alpha < 1f; alpha += Time.unscaledDeltaTime * speed) {
        //         enterPromptImg.color = new Color(1f, 1f, 1f, alpha);
        //         yield return new WaitForEndOfFrame();
        //     }
        //     enterPromptImg.color = new Color(1f, 1f, 1f, 1f);
            
        //     for (float alpha = 1f; alpha > 0f; alpha -= Time.unscaledDeltaTime * speed) {
        //         enterPromptImg.color = new Color(1f, 1f, 1f, alpha);
        //         yield return new WaitForEndOfFrame();
        //     }
        //     enterPromptImg.color = new Color(1f, 1f, 1f, 0f);
        // }
    }
    #endregion
}
