using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelStats : MonoBehaviour
{
    [SerializeField][Tooltip("The text to display the number of supplies that have been looted.")]
    TMP_Text suppliesLootedStat;
    [SerializeField][Tooltip("The text to display the number of enemies that have been killed.")]
    TMP_Text enemiesKilledStat;
    [SerializeField][Tooltip("The text to display the number of detections that occurred.")]
    TMP_Text numDetectionsStat;
    [SerializeField][Tooltip("The text to display the amount of in game time that has passed.")]
    TMP_Text timeStat;
    [SerializeField][Tooltip("The number of stars to display.")]
    RawImage starsImg;
    [SerializeField][Tooltip("An array holding different number of star images.")]
    List<Texture> starsImgArray;

    int numSatisfiedStats;
    bool waitingForInput;

    private void Update() {
        if (waitingForInput && InputManager.singleton.ClosedPressed) {
            waitingForInput = false;
            GameManager.singleton.LoadLevel("TitleScreen");
        }
    }

    public void DisplayLevelStats() {
        DisplayStat(suppliesLootedStat, GameManager.singleton.SuppliesLooted, GameManager.singleton.TotalSupplies, false);
        DisplayStat(enemiesKilledStat, GameManager.singleton.EnemiesKilled, GameManager.singleton.TotalEnemies, false);
        DisplayStat(numDetectionsStat, GameManager.singleton.NumDetections, GameManager.singleton.NumDetectionsAllowed, true);
        timeStat.text = GameManager.singleton.CalculateTime(GameManager.singleton.InGameTime);
        starsImg.texture = starsImgArray[numSatisfiedStats];
        GameManager.singleton.SaveStats(numSatisfiedStats);
        waitingForInput = true;
    }


    private void DisplayStat(TMP_Text stat, int num, int total, bool lessIsBetter) {
        stat.text = num.ToString() + " / " + total.ToString();
        
        if (num <= total && lessIsBetter) {
            stat.color = Color.yellow;
            numSatisfiedStats += 1;
        } else if (num > total && lessIsBetter) {
            stat.color = Color.red;
        } else if (num == total) {
            stat.color = Color.yellow;
            numSatisfiedStats += 1;
        } else {
            stat.color = Color.red;
        }
    }


}
