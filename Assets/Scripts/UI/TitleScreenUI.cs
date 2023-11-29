using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TitleScreenUI : MonoBehaviour
{
    [SerializeField] GameObject tutorialPrompt;
    
    // Toggles the prompt asking if tutorial pop ups should be enabled.
    public void ToggleTutorialPrompt() {
        bool isActive = tutorialPrompt.activeInHierarchy;
        tutorialPrompt.SetActive(!isActive);
    }
}
