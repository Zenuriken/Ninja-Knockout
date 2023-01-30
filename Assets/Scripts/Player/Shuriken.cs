using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shuriken : MonoBehaviour
{
    [SerializeField]
    [Tooltip("The delay before shuriken begins to fade.")]
    private float fadeAwayDelay;
    [SerializeField]
    [Tooltip("The speed in which shuriken fades away.")]
    private float fadeAwaySpeed;

    #region Private Variables
    private Rigidbody2D rb;
    private CircleCollider2D col;
    private TrailRenderer trailRen;
    private SpriteRenderer shurikenSprite;
    private ParticleSystem sparks;
    private Animator anim;
    private SoundManager sounds;
    private Vector2 throwDir;
    private float shurikenSpeed = 20;
    private float noContactDestroyTime = 5f;
    private float currflightTime;
    private bool isActive;
    private bool hasContact;

    private string owner;
    #endregion

    #region Initialization Functions
    private void Awake()
    {
        rb = this.GetComponent<Rigidbody2D>();
        col = this.GetComponent<CircleCollider2D>();
        trailRen = this.GetComponent<TrailRenderer>();
        sparks = this.transform.GetChild(0).GetComponent<ParticleSystem>();
        shurikenSprite = this.GetComponent<SpriteRenderer>();
        anim = this.GetComponent<Animator>();
        sounds = this.transform.GetChild(1).GetComponent<SoundManager>();
    }

    private void Start() {
        isActive = true;
    }

    private void Update() {
        // Increment flight time if the shuriken hasn't made contact.
        if(!hasContact) currflightTime += Time.deltaTime;
        // Destroy the shuriken after flying for specified time.
        if (currflightTime >= noContactDestroyTime) Destroy(this.gameObject);
    }
    #endregion

    #region Collision Functions
    // When the shuriken hits a platform/Entity.
    private void OnTriggerEnter2D(Collider2D other) {
        if (!isActive) return;
        
        if (other.gameObject.tag == "Platform" || other.gameObject.tag == "TrapDoor") {
            StartCoroutine(Contact(false));
        } else if (other.gameObject.tag == "Enemy" && this.owner == "Player") {
            EnemyController enemyScript = other.gameObject.GetComponent<EnemyController>();
            if (!enemyScript.IsAlerted() && !enemyScript.IsDetectingPlayer()) {
                enemyScript.TakeDmg(5);
                sounds.Play("ShurikenStealthKill");
            } else {
                enemyScript.TakeDmg(1);
                sounds.Play("ShurikenBodyHit");
            }
            StartCoroutine(Contact(true));
        } else if (other.gameObject.tag == "Player" && this.owner == "Enemy") {
            Health playerHealth = PlayerController.singleton.GetComponent<Health>();
            playerHealth.TakeDmg(1, this.transform.position);
            sounds.Play("ShurikenBodyHit");
            StartCoroutine(Contact(true));
        }
    }

    // Creates spark on contact and then destroys shuriken.
    IEnumerator Contact(bool hitEntity) {
        isActive = false;
        anim.enabled = false;
        hasContact = true;
        rb.velocity = new Vector2(0f, 0f);

        sounds.Stop("Shuriken");
        if (hitEntity) {
            this.gameObject.layer = 12;
            shurikenSprite.enabled = false;
            trailRen.enabled = false;
            yield return new WaitForSeconds(1f);
            Destroy(this.gameObject);
        } else {
            sounds.Play("ShurikenGroundHit");
            CreateSparks();
            StartCoroutine("FadeAway");
        }
    }

    IEnumerator FadeAway() {
        yield return new WaitForSeconds(fadeAwayDelay);
        for (float alpha = 1f; alpha > 0f; alpha -= Time.deltaTime * fadeAwaySpeed) {
            shurikenSprite.color = new Color(1f, 1f, 1f, alpha);
            yield return new WaitForEndOfFrame();
        }
    }
    #endregion

    #region Public Functions
    // Creates sparks when called.
    private void CreateSparks() {
        sparks.Play();
    }

    // Sets the owner of the shuriken
    public void SetOwner(string tag) {
        this.owner = tag;
    }

    // Sets the shuriken's velocity and direction.
    public void SetShurikenVelocity(Vector2 dir) {
        throwDir = dir;
        if (throwDir.x < 0f) {
            shurikenSprite.flipX = true;
            sparks.transform.rotation = Quaternion.Euler(0, 0, -90);
        } else {
            shurikenSprite.flipX = false;
            sparks.transform.rotation = Quaternion.Euler(0, 0, 90);
        }
        rb.velocity = throwDir * shurikenSpeed;
        sounds.Play("Shuriken");
    }
    #endregion
}
