using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class ScoreManager : MonoBehaviour
{
    public static ScoreManager singleton;
    private TMP_Text healthText;
    private TMP_Text shurikenText;
    private TMP_Text scoreText;
    private static int health;
    private static int shurikensRemaining;
    private static int score;

    private void Awake() {
        if (singleton != null && singleton != this) { 
            Destroy(this); 
        } 
        else { 
            singleton = this;
        }
    }

    private void Start() {
        healthText = GameObject.Find("Health").GetComponent<TMP_Text>();
        shurikenText = GameObject.Find("Shurikens").GetComponent<TMP_Text>();
        scoreText = GameObject.Find("Score").GetComponent<TMP_Text>();
    }
    
    public void IncreaseScore(int amount) {
        score += amount;
        scoreText.text = "Score: " + score;
    }

    public void UpdateShurikenNum(int newNum) {
        shurikensRemaining = newNum;
        shurikenText.text = "Shurikens: " + shurikensRemaining;
    }

    public void UpdateHealth(int newHealth) {
        health = newHealth;
        healthText.text = "HP: " + health;
    }

}
