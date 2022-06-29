using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    // Player cahced references
    private GameObject player;
    private PlayerController playerScript;
    private Melee meleeScript;

    // Cached components
    private GameObject unalertedObj;
    private GameObject alertedObj;
    private SpriteRenderer unalertedSprite;
    private SpriteRenderer alertedSprite;
    private SpriteRenderer enemySprite;
    private PolygonCollider2D unalertedCol;
    private PolygonCollider2D alertedCol;
    private Animator enemyAnim;
    private BoxCollider2D enemyCollider;
    private Rigidbody2D enemyRB;

    private Transform firePointTrans;
    private RectTransform meleePointRectTrans;

    // Private variables
    private int enemyHealth;
    private bool isAlerted;
    private bool playerDetected;

    public float knockBackDur;
    public float knockBackForce;

    public float attackSpeed;
    public float dmg;

    public float firePointDist;
    public float meleePointDist;

    public float destroyDelay;
    private bool hasDied;

    //private int lastDir;
    //private int lastMeleeDir;

    public float stunDuration;
    public float stunForce;
    public float stunnedGravity;

    // The player's meleeCounter that damaged the enemy
    private int damageCounter;

    private bool isStunned;
    private bool isGrounded;

    private int lastPlayerDir;

    public LayerMask allPlatformsLayerMask;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        playerScript = player.GetComponent<PlayerController>();
        meleeScript = player.transform.GetChild(1).GetComponent<Melee>();

        unalertedObj = this.transform.GetChild(0).gameObject;
        alertedObj = this.transform.GetChild(1).gameObject;
        unalertedSprite = unalertedObj.GetComponent<SpriteRenderer>();
        alertedSprite = alertedObj.GetComponent<SpriteRenderer>();
        enemySprite = this.GetComponent<SpriteRenderer>();
        unalertedCol = unalertedObj.GetComponent<PolygonCollider2D>();
        alertedCol = alertedObj.GetComponent<PolygonCollider2D>();
        enemyCollider = this.GetComponent<BoxCollider2D>();
        enemyRB = this.GetComponent<Rigidbody2D>();
        enemyAnim = this.GetComponent<Animator>();
        firePointTrans = this.transform.GetChild(4).transform;
        meleePointRectTrans = this.transform.GetChild(3).GetComponent<RectTransform>();

        allPlatformsLayerMask = LayerMask.GetMask("Platform", "OneWayPlatform");

        isAlerted = false;
        enemyHealth = 1000;
        hasDied = false;
        //lastMeleeDir = 1;
        //lastDir = 1;
    }

    private void Update() {
        if (!hasDied) {
            //setDirection();
            isGrounded = IsGrounded();
            enemyRB.velocity = new Vector2(enemyRB.velocity.x, Mathf.Clamp(enemyRB.velocity.y, -25, 25));
            UpdateSprite();
        }
    }

    // Returns whether or not an enemy is alerted to the player's presence.
    private void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.tag == "Player" && other.IsTouching(unalertedCol) && !playerScript.GetHidingStatus()) {
            isAlerted = true;
            unalertedSprite.color = new Color(1f, 1f, 0f, 0);
            alertedSprite.color = new Color(1f, 0f, 0f, 0.18f);
        }
    }

    // Returns if enemy is in alerted state.
    public bool IsAlerted() {
        return isAlerted;
    }

    // Reduces the enemy's health by dmg.
    public void TakeDmg(int dmg, bool wasMeleed) {
        enemyHealth -= dmg;
        if (wasMeleed) {
            StartCoroutine(KnockBack(new Vector2(playerScript.GetPlayerDir(), 0f)));
        }
        if (enemyHealth <= 0) {
            hasDied = true;
            Invoke("DestroyEnemy", destroyDelay);
        }
    }

    // Removes the enemy from the player's melee collider list and destroys the enemy.
    private void DestroyEnemy() {
        meleeScript.RemoveEnemyFromList(enemyCollider);
        Destroy(this.gameObject);
    }

    // Returns the enemy's current health.
    public int GetHealth() {
        return this.enemyHealth;
    }
    
    public bool HasDied() {
        return this.hasDied;
    }

    // Assigns the player's melee counter to the enemy after being damaged.
    public void SetDamagedCounter(int counter) {
        this.damageCounter = counter;
    }

    // Checks to see if enemy has already been damaged by player's current meleeCounter.
    public bool HasBeenDamaged(int counter) {
        return this.damageCounter == counter;
    }

    IEnumerator KnockBack(Vector2 playerDir) {
        isStunned = true;
        enemyRB.velocity = new Vector2(0f, 0f);
        enemyRB.AddForce(playerDir * knockBackForce, ForceMode2D.Impulse);
        yield return new WaitForSeconds(knockBackDur);
        isStunned = false;
    }

    // Sets the variable: lastDir based on the velocity of the enemy.
    // private void setDirection() {
    //     int currDir = lastDir;
    //     if (enemyRB.velocity.x > 0.001) {
    //         lastDir = 1;
    //     } else if (enemyRB.velocity.x < -0.001) {
    //         lastDir = -1;
    //     }

    //     if (currDir != lastDir) {
    //         SwitchChildPositions();
    //     }
    // }

    // Switches the attack point gameObject of the enemy based on direction.
    // private void SwitchChildPositions() {
    //     Vector3 firePos = firePointTrans.position;
    //     Vector3 meleePos = meleePointRectTrans.position;
    //     if (lastDir == -1) {
    //         firePos.x = this.transform.position.x - firePointDist;
    //         meleePos.x = this.transform.position.x - meleePointDist;
    //     } else {
    //         firePos.x = this.transform.position.x + firePointDist;
    //         meleePos.x = this.transform.position.x + meleePointDist;
    //     }
    //     if (lastDir != lastMeleeDir) {
    //         meleePointRectTrans.Rotate(new Vector3(0, 180, 0), Space.Self);
    //         lastMeleeDir = lastDir;
    //     }
    //     firePointTrans.position = firePos;
    //     meleePointRectTrans.position = meleePos;
    // }

    private bool IsGrounded() {
        RaycastHit2D raycastHit2D = Physics2D.BoxCast(enemyCollider.bounds.center, new Vector2(0.6f, enemyCollider.bounds.size.y - 0.1f), 0f, Vector2.down, 0.2f, allPlatformsLayerMask);
        bool onGround = raycastHit2D.collider != null;
        return onGround;
    }

    // Stuns enemy when getting hit
    // private IEnumerator Stunned(Vector2 dir) {
    //     isStunned = true;
    //     enemyRB.velocity = new Vector2(0f, 0f);
    //     enemyRB.gravityScale = stunnedGravity;
    //     enemyRB.AddForce(dir * stunForce, ForceMode2D.Impulse);
    //     yield return new WaitForSeconds(stunDuration);
    //     enemyRB.gravityScale = gravity;
    //     isStunned = false;
    // }

    #region Sprite Rendering Functions
    // Updates the player's sprites based on input/state.
    private void UpdateSprite() {
        // if (lastDir == 1 && !isStunned) {
        //     enemySprite.flipX = false;
        // } else if (lastDir == -1 && !isStunned) {
        //     enemySprite.flipX = true;
        // }

        if (Mathf.Abs(enemyRB.velocity.x) > 0.001) {
            enemyAnim.SetBool("isMoving", true);
        } else {
            enemyAnim.SetBool("isMoving", false);
        }

        if (enemyRB.velocity.y > 0.001) {
            enemyAnim.SetBool("isJumping", true);
        } else {
            enemyAnim.SetBool("isJumping", false);
        }
        
        if (enemyRB.velocity.y < -0.001) {
            enemyAnim.SetBool("isFalling", true);
        } else {
            enemyAnim.SetBool("isFalling", false);
        }

        if (isGrounded) {
            enemyAnim.SetBool("isGrounded", true);
        } else {
            enemyAnim.SetBool("isGrounded", false);
        }

        if(isAlerted) {
            enemyAnim.SetBool("isAlerted", true);
        } else {
            enemyAnim.SetBool("isAlerted", false);
        }

        // if (firePressed && CanAttack() && numShurikens > 0 && !isDashing) {
        //     isAttacking = true;
        //     enemyAnim.SetBool("isThrowing", true);
        //     lastAttack = Time.time;
        //     numShurikens -= 1;
        //     shurikenTxt.text = "Shurikens: " + numShurikens.ToString();
        //     Invoke("SetIsThrowingFalse", 0.5f);
        // }

        // if (meleePressed && CanAttack() && isGrounded) {
        //     isAttacking = true;
        //     enemyAnim.SetBool("isMeleeing", true);
        //     lastAttack = Time.time;
        //     Invoke("SetIsMeleeingFalse", 0.5f);
        // }

        if (isStunned) {
            enemyAnim.SetBool("isStunned", true);
        } else {
            enemyAnim.SetBool("isStunned", false);
        }
    }

    // private void SetIsThrowingFalse() {
    //     playerAnim.SetBool("isThrowing", false);
    //     isAttacking = false;
    // }

    // private void SetIsMeleeingFalse() {
    //     playerAnim.SetBool("isMeleeing", false);
    //     isAttacking = false;
    // }
    #endregion





}
