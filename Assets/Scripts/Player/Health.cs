using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Health : MonoBehaviour
{
    // Public variables
    public int maxHealth;
    public float bufferDur;
    public float stunDuration;
    public float stunForce;
    public float stunnedGravity;

    // Private variables
    private int currHealth;
    private PlayerController playerScript;
    private BoxCollider2D playerCollider;
    private SpriteRenderer playerSprite;
    private Rigidbody2D playerRB;
    private float gravity;
    public bool isBuffering;

    void Start() {
        currHealth = maxHealth;
        playerScript = this.GetComponent<PlayerController>();
        playerSprite = this.GetComponent<SpriteRenderer>();
        playerCollider = this.GetComponent<BoxCollider2D>();
        playerRB = this.GetComponent<Rigidbody2D>();
        gravity = playerRB.gravityScale;
    }

    private void OnCollisionStay2D(Collision2D other) {
        if (other.gameObject.tag == "Enemy" && !isBuffering) {
            EnemyController enemyScript = other.gameObject.GetComponent<EnemyController>();
            if (!enemyScript.HasDied()) {
                currHealth -= 1;
                ScoreManager.singleton.UpdateHealth(currHealth);
                if (currHealth <= 0) {
                    Destroy(this.gameObject);
                }
                enemyScript.SetAlertStatus(true);
                
                // Knocks player back at a 60 degree angle.
                Vector2 dir = new Vector2(1f / 2f, Mathf.Sqrt(3) / 2f);
                float collisionDir = this.gameObject.transform.position.x - other.gameObject.transform.position.x;
                if (collisionDir < 0) {
                    dir.x *= -1;
                }
                StartCoroutine(Stunned(dir));
                StartCoroutine("DamageBuffer");
            }
        }
    }

    // Prevents the player from moving when getting hit
    private IEnumerator Stunned(Vector2 dir) {
        playerScript.SetStunned(true);
        playerRB.velocity = new Vector2(0f, 0f);
        playerRB.gravityScale = stunnedGravity;
        playerRB.AddForce(dir * stunForce, ForceMode2D.Impulse);
        yield return new WaitForSeconds(stunDuration);
        playerRB.gravityScale = gravity;
        playerScript.SetStunned(false);
    }

    // Controls the invincibility and blinking animation when getting damaged.
    private IEnumerator DamageBuffer() {
        isBuffering = true;
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
        isBuffering = false;
        Physics2D.IgnoreLayerCollision(7, 9, false);
    }

    // Decreases the player's current health by x amount.
    public void TakeDmg(int x, Vector3 enemyPos) {
        if (!isBuffering) {
            currHealth -= x;
            Vector2 dir = new Vector2(1f / 2f, Mathf.Sqrt(3) / 2f);
            float collisionDir = this.gameObject.transform.position.x - enemyPos.x;
            if (collisionDir < 0) {
                dir.x *= -1;
            }
            StartCoroutine(Stunned(dir));
            StartCoroutine("DamageBuffer");
        }
    }
}


