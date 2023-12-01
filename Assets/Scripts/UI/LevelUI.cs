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
    [SerializeField]
    private GameObject UISounds;
    #endregion

    #region UI Variables
    [SerializeField][Tooltip("How fast the gold UI will fade")]
    private float goldUIFadeSpeed;
    [SerializeField][Tooltip("The delay before something starts fading.")]
    private float fadeAwayDelay;
    #endregion
   
    private SoundManager sounds;
    private RawImage currHealthSprite;
    private RawImage currShurikenBackgroundSprite;
    private RawImage currShurikenSprite;
    private RawImage currTutorial;
    private RawImage goldSprite;
    private TMP_Text goldTxt;

    private float pickUpTime;
    private int health;
    private int shurikensRemaining;
    private int gold;
    private int lastTutorialPopUp;
    private List<string> shownTutorials; 
    bool isShowingTutorialPopUp;

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

            goldSprite = playerStatus.transform.GetChild(5).GetComponent<RawImage>();
            goldTxt = goldSprite.transform.GetChild(0).GetComponent<TMP_Text>();

            sounds = UISounds.GetComponent<SoundManager>();
            shownTutorials = new List<string>();
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
        // tutorialBackground.SetActive(true);
        // enterPrompt.SetActive(true);
        Time.timeScale = 0f;
        shownTutorials.Add(name);
        PlayerController.singleton.SetPlayerInput(false);
        sounds.Play("TutorialPopUp");
    }

    // Exits whatever Pop Up or prompt is being displayed.
    public void ExitPopUp() {
        if (isShowingTutorialPopUp) {
            RemoveTutorialPopUp();
        }
    }

    // Removes the tutorial pop up if it's currently being displayed.
    public void RemoveTutorialPopUp() {
        currTutorial.texture = null;
        currTutorial.color = new Color(0f, 0f, 0f, 0f);
        // tutorialBackground.SetActive(false);
        // enterPrompt.SetActive(false);
        isShowingTutorialPopUp = false;
        Time.timeScale = 1f;
        PlayerController.singleton.SetPlayerInput(true);
    }


    // Returns whether the currTutorialNumber is less than or greater than the tutorial popup that wants to show
    public bool ShouldShow(string name) {
        return !shownTutorials.Contains(name);
    }


    // Plays the dialogue at the end of the tutorial.
    // public IEnumerator StartTutorialEndCinematic() {
    //     dialogue.SetActive(true);
    //     enemiesKilled = totalEnemies - enemies.transform.childCount;
    //     suppliesLooted = totalSupplies - breakables.transform.childCount;
    //     Dialogue dialogueScript = dialogue.GetComponent<Dialogue>();
    //     dialogueScript.InitializeVariables(enemiesKilled, suppliesLooted, enemies.transform.childCount);
    //     dialogueScript.InitializeDialogue();
    //     yield return dialogueScript.StartDialogue();
    //     dialogue.SetActive(false);
    // }
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
    #endregion
}
