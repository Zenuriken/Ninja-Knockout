using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Health : MonoBehaviour
{
    // Public variables
    public float maxHealth;
    public float bufferDur;
    // Private variables
    private float currHealth;
    private BoxCollider2D playerCollider;
    private SpriteRenderer playerSprite;
    private TMP_Text healthText;

    void Start() {
        currHealth = maxHealth;
        playerSprite = this.GetComponent<SpriteRenderer>();
        playerCollider = this.GetComponent<BoxCollider2D>();
        healthText = GameObject.Find("Health").GetComponent<TMP_Text>();
    }

    private void Update() {
        //Debug.Log("Player health: " + currHealth.ToString() + "  Ignore layer collision: " + Physics2D.GetIgnoreLayerCollision(7, 9));
    }

    private void OnCollisionStay2D(Collision2D other) {
        if (other.gameObject.tag == "Enemy") {
            currHealth -= 1;
            healthText.text = "Health: " + currHealth.ToString();
            if (currHealth <= 0) {
                Destroy(this.gameObject);
            }
            StartCoroutine("DamageBuffer");
        }
    }

    // Controls the invincibility and blinking animation when getting damaged.
    private IEnumerator DamageBuffer() {
        // 7 = Player Layer, 9 = Enemy Layer
        Physics2D.IgnoreLayerCollision(7, 9, true);

        for (float alpha = 1f; alpha >= 0.75f; alpha -= 0.05f) {
            playerSprite.color = new Color(1f, 1f, 1f, alpha);
            yield return new WaitForSeconds(0.01f);
        }

        for (int i = 0; i < bufferDur; i++) {
            for (float alpha = 0.75f; alpha >= 0.25f; alpha -= 0.05f) {
                playerSprite.color = new Color(1f, 1f, 1f, alpha);
                yield return new WaitForSeconds(0.01f);
            }
            for (float alpha = 0.25f; alpha <= 0.75f; alpha += 0.05f) {
                playerSprite.color = new Color(1f, 1f, 1f, alpha);
                yield return new WaitForSeconds(0.01f);
            }
        }

        for (float alpha = 0.75f; alpha <= 1f; alpha += 0.05f) {
            playerSprite.color = new Color(1f, 1f, 1f, alpha);
            yield return new WaitForSeconds(0.01f);
        }

        Physics2D.IgnoreLayerCollision(7, 9, false);

    }





}


