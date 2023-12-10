using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.IO;
using UnityEngine.UI;

public class Dialogue : MonoBehaviour
{
    [SerializeField]
    [Tooltip("Dialogue text")]
    private TMP_Text dialogue;
    [SerializeField]
    [Tooltip("The time per char")]
    private float timePerChar;
    [SerializeField]
    [Tooltip("UI sounds")]
    private SoundManager sounds;

    private List<string> dialogueList;

    private int enemiesKilled;
    private int suppliesLooted;
    private int survivors;

    private int currIndex;

    private bool isAnimatingDialogue;

    private AudioSource sound;
    private Image backgroundSprite;
    private GameObject indicator;
    private GameObject dialogueBox;

    private void Awake() {
        sound = this.GetComponent<AudioSource>();
        dialogueBox = this.transform.GetChild(0).gameObject;
        backgroundSprite = dialogueBox.GetComponent<Image>();
        indicator = this.transform.GetChild(1).gameObject;
        dialogueList = new List<string>();
        
    }

    public void InitializeDialogue() {
        dialogueList.Add("_W_Sir! There was an attack last night at one of our camps." + " " + enemiesKilled.ToString() + " of our comrades were killed and "
                         + suppliesLooted.ToString() + " of our supplies were looted.");
        dialogueList.Add("_R_Were there any survivors?");
        if (survivors > 0) {
            dialogueList.Add("_W_Only " + survivors.ToString() + " sir.");
        } else {
            dialogueList.Add("_W_No sir.");
        }
        dialogueList.Add("_R_Have you captured any of the perpetrators?");
        dialogueList.Add("_W_No sir.");
        dialogueList.Add("_R_Damn thieves!");
        dialogueList.Add("_R_Interrogate the villagers in the area for any eye-witness reports and have a burial for the bodies. Report back to me if anything comes up.");
        dialogueList.Add("_W_Yes sir. Right away.");
        dialogueList.Add("_R_Whoever did this will pay for what they've done...");
    }

    IEnumerator ShowText() {
        isAnimatingDialogue = true;
        indicator.SetActive(false);
        string currTxt = dialogueList[currIndex];
        string color = currTxt.Substring(0, 3);
        if (color == "_W_") {
            dialogue.color = Color.white;
        } else if (color == "_R_") {
            dialogue.color = Color.red;
        }
        currTxt = currTxt.Substring(3, currTxt.Length - 3);
        sounds.Play("Dialogue");
        for (int t = 1; t <= currTxt.Length; t++) {
            // if (PlayerController.singleton.HasPressedContinue() && isAnimatingDialogue) {
            //     break;
            // }
            dialogue.text = currTxt.Substring(0, t);

            yield return new WaitForSeconds(timePerChar);
        }
        dialogue.text = currTxt;
        isAnimatingDialogue = false;
        sounds.Stop("Dialogue");
        indicator.SetActive(true);
        yield return new WaitUntil(ShouldSkip);
        //isAnimatingDialogue = false;
    }

    public IEnumerator StartDialogue() {
        dialogueBox.SetActive(true);

        for (int i = 0; i < dialogueList.Count; i++) {
            yield return StartCoroutine("ShowText");
            currIndex += 1;
        }
        indicator.SetActive(false);
        yield return StartCoroutine("FadeOutText");

    }

    public bool ShouldSkip() {
        bool shouldSkip = InputManager.singleton.ContinuePressed && !isAnimatingDialogue;
        if (shouldSkip) {
            sounds.Play("Click");
        }
        return shouldSkip;
    }

    public void InitializeVariables(int enemiesKilled, int suppliesLooted, int survivors) {
        this.enemiesKilled = enemiesKilled;
        this.suppliesLooted = suppliesLooted;
        this.survivors = survivors;
    }

    IEnumerator FadeOutText() {
        yield return new WaitForSeconds(1f);
        for (float alpha = 1f; alpha > 0f; alpha -= Time.deltaTime * 0.5f) {
            backgroundSprite.color = new Color(1f, 1f, 1f, alpha);
            dialogue.alpha = alpha;
            yield return new WaitForEndOfFrame();
        }
        backgroundSprite.color = new Color(0f, 0f, 0f, 0f);
        dialogue.alpha = 0f;
    }

}
