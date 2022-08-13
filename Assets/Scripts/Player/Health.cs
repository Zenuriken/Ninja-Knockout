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
    public float destroyDelay;

    // Private variables
    private int currHealth;
    private PlayerController playerScript;
    private BoxCollider2D playerCollider;
    private SpriteRenderer playerSprite;
    private Rigidbody2D playerRB;
    private SoundManager sounds;
    private bool hasDied;
    private float gravity;
    public bool isBuffering;

    void Start() {
        currHealth = maxHealth;
        playerScript = this.GetComponent<PlayerController>();
        playerSprite = this.GetComponent<SpriteRenderer>();
        playerCollider = this.GetComponent<BoxCollider2D>();
        playerRB = this.GetComponent<Rigidbody2D>();
        gravity = playerRB.gravityScale;
        sounds = this.transform.GetChild(6).GetComponent<SoundManager>();
        if (!playerScript.GetTitleScreenModeStatus()) {
            UIManager.singleton.UpdateHealth(currHealth);
        }
    }

    private void OnCollisionStay2D(Collision2D other) {
        if (other.gameObject.tag == "Enemy" && !isBuffering && !hasDied) {
            EnemyController enemyScript = other.gameObject.GetComponent<EnemyController>();
            if (!enemyScript.HasDied()) {
                TakeDmg(1, enemyScript.transform.position);
                enemyScript.SetAlertStatus(true);
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
        playerScript.SetPlayerBuffer(true);
        // 7 = Player Layer, 9 = Enemy Layer
        Physics2D.IgnoreLayerCollision(7, 9, true);
        Physics2D.IgnoreLayerCollision(0, 9, true);
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
        playerScript.SetPlayerBuffer(false);
        Physics2D.IgnoreLayerCollision(7, 9, false);
        Physics2D.IgnoreLayerCollision(0, 9, false);
    }

    // Controls the player's death animation.
    private void Death() {
        hasDied = true;
        // 7 = Player Layer, 9 = Enemy Layer
        Physics2D.IgnoreLayerCollision(7, 9, true);
        Physics2D.IgnoreLayerCollision(0, 9, true);
        playerRB.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation;
        playerScript.SetHasDied(true);
    }

    // Decreases the player's current health by x amount.
    public void TakeDmg(int x, Vector3 enemyPos) {
        if (!isBuffering) {
            currHealth -= x;
            UIManager.singleton.UpdateHealth(currHealth);

            // Kill the player
            if (currHealth <= 0) {
                UIManager.singleton.FadeScreen();
                //playerScript.SetPlayerInput(false);
                Death();
                return;
            }

            // Stun the Player
            if (!hasDied) {
                Vector2 dir = new Vector2(1f / 2f, Mathf.Sqrt(3) / 2f);
                float collisionDir = this.gameObject.transform.position.x - enemyPos.x;
                if (collisionDir < 0) {
                    dir.x *= -1;
                }
                sounds.Play("Grunt");
                StartCoroutine(Stunned(dir));
                StartCoroutine("DamageBuffer");
            }
        }
    }

    // Decreases the player's health when interacting with environment
    public void TakeEnvironDmg(int x) {
        currHealth -= x;
        UIManager.singleton.UpdateHealth(currHealth);
        UIManager.singleton.FadeScreen();
        playerScript.SetPlayerInput(false);

        // Kill the player
        if (currHealth <= 0) {
            StartCoroutine("Death");
            return;
        } else {
            sounds.Play("Grunt");
        } 
    }

    // Sets the players health to the specified number
    public void SetPlayerHealth(int num) {
        currHealth = num;
        UIManager.singleton.UpdateHealth(currHealth);
    }

    public void ResetHealth() {
        hasDied = false;
        // 7 = Player Layer, 9 = Enemy Layer
        Physics2D.IgnoreLayerCollision(7, 9, false);
        Physics2D.IgnoreLayerCollision(0, 9, false);
        playerRB.constraints = RigidbodyConstraints2D.FreezeRotation;
        playerScript.SetHasDied(false);
        playerSprite.color = new Color(1f, 1f, 1f, 1f);
        currHealth = maxHealth;
        UIManager.singleton.UpdateHealth(currHealth);
    }
}


