using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shuriken : MonoBehaviour
{
    [SerializeField]
    [Tooltip("The color of the shuriken's trail after being deflected once.")]
    private Gradient deflectedOnceColor;
    [SerializeField]
    [Tooltip("The color of the shuriken's trail after being deflected more than once.")]
    private Gradient deflectedTwiceColor;
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
    private GameObject player;
    private PlayerController playerScript;
    private Melee meleeScript;
    private SpriteRenderer shurikenSprite;
    private ParticleSystem sparks;
    private Animator anim;
    private SoundManager sounds;

    private float shurikenSpeed = 20;
    //private float destroyDelay = 0.0001f;
    private float deflectedMultiplier = 0.5f;
    private int numDeflections = 0;
    private float noContactDestroyTime = 5f;
    private float currflightTime;
    private int deflectCounter;
    private Vector2 throwDir;
    private bool isActive;
    private bool hasContact;
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
        player = GameObject.Find("Player");
        playerScript = player.GetComponent<PlayerController>();
        meleeScript = player.transform.GetChild(1).GetComponent<Melee>();

        isActive = true;
    }

    private void Update() {
        if(!hasContact) {
            currflightTime += Time.deltaTime;
        }

        if (currflightTime >= noContactDestroyTime) {
            meleeScript.RemoveProjFromList(col);
            Destroy(this.gameObject);
        }
    }
    #endregion

    #region Collision Functions
    // When the shuriken hits an enemy
    private void OnTriggerEnter2D(Collider2D other) {
        if (isActive) {
            if (other.gameObject.tag == "Enemy") {
                EnemyController enemyScript = other.gameObject.GetComponent<EnemyController>();
                if (!enemyScript.IsAlerted() && !enemyScript.IsDetectingPlayer()) {
                    enemyScript.TakeDmg(5);
                    sounds.Play("ShurikenStealthKill");
                } else {
                    enemyScript.TakeDmg(1);
                }
            }
            if (other.gameObject.tag == "Enemy") {
                StartCoroutine(Contact(true));
            } else if (other.gameObject.tag == "Platform" || other.gameObject.tag == "TrapDoor") {
                StartCoroutine(Contact(false));
            }
        }
    }

    // Creates spark on contact and then destroys shuriken.
    IEnumerator Contact(bool hitEnemy) {
        isActive = false;
        anim.enabled = false;
        hasContact = true;
        meleeScript.RemoveProjFromList(col);
        rb.velocity = new Vector2(0f, 0f);

        sounds.Stop("Shuriken");
        if (hitEnemy) {
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
        rb.velocity = throwDir * shurikenSpeed * (1 + deflectedMultiplier * numDeflections);
        //SoundManager.singleton.PlayShuriken();
        sounds.Play("Shuriken");
    }

    // Sends the shuriken in the other direction when deflected.
    public void Deflected() {
        numDeflections += 1;
        if (numDeflections == 1) {
           // Change trail color orange
           trailRen.colorGradient = deflectedOnceColor;
        }  else if (numDeflections == 2) {
            // Change trail color red
            trailRen.colorGradient = deflectedTwiceColor;
            deflectedMultiplier += 0.5f;
        }
        SetShurikenVelocity(-throwDir);
    }

    // Checks to see if shuriken has already been deflected by player's current meleeCounter.
    public bool HasBeenDeflected(int counter) {
        return this.deflectCounter == counter;
    }

    // Assigns the player's melee counter to the shuriken after being deflected.
    public void SetDeflectedCounter(int counter) {
        this.deflectCounter = counter;
    }
    #endregion
}
