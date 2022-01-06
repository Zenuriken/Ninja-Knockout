using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    // Public variables
    public float maxHealth;
    public float bufferDur;
    public float smoothTime;
    // Private variables
    private float currHealth;
    private BoxCollider2D playerCollider;
    private SpriteRenderer playerSprite;

    public float minimum = 0.0f;
    public float maximum = 1f;
    public float duration = 5.0f;
    private float startTime;
    private bool damaged;

    void Start() {
        startTime = Time.time;
        currHealth = maxHealth;
        playerSprite = this.GetComponent<SpriteRenderer>();
        playerCollider = this.GetComponent<BoxCollider2D>();
    }

    void Update() {
        //Debug.Log("Player health: " + currHealth.ToString() + "  Ignore layer collision: " + Physics2D.GetIgnoreLayerCollision(7, 9));
        if (damaged) {
            float t = (Time.time - startTime) / duration;
            playerSprite.color = new Color(1f, 1f, 1f, Mathf.SmoothStep(minimum, maximum, t)); 
        }
       
    }

    private void OnCollisionStay2D(Collision2D other) {
        if (other.gameObject.tag == "Enemy") {
            currHealth -= 1;
            StartCoroutine("DamageBuffer");
        }
    }

    private IEnumerator DamageBuffer() {
        // 7 = Player Layer, 9 = Enemy Layer
        Physics2D.IgnoreLayerCollision(7, 9, true);
        //playerSprite.color = new Color(1f, 1f, 1f, Mathf.SmoothStep(minimum, maximum, t));
        damaged = true;
        yield return new WaitForSeconds(bufferDur);
        damaged = false;
        Physics2D.IgnoreLayerCollision(7, 9, false);
        //this.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 1f);

    }





}


