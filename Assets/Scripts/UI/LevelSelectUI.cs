using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelSelectUI : MonoBehaviour
{
    [SerializeField] List<GameObject> levels;
    [SerializeField] List<Sprite> stars;
    [SerializeField] GameObject tutorialPrompt;
    [SerializeField] TMP_Text coins;

    private void Awake() {
        // PlayerPrefs.SetInt("lvl2_stars", 3);
    }

    private void Start() {
        coins.text = PlayerPrefs.GetInt("coins").ToString();
        int currLvl = PlayerPrefs.GetInt("currLvl");
        for (int i = 0; i < levels.Count; i++) {
            if (i > currLvl) break;
            string currStars = $"lvl{i}_stars";
            Image starsImg = levels[i].transform.GetChild(1).GetComponent<Image>();
            starsImg.sprite = stars[PlayerPrefs.GetInt(currStars)];

            Button bttn = levels[i].transform.GetChild(0).GetComponent<Button>();
            bttn.interactable = true;

            string currTime = $"lvl{i}_time";
            TMP_Text timeTxt = levels[i].transform.GetChild(2).GetComponent<TMP_Text>();
            float time = PlayerPrefs.GetFloat(currTime);
            timeTxt.text = (time < float.MaxValue) ? GameManager.singleton.CalculateTime(time) : "";
        }
    }

    // Toggles the prompt asking if tutorial pop ups should be enabled.
    public void ToggleTutorialPrompt() {
        bool isActive = tutorialPrompt.activeInHierarchy;
        tutorialPrompt.SetActive(!isActive);
    }

    public void SetTutorialStatus(bool state) {
        GameManager.singleton.SetTutorialStatus(state);
    }

    public void LoadLevel(string lvlName) {
        GameManager.singleton.LoadLevel(lvlName);
    }
}
