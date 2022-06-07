using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Health : MonoBehaviour
{
    // Public variables
    public float maxHealth;
    public float bufferDur;
    public float stunDuration;
    public float knockBackForce;
    public float stunnedGravity;

    // Private variables
    private float currHealth;
    private PlayerController playerScript;
    private BoxCollider2D playerCollider;
    private SpriteRenderer playerSprite;
    private Rigidbody2D playerRB;
    private TMP_Text healthText;
    private float gravity;

    void Start() {
        currHealth = maxHealth;
        playerScript = this.GetComponent<PlayerController>();
        playerSprite = this.GetComponent<SpriteRenderer>();
        playerCollider = this.GetComponent<BoxCollider2D>();
        playerRB = this.GetComponent<Rigidbody2D>();
        healthText = GameObject.Find("Health").GetComponent<TMP_Text>();
        gravity = playerRB.gravityScale;
    }

    private void Update() {
        //Debug.Log("Player health: " + currHealth.ToString() + "  Ignore layer collision: " + Physics2D.GetIgnoreLayerCollision(7, 9));
    }

    private void OnCollisionStay2D(Collision2D other) {
        if (other.gameObject.tag == "Enemy") {
            currHealth -= 1;
            if (currHealth <= 0) {
                Destroy(this.gameObject);
            }
            
            Vector2 dir = new Vector2(1f / 2f, Mathf.Sqrt(3) / 2f);
            float collisionDir = this.gameObject.transform.position.x - other.gameObject.transform.position.x;
            if (collisionDir < 0) {
                dir.x *= -1;
            }
            Debug.Log("Direction: " + dir.ToString());
            StartCoroutine(Stunned(dir));
            StartCoroutine("DamageBuffer");
        }
    }

    // Prevents the player from moving when getting hit
    private IEnumerator Stunned(Vector2 dir) {
        playerScript.SetStunned(true);
        // float t = 0;
        // while (t < 100) {
        //     float x = v0 * t * Mathf.Cos(angle);
        //     float y = v0 * t * Mathf.Sin(angle) - (1f / 2f) * -Physics.gravity.y * Mathf.Pow(t, 2);
        //     transform.position = new Vector3(x, y, 0);
        //     t += Time.deltaTime;
        //     yield return null;
        // }
        playerRB.velocity = new Vector2(0f, 0f);
        playerRB.gravityScale = stunnedGravity;
        playerRB.AddForce(dir * knockBackForce, ForceMode2D.Impulse);
        yield return new WaitForSeconds(stunDuration);
        playerRB.gravityScale = gravity;
        playerScript.SetStunned(false);
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

    // Knocks the player back when getting damaged;
    // private IEnumerator KnockBack(Vector2 dir) {
    //     playerRB.velocity = new Vector2(0f, 0f);
    //     playerRB.AddForce(dir * knockBackForce, ForceMode2D.Impulse);
    //     yield return new WaitForSeconds(knockBackDur);
    // }





}


