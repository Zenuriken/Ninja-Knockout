using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager singleton;
    // private TMP_Text healthText;
    // private TMP_Text shurikenText;
    // private TMP_Text scoreText;
    private static int health;
    private static int shurikensRemaining;
    private static int score;

    public List<Texture> healthList;
    public List<Texture> shurikenList;

    private RawImage curHealthSprite;
    private RawImage curShurikenSprite;

    private void Awake() {
        if (singleton != null && singleton != this) { 
            Destroy(this); 
        } 
        else { 
            singleton = this;
            curHealthSprite = GameObject.Find("LifeUI").GetComponent<RawImage>();
            curShurikenSprite = GameObject.Find("ShurikenUI").GetComponent<RawImage>();
        }
    }
    
    public void IncreaseScoreBy(int amount) {
        score += amount;
    }

    public void UpdateShurikenNum(int newNum) {
        shurikensRemaining = newNum;
        if (shurikensRemaining <= 0) {
            curShurikenSprite.texture = null;
            curShurikenSprite.color = new Color(0f, 0f, 0f, 0f);
        } else {
            curShurikenSprite.texture = shurikenList[shurikensRemaining - 1];
        }
    }

    public void UpdateHealth(int newHealth) {
        health = newHealth;
        if (health <= 0) {
            curHealthSprite.texture = null;
            curHealthSprite.color = new Color(0f, 0f, 0f, 0f);
        } else {
            curHealthSprite.texture = healthList[health - 1];
        }
    }

}
