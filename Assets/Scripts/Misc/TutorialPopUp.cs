using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialPopUp : MonoBehaviour
{
    private bool hasActivated;
    public string tutorialName;

    private void OnTriggerStay2D(Collider2D other) {
        if (other.gameObject.tag == "Player" && !hasActivated && UIManager.singleton.TutorialIsEnabled() && !UIManager.singleton.IsFading() && UIManager.singleton.ShouldShow(tutorialName)) {
            UIManager.singleton.ShowTutorialPopUp(tutorialName);
            hasActivated = true;
        }
    }
}
