using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.IO;
using UnityEngine.UI;

public class Dialogue : MonoBehaviour
{
    [SerializeField][Tooltip("Dialogue text")]
    private TMP_Text dialogue;
    [SerializeField][Tooltip("The time per char")]
    private float timePerChar;
    [SerializeField][Tooltip("UI sounds")]
    private SoundManager sounds;

    private List<string> dialogueList;
    private string currTxt;
    private int currIndex;
    private bool isAnimatingDialogue;
    private Image backgroundSprite;
    private GameObject indicator;
    private GameObject dialogueBox;
    private IEnumerator coroutineMethod;

    private void Awake() {
        dialogueBox = this.transform.GetChild(0).gameObject;
        backgroundSprite = dialogueBox.GetComponent<Image>();
        indicator = this.transform.GetChild(1).gameObject;
        dialogueList = new List<string>();
    }

    private void Update() {
        if (ShouldSkip() && coroutineMethod != null) {
            StopCoroutine(coroutineMethod);
            coroutineMethod = null;
        }
    }

    public void InitializeDialogue() {
        dialogueList.Add("_W_Sir! There was an attack last night at one of our camps. A few of our members were killed, and some of our supplies have been looted.");
        dialogueList.Add("_R_What? How is this possible?");
        dialogueList.Add("_W_We are unsure sir. The perpetuators must have snuck in and took out the guards silently. They left no evidence behind.");
        dialogueList.Add("_R_Damn thieves!");
        dialogueList.Add("_R_Interrogate the villagers in the area for any eye-witness reports and have a burial for the bodies. Report back to me if anything comes up.");
        dialogueList.Add("_W_Yes sir. Right away.");
        dialogueList.Add("_R_Whoever did this will pay for what they've done...");
    }

    IEnumerator ShowText() {
        isAnimatingDialogue = true;
        indicator.SetActive(false);
        currTxt = dialogueList[currIndex];
        string color = currTxt.Substring(0, 3);
        if (color == "_W_") {
            dialogue.color = Color.white;
        } else if (color == "_R_") {
            dialogue.color = Color.red;
        }
        currTxt = currTxt.Substring(3, currTxt.Length - 3);
        sounds.Play("Dialogue");
        for (int t = 1; t <= currTxt.Length; t++) {
            dialogue.text = currTxt.Substring(0, t);
            yield return new WaitForSeconds(timePerChar);
        }
        coroutineMethod = null;
    }

    public IEnumerator StartDialogue() {
        InitializeDialogue();
        dialogueBox.SetActive(true);

        for (int i = 0; i < dialogueList.Count; i++) {
            coroutineMethod = ShowText();
            StartCoroutine(coroutineMethod);

            while (coroutineMethod != null){
                yield return null;
            }

            dialogue.text = currTxt;
            isAnimatingDialogue = false;
            sounds.Stop("Dialogue");
            indicator.SetActive(true);
            currIndex += 1;

            yield return new WaitForSecondsRealtime(0.1f);

            yield return new WaitUntil(ShouldContinue);
        }
        indicator.SetActive(false);
        yield return StartCoroutine("FadeOutText");

    }

    private bool ShouldContinue() {
        bool shouldContinue = InputManager.singleton.ContinuePressed && !isAnimatingDialogue;
        if (shouldContinue) {
            sounds.Play("Click");
        }
        return shouldContinue;
    }

    private bool ShouldSkip() {
        bool shouldSkip = InputManager.singleton.ContinuePressed && isAnimatingDialogue;
        return shouldSkip;
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
