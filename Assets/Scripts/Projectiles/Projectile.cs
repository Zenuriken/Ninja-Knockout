using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Projectile
{
    // private Rigidbody2D rb;
    // private CircleCollider2D col;
    // private TrailRenderer trailRen;
    // private SpriteRenderer shurikenSprite;
    // private ParticleSystem sparks;
    // private Animator anim;
    // private SoundManager sounds;
    // private Vector2 throwDir;
    // private float fadeAwayDelay = 1.5f;
    // private float fadeAwaySpeed = 0.5f;
    // private float shurikenSpeed = 20;
    // private float noContactDestroyTime = 5f;
    // private float currflightTime;
    // private bool isActive;
    // private bool hasContact;
    // private string owner;


    // private void Update() {
    //     // Increment flight time if the shuriken hasn't made contact.
    //     if(!hasContact) currflightTime += Time.deltaTime;
    //     // Destroy the shuriken after flying for specified time.
    //     if (currflightTime >= noContactDestroyTime) GameObject.Destroy(this);
    // }

    // // When the shuriken hits a platform/Entity.
    // private void OnTriggerEnter2D(Collider2D other) {
    //     if (!isActive) return;
        
    //     if (other.gameObject.tag == "Platform" || (other.gameObject.tag == "TrapDoor" && this.owner == "Player")) {
    //         StartCoroutine(Contact(false));
    //     } else if (other.gameObject.tag == "Enemy" && this.owner == "Player") {
    //         EnemyStateManager enemyScript = other.gameObject.GetComponent<EnemyStateManager>();
    //         if (!enemyScript.IsAlerted && !enemyScript.IsDetectingPlayer) {
    //             enemyScript.TakeDmg(5);
    //             sounds.Play("ShurikenStealthKill");
    //         } else {
    //             enemyScript.TakeDmg(1);
    //             sounds.Play("ShurikenBodyHit");
    //         }
    //         StartCoroutine(Contact(true));
    //     } else if (other.gameObject.tag == "Player" && this.owner == "Enemy") {
    //         Health playerHealth = PlayerController.singleton.GetComponent<Health>();
    //         playerHealth.TakeDmg(1, this.transform.position);
    //         sounds.Play("ShurikenBodyHit");
    //         StartCoroutine(Contact(true));
    //     } else if (other.gameObject.tag == "Destructible") {
    //         Destructible obj = other.gameObject.GetComponent<Destructible>();
    //         obj.Break();
    //         StartCoroutine(Contact(true));
    //     }
    // }

    // // Creates spark on contact and then destroys shuriken.
    // IEnumerator Contact(bool hitEntity) {
    //     isActive = false;
    //     anim.enabled = false;
    //     hasContact = true;
    //     rb.velocity = new Vector2(0f, 0f);

    //     sounds.Stop("Shuriken");
    //     if (hitEntity) {
    //         this.gameObject.layer = 12;
    //         shurikenSprite.enabled = false;
    //         trailRen.enabled = false;
    //         yield return new WaitForSeconds(1f);
    //         Destroy(this.gameObject);
    //     } else {
    //         sounds.Play("ShurikenGroundHit");
    //         CreateSparks();
    //         StartCoroutine("FadeAway");
    //     }
    // }

    // IEnumerator FadeAway() {
    //     yield return new WaitForSeconds(fadeAwayDelay);
    //     for (float alpha = 1f; alpha > 0f; alpha -= Time.deltaTime * fadeAwaySpeed) {
    //         shurikenSprite.color = new Color(1f, 1f, 1f, alpha);
    //         yield return new WaitForEndOfFrame();
    //     }
    // }

    // // Creates sparks when called.
    // private void CreateSparks() {
    //     sparks.Play();
    // }

    // public string Owner {get{return owner;} set{owner = value;}}

    // // Sets the shuriken's velocity and direction.
    // public void SetShurikenVelocity(Vector2 dir) {
    //     throwDir = dir;
    //     if (throwDir.x < 0f) {
    //         shurikenSprite.flipX = true;
    //         sparks.transform.rotation = Quaternion.Euler(0, 0, -90);
    //     } else {
    //         shurikenSprite.flipX = false;
    //         sparks.transform.rotation = Quaternion.Euler(0, 0, 90);
    //     }
    //     rb.velocity = throwDir * shurikenSpeed;
    //     sounds.Play("Shuriken");
    // }
}
