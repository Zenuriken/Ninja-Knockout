using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

public class TitleScreenUI : MonoBehaviour
{    
    [SerializeField] GameObject tutorialPrompt;
    [SerializeField] GameObject titleScreenCanvas;
    [SerializeField] GameObject levelSelectCanvas;
    [SerializeField] GameObject optionsCanvas;

    string currCanvas;

    private void Awake() {
        SetCanvas("title");
    }
    
    // Called by Play and Main Menu buttons.
    public void SetCanvas(string canvas) {
        currCanvas = canvas;
        if (currCanvas == "title") {
            titleScreenCanvas.SetActive(true);
            optionsCanvas.SetActive(false);
            levelSelectCanvas.SetActive(false);
        } else if (currCanvas == "options") {
            titleScreenCanvas.SetActive(false);
            optionsCanvas.SetActive(true);
            levelSelectCanvas.SetActive(false);
        } else {
            titleScreenCanvas.SetActive(false);
            optionsCanvas.SetActive(false);
            levelSelectCanvas.SetActive(true);
        }
    }


    #region Title Canvas

    #endregion

    #region Options Canvas

    #endregion

    #region Levels Canvas
    // Toggles the prompt asking if tutorial pop ups should be enabled.
    public void ToggleTutorialPrompt() {
        bool isActive = tutorialPrompt.activeInHierarchy;
        tutorialPrompt.SetActive(!isActive);
    }
    #endregion
}
