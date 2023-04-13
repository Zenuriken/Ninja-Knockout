using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TitleScreenUI : MonoBehaviour
{
    [SerializeField] GameObject tutorialPrompt;

    public void ShowTutorialPrompt() {
        tutorialPrompt.SetActive(true);
    }


}
