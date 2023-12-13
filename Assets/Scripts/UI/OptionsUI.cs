using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionsUI : MonoBehaviour
{
    [SerializeField] GameObject resetPrompt;
    [SerializeField] Button showTimerYesBttn;
    [SerializeField] Button showTimerNoBttn;
    [SerializeField] Slider volSlider;

    private void Start() {
        ShowTimer(PlayerPrefs.GetInt("showTimer") == 1);
        volSlider.value = PlayerPrefs.GetFloat("volume");
    }

    // Toggles the prompt asking to erase all data.
    public void ToggleResetPrompt() {
        bool isActive = resetPrompt.activeInHierarchy;
        resetPrompt.SetActive(!isActive);
    }

    // Deletes all sates.
    public void ResetProgress() {
        GameManager.singleton.DeleteAllStates();
    }

    public void ShowTimer(bool state) {
        if (state) {
            showTimerYesBttn.interactable = false;
            showTimerNoBttn.interactable = true;
        } else {
            showTimerYesBttn.interactable = true;
            showTimerNoBttn.interactable = false;
        }
        GameManager.singleton.ShowTimer(state);
    }

    public void SetVolume() {
        GameManager.singleton.SetVolume(volSlider.value);
    }

    public void GoToMainMenu() {
        GameManager.singleton.ToggleOptions();
        GameManager.singleton.LoadLevel("TitleScreen");
    }
}
