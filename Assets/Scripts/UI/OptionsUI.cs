using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OptionsUI : MonoBehaviour
{
    [SerializeField] GameObject resetPrompt;

    // Toggles the prompt asking if tutorial pop ups should be enabled.
    public void ToggleResetPrompt() {
        bool isActive = resetPrompt.activeInHierarchy;
        resetPrompt.SetActive(!isActive);
    }
}
