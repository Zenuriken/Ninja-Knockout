using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

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
    public float fadeAwaySpeed;
    public float deathFadeAwaySpeed;
    public float fadeAwayDelay;

    private bool isBlackingOutScreen;
    private RawImage curHealthSprite;
    private RawImage curShurikenSprite;
    private Image fadeOutScreen;
    private Vector2 spawnLocation;

    private void Awake() {
        if (singleton != null && singleton != this) { 
            Destroy(this); 
        } 
        else { 
            singleton = this;
            curHealthSprite = GameObject.Find("LifeUI").GetComponent<RawImage>();
            curShurikenSprite = GameObject.Find("ShurikenUI").GetComponent<RawImage>();
            fadeOutScreen = GameObject.Find("FadeOutScreen").GetComponent<Image>();
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
        } else if (health > 0) {
            curHealthSprite.texture = healthList[health - 1];
        }
    }

    IEnumerator BlackOut() {
        isBlackingOutScreen = true;
        float speed;
        if (health <= 0) {
            speed = deathFadeAwaySpeed;
        } else {
            speed = fadeAwaySpeed;
        }
        yield return new WaitForSeconds(fadeAwayDelay);
        for (float alpha = 0f; alpha < 1f; alpha += Time.deltaTime * speed) {
            fadeOutScreen.color = new Color(0f, 0f, 0f, alpha);
            yield return new WaitForEndOfFrame();
        }
        fadeOutScreen.color = new Color(0f, 0f, 0f, 1f);

        // If the player is still alive, bring them to the last checkpoint. If not, then restart level.
        PlayerController playerScript = GameObject.Find("Player").GetComponent<PlayerController>();
        if (health > 0) {
            playerScript.SetPlayerInput(false);
            Rigidbody2D playerRB = playerScript.GetComponent<Rigidbody2D>();
            playerRB.velocity = new Vector2(0f, playerRB.velocity.y);
            playerScript.transform.position = this.spawnLocation;
            yield return new WaitForSeconds(fadeAwayDelay);
            for (float alpha = 1f; alpha > 0f; alpha -= Time.deltaTime * speed) {
                fadeOutScreen.color = new Color(0f, 0f, 0f, alpha);
                yield return new WaitForEndOfFrame();
            }
            fadeOutScreen.color = new Color(0f, 0f, 0f, 0f);
        } else {
            SceneManager.LoadScene("Tutorial");
        }
        isBlackingOutScreen = false;
        playerScript.SetPlayerInput(true);
    }

    public void FadeScreen() {
        if (!isBlackingOutScreen) {
            StartCoroutine("BlackOut");
        }
    }

    public void SetSpawnLocation(Vector2 location) {
        this.spawnLocation = location;
    }
}
