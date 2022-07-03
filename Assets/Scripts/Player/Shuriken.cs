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

    private float shurikenSpeed = 20;
    private float destroyDelay = 0.0001f;
    private float deflectedMultiplier = 0.5f;
    private int numDeflections = 0;
    private int deflectCounter;
    private int throwDir;
    private bool isActive;
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
    }

    private void Start() {
        player = GameObject.Find("Player");
        playerScript = player.GetComponent<PlayerController>();
        meleeScript = player.transform.GetChild(1).GetComponent<Melee>();

        isActive = true;
    }
    #endregion

    #region Collision Functions
    // When the shuriken hits an enemy
    private void OnTriggerEnter2D(Collider2D other) {
        if (isActive) {
            if (other.gameObject.tag == "Enemy") {
            EnemyController enemyScript = other.gameObject.GetComponent<EnemyController>();
            if (!enemyScript.IsAlerted()) {
                enemyScript.TakeDmg(5);
            } else {
                enemyScript.TakeDmg(1);
            }
        }
            if (other.gameObject.tag == "Enemy") {
                StartCoroutine(Contact(true));
            } else if (other.gameObject.tag == "Platform") {
                StartCoroutine(Contact(false));
            }
        }
    }

    // Creates spark on contact and then destroys shuriken.
    IEnumerator Contact(bool hitEnemy) {
        isActive = false;
        anim.enabled = false;
        meleeScript.RemoveProjFromList(col);
        rb.velocity = new Vector2(0f, 0f);

        if (hitEnemy) {
            yield return new WaitForSeconds(destroyDelay);
        } else {
            CreateSparks();
            yield return new WaitForSeconds(1f);
        }
    
        Destroy(this.gameObject);
    }
    #endregion

    #region Public Functions
    // Creates sparks when called.
    private void CreateSparks() {
        sparks.Play();
    }

    // Sets the shuriken's velocity and direction.
    public void SetShurikenVelocity(int dir) {
        throwDir = dir;
        if (throwDir == -1) {
            shurikenSprite.flipX = true;
            sparks.transform.rotation = Quaternion.Euler(0, 0, -90);
        } else {
            shurikenSprite.flipX = false;
            sparks.transform.rotation = Quaternion.Euler(0, 0, 90);
        } 
        rb.velocity = new Vector2(throwDir * shurikenSpeed * (1 + deflectedMultiplier * numDeflections), 0);
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
