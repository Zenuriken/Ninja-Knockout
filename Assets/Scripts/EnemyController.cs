using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    #region Movement Variables
    [SerializeField]
    [Tooltip("The number of tiles the enemy can stray from starting pos while in patrol mode.")]
    private int maxNodeDist;
    [SerializeField]
    [Tooltip("How long the enemy is in an idle state before patrolling.")]
    private float idleDur;
    [SerializeField]
    [Tooltip("The speed of the enemy when patrolling.")]
    private float patrolSpeed;
    [SerializeField]
    [Tooltip("The speed of the enemy when pursuing.")]
    private float pursueSpeed;
    [SerializeField]
    [Tooltip("The jump velocity of the enemy when pursuing.")]
    private float jumpVel;

    #endregion
    
    #region Health Variables
    [SerializeField]
    [Tooltip("How much health this enemy has.")]
    private int enemyHealth;
    [SerializeField]
    [Tooltip("The dealy before the enemy is destroyed")]
    private float destroyDelay;
    #endregion

    #region Attack Variables
    [SerializeField]
    [Tooltip("How often the enemy can attack.")]
    private float attackSpeed;
    [SerializeField]
    [Tooltip("How much damage this enemy deals per hit.")]
    private float dmg;
    #endregion

    #region Knockback Variables
    [SerializeField]
    [Tooltip("The amount of force applied to enemy when getting hit.")]
    private float knockBackForce;
    [SerializeField]
    [Tooltip("The duration in which knockback is applied.")]
    private float knockBackDur;
    #endregion
    
    #region Private Variables
    // Player cached references
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
    private AStar astarScript;

    // Private variables
    public LayerMask allPlatformsLayerMask;
    private int damageCounter;
    private int startingDir;
    private Vector2 adjustedPos;
    private float lastIdle;

    // Pathfinding Variables
    private List<Vector2> pursuePath;
    private List<Vector2> newPath;
    private int currPathIndex;

    // Condition Variables
    private bool isAlerted;
    private bool hasDied;
    private bool isStunned;
    private bool isGrounded;
    private bool isIdling;
    #endregion

    #region Initializaiton Functions
    void Awake() {
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
        astarScript = this.GetComponent<AStar>();
    }

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        playerScript = player.GetComponent<PlayerController>();
        meleeScript = player.transform.GetChild(1).GetComponent<Melee>();
        allPlatformsLayerMask = LayerMask.GetMask("Platform", "OneWayPlatform");

        // Determines whether the enemy will begin patrolling left or right.
        float value = Random.Range(0, 1);
        if (value < 0.5) {
            startingDir = -1;
        } else {
            startingDir = 1;
        }

        InvokeRepeating("UpdatePursuePath", 0f, 0.5f);

    }
    #endregion

    private void Update() {
        if (!hasDied) {
            IsGrounded();
            SetDirection();
            adjustedPos = astarScript.GetAdjustedPosition();
            Move();
            UpdateSprite();
        }
    }

    #region Movement Functions
    // Controls the enemy's movments.
    private void Move() {
        // Clamps the enemie's vertical velocity to 25
        enemyRB.velocity = new Vector2(enemyRB.velocity.x, Mathf.Clamp(enemyRB.velocity.y, -25, 25));

        if (!isAlerted) {
            Patrol();
        } else {
            Pursue();
        }
    }

    // The enemy's patrolling state
    private void Patrol() {
        List<Vector2> pathList = astarScript.CalculatePatrolPath(maxNodeDist);
        if (pathList == null) {
            enemyRB.velocity = new Vector2(0f, 0f);
        } else if ((adjustedPos.x < pathList[1].x && startingDir == 1) || 
                   (adjustedPos.x > pathList[0].x && startingDir == -1)) {
            enemyRB.velocity = new Vector2(startingDir * patrolSpeed, 0f);
        } else if (CanIdle()) {
            StartCoroutine("Idle");
        }
    }

    // Controls the player's Idle state when reaching the end of a platform.
    IEnumerator Idle() {
        lastIdle = Time.time;
        enemyRB.velocity = new Vector2(0f, 0f);
        yield return new WaitForSeconds(idleDur);
        startingDir *= -1;
    }

    // Returns whether the enemy can start idling at the end of a platform
    private bool CanIdle() {
        return (lastIdle + idleDur * 2f) < Time.time;
    }

    // The enemy's pursuing state.
    private void Pursue() {
        if (pursuePath == null || currPathIndex >= pursuePath.Count) {
            return;
        }

        Vector2 nextPos = pursuePath[currPathIndex];
        Vector2 dir = (nextPos - adjustedPos).normalized;
        //Debug.Log("CurrPos: " + adjustedPos.ToString() + "     NextPos: " + nextPos.ToString());
        // Move/Drop right
        if (dir.x > 0 && dir.y <= 0) {
            enemyRB.velocity = new Vector2(pursueSpeed, enemyRB.velocity.y);
        // Move/Drop left
        } else if (dir.x < 0 && dir.y <= 0) {
            enemyRB.velocity = new Vector2(-pursueSpeed, enemyRB.velocity.y);
        // Jump right
        } else if (dir.x > 0 && dir.y > 0) {
            enemyRB.velocity = new Vector2(pursueSpeed, jumpVel);
        // Jump left
        } else if (dir.x < 0 && dir.y > 0) {
            enemyRB.velocity = new Vector2(-pursueSpeed, jumpVel);
        } else {
            Debug.Log(dir);
        }

        // Increment path counter if enemy has reached the current path node.
        if (adjustedPos == pursuePath[currPathIndex]) {
            currPathIndex++;
        }
    }


    // Calculates and returns the distance between two positions
    private float CalcDist(Vector2 p0, Vector2 p1) {
        return 0;
    }

    // Updates the enemy's pursue path.
    private void UpdatePursuePath() {
        newPath = astarScript.CalculatePath();
        if (newPath != null) {
            pursuePath = newPath;
            currPathIndex = 0;
        }
    }
    #endregion

    // Controls whether or not an enemy is alerted to the player's presence.
    private void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.tag == "Player" && other.IsTouching(unalertedCol) && !playerScript.GetHidingStatus()) {
            isAlerted = true;
            unalertedSprite.color = new Color(1f, 1f, 0f, 0);
            alertedSprite.color = new Color(1f, 0f, 0f, 0.18f);
        }
    }

    // Removes the enemy from the player's melee collider list and destroys the enemy.
    private void DestroyEnemy() {
        meleeScript.RemoveEnemyFromList(enemyCollider);
        Destroy(this.gameObject);
    }

    // Knocks the enemy back when getting damaged.
    IEnumerator KnockBack(Vector2 playerDir) {
        isStunned = true;
        enemyRB.velocity = new Vector2(0f, 0f);
        enemyRB.AddForce(playerDir * knockBackForce, ForceMode2D.Impulse);
        yield return new WaitForSeconds(knockBackDur);
        isStunned = false;
    }

    // Sets whether the enemy is grounded.
    private void IsGrounded() {
        RaycastHit2D raycastHit2D = Physics2D.BoxCast(enemyCollider.bounds.center, new Vector2(0.6f, enemyCollider.bounds.size.y - 0.1f), 0f, Vector2.down, 0.2f, allPlatformsLayerMask);
        bool onGround = raycastHit2D.collider != null;
        isGrounded = onGround;
    }

    // Sets the direction of the enemy to where it's moving.
    private void SetDirection() {
        if (enemyRB.velocity.x > 0.05f) {
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        } else if (enemyRB.velocity.x < -0.05f) {
            transform.localScale = new Vector3(-1f * Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
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

        if (Mathf.Abs(enemyRB.velocity.x) > 0.05f) {
            enemyAnim.SetBool("isMoving", true);
        } else {
            enemyAnim.SetBool("isMoving", false);
        }

        if (enemyRB.velocity.y > 0.05f) {
            enemyAnim.SetBool("isJumping", true);
        } else {
            enemyAnim.SetBool("isJumping", false);
        }
        
        if (enemyRB.velocity.y < -0.05f) {
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

    #region Public Functions
    // Returns the enemy's current health.
    public int GetHealth() {
        return this.enemyHealth;
    }
    
    // Returns if the enemy has died.
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
        } else {
            isAlerted = true;
        }
    }
    #endregion
}
