using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.IO;

public class Dialogue : MonoBehaviour
{
    [SerializeField]
    [Tooltip("Dialogue text")]
    private TMP_Text dialogue;
    [SerializeField]
    [Tooltip("The time per char")]
    private float timePerChar;

    private List<string> dialogueList;

    private int enemiesKilled;
    private int suppliesLooted;
    private int survivors;

    private int currIndex;

    private bool isAnimatingDialogue;

    private void Start() {
        dialogueList = new List<string>();
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
        dialogueList.Add("_R_Interrogate the villagers in the area for any eye-witness reports and have a burial for the bodies. Let me know if anything comes up.");
        dialogueList.Add("_W_Yes sir. Right away.");
        dialogueList.Add("_R_Whoever did this will pay for what they've done...");
        dialogueList.Add("_W_ ");

        StartCoroutine("StartDialogue");
    }

    IEnumerator ShowText() {
        isAnimatingDialogue = true;
        string currTxt = dialogueList[currIndex];
        string color = currTxt.Substring(0, 3);
        if (color == "_W_") {
            dialogue.color = Color.white;
        } else if (color == "_R_") {
            dialogue.color = Color.red;
        }
        currTxt = currTxt.Substring(3, currTxt.Length - 3);
        for (int t = 1; t <= currTxt.Length; t++) {
            // if (PlayerController.singleton.HasPressedContinue() && isAnimatingDialogue) {
            //     break;
            // }
            dialogue.text = currTxt.Substring(0, t);
            yield return new WaitForSeconds(timePerChar);
        }
        dialogue.text = currTxt;
        isAnimatingDialogue = false;
        yield return new WaitUntil(ShouldSkip);
        //isAnimatingDialogue = false;
    }

    IEnumerator StartDialogue() {
        for (int i = 0; i < dialogueList.Count; i++) {
            yield return StartCoroutine("ShowText");
            currIndex += 1;
        }
    }

    public bool ShouldSkip() {
        return PlayerController.singleton.HasPressedContinue() && !isAnimatingDialogue;
    }
}
