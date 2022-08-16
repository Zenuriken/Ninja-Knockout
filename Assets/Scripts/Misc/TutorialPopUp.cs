using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialPopUp : MonoBehaviour
{
    private bool hasActivated;
    public string tutorialName;

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.tag == "Player" && !hasActivated && UIManager.singleton.TutorialIsEnabled()) {
            UIManager.singleton.ShowTutorialPopUp(tutorialName);
            hasActivated = true;
        }
    }
}
