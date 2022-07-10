using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    #region Movement Variables
    [Header("Movement")]
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
    [Tooltip("The vertical jump velocity of the enemy when pursuing.")]
    private float jumpVelY;
    [SerializeField]
    [Tooltip("The horizontal airborne velocity of the enemy when pursuing.")]
    private float airborneVelX;
    [SerializeField]
    [Tooltip("Dust particle effect when on the ground.")]
    private ParticleSystem groundDust;
    [Space(5)]
    #endregion
    
    #region Health Variables
    [Header("Health")]
    [SerializeField]
    [Tooltip("How much health this enemy has.")]
    private int enemyHealth;
    [SerializeField]
    [Tooltip("The delay before the enemy is destroyed")]
    private float destroyDelay;
    [SerializeField]
    [Tooltip("The amount this enemy's death will increse your score.")]
    private int enemyPoints;
    [Space(5)]
    #endregion

    #region Attack Variables
    [Header("Attack")]
    [SerializeField]
    [Tooltip("How often the enemy can attack.")]
    private float attackRate;
    [SerializeField]
    [Tooltip("How much damage this enemy deals per hit.")]
    private int dmg;
    [Tooltip("The speed a shuriken flies through the air.")]
    private float shurikenSpeed;
    [SerializeField]
    [Tooltip("The delay of spawning the shuriken.")]
    private float spawnDelay;
    [Space(5)]
    #endregion

    #region Knockback Variables
    [Header("Knockback")]
    [SerializeField]
    [Tooltip("The amount of force applied to enemy when getting hit.")]
    private float knockBackForce;
    [SerializeField]
    [Tooltip("The duration in which knockback is applied.")]
    private float knockBackDur;
    [Space(5)]
    #endregion

    #region GameObject Variables
    [Header("GameObjects")]
    [SerializeField]
    [Tooltip("The line of sight prefab for player detection.")]
    private GameObject lineOfSightObj;
    [SerializeField]
    [Tooltip("The shuriken prefab.")]
    private GameObject shurikenPrefab;
    [Space(5)]
    #endregion
    
    #region Private Variables
    // Player cached references
    private PlayerController playerScript;
    private Health playerHealthScript;
    private Melee meleeScript;

    // Cached components
    private GameObject alertedObj;
    private Transform firePointTrans;
    private SpriteRenderer alertedSprite;
    private SpriteRenderer enemySprite;
    private PolygonCollider2D alertedCol;
    private Animator enemyAnim;
    private BoxCollider2D enemyCollider;
    private Rigidbody2D enemyRB;
    private AStar astarScript;
    private FieldOfView fov;
    private MeleeEnemy meleeEnemyScript;

    // Private variables
    private LayerMask allPlatformsLayerMask;
    private Vector2 adjustedPos;
    private int damageCounter;
    private int startingDir;
    private float lastIdle;
    private float lastAttack;

    // Pathfinding Variables
    private List<Vector2> pursuePath;
    private List<Vector2> newPath;
    private List<Vector2> patrolPath;
    private int currPathIndex;
    private float leftPatrolEnd;
    private float rightPatrolEnd;

    // Condition Variables
    private bool isAlerted;
    private bool hasDied;
    private bool isStunned;
    private bool isGrounded;
    private bool isIdling;
    private bool playerIsInMeleeRange;
    private bool isMeleeing;
    #endregion

    #region Initializaiton Functions
    void Awake() {
        alertedObj = this.transform.GetChild(0).gameObject;
        alertedSprite = alertedObj.GetComponent<SpriteRenderer>();
        enemySprite = this.GetComponent<SpriteRenderer>();
        alertedCol = alertedObj.GetComponent<PolygonCollider2D>();
        enemyCollider = this.GetComponent<BoxCollider2D>();
        enemyRB = this.GetComponent<Rigidbody2D>();
        enemyAnim = this.GetComponent<Animator>();
        astarScript = this.GetComponent<AStar>();
        meleeEnemyScript = this.transform.GetChild(2).GetComponent<MeleeEnemy>();
        firePointTrans = this.transform.GetChild(3).transform;
    }

    // Start is called before the first frame update
    void Start()
    {
        playerScript = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        playerHealthScript = playerScript.gameObject.GetComponent<Health>();
        meleeScript = playerScript.transform.GetChild(1).GetComponent<Melee>();
        allPlatformsLayerMask = LayerMask.GetMask("Platform", "OneWayPlatform");

        // Determines whether the enemy will begin patrolling left or right.
        float value = Random.Range(0, 1);
        if (value < 0.5) {
            startingDir = -1;
        } else {
            startingDir = 1;
        }

        GameObject lineOfSight = GameObject.Instantiate(lineOfSightObj);
        fov = lineOfSight.GetComponent<FieldOfView>();
        fov.InitializeEnemyScript(this);

        adjustedPos = astarScript.GetAdjustedPosition();
        patrolPath = astarScript.CalculatePatrolPath(maxNodeDist);
        leftPatrolEnd = adjustedPos.x - maxNodeDist;
        rightPatrolEnd = adjustedPos.x + maxNodeDist;
        InvokeRepeating("UpdatePursuePath", 0f, 0.5f);

    }
    #endregion

    private void Update() {
        if (!hasDied) {
            fov.SetOrigin(transform.position);
            IsGrounded();
            adjustedPos = astarScript.GetAdjustedPosition();
            if (!isStunned) {
                SetDirection();
            }
            Attack();
            Move();
        }
        UpdateSprite();
        Debug.Log("InMeleeRange: " + playerIsInMeleeRange);
    }

    #region Movement Functions
    // Controls the enemy's movments.
    private void Move() {
        // Clamps the enemie's vertical velocity to 25
        enemyRB.velocity = new Vector2(enemyRB.velocity.x, Mathf.Clamp(enemyRB.velocity.y, -25, 25));

        if (!isAlerted) {
            Patrol();
        } else if (!isStunned && !isMeleeing) {
            Pursue();
        }
    }

    // The enemy's patrolling state
    private void Patrol() {
        if (patrolPath == null) {
            enemyRB.velocity = new Vector2(0f, 0f);
        } else if ((adjustedPos.x < patrolPath[1].x && startingDir == 1) || 
                   (adjustedPos.x > patrolPath[0].x && startingDir == -1)) {
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
        // Move/Drop right
        if (dir.x > 0 && dir.y <= 0) {
            enemyRB.velocity = new Vector2(pursueSpeed, enemyRB.velocity.y);
        // Move/Drop left
        } else if (dir.x < 0 && dir.y <= 0) {
            enemyRB.velocity = new Vector2(-pursueSpeed, enemyRB.velocity.y);
        // Jump right
        } else if (dir.x > 0 && dir.y > 0) {
            enemyRB.velocity = new Vector2(airborneVelX, jumpVelY);
        // Jump left
        } else if (dir.x < 0 && dir.y > 0) {
            enemyRB.velocity = new Vector2(-airborneVelX, jumpVelY);
        }
        // Create dust when running on the ground.
        if (Mathf.Abs(enemyRB.velocity.x) > 0f && isGrounded) {
            CreateDust();
        }

        // Set horizontal velocity when falling.
        if (enemyRB.velocity.y < -0.05) {
            if (enemyRB.velocity.x < -0.05) {
                enemyRB.velocity = new Vector2(-airborneVelX, enemyRB.velocity.y);
            } else if (enemyRB.velocity.x > 0.05) {
                enemyRB.velocity = new Vector2(airborneVelX, enemyRB.velocity.y);
            }
        }

        // Increment path counter if enemy has reached the current path node.
        if (adjustedPos == pursuePath[currPathIndex]) {
            currPathIndex++;
        }
    }

    // Updates the enemy's pursue path.
    private void UpdatePursuePath() {
        newPath = astarScript.CalculatePath();
        if (newPath != null) {
            pursuePath = newPath;
            currPathIndex = 0;
        }
    }

    void CreateDust() {
        groundDust.Play();
    }
    #endregion

    // Controls whether or not an enemy is alerted to the player's presence.
    // private void OnTriggerEnter2D(Collider2D other) {
    //     if (other.gameObject.tag == "Player" && other.IsTouching(unalertedCol) && !playerScript.GetHidingStatus() && !hasDied) {
    //         isAlerted = true;
    //         alertedSprite.color = new Color(1f, 0f, 0f, 0.18f);
    //     }
    // }

    // Knocks the enemy back when getting damaged.
    IEnumerator KnockBack(Vector2 playerDir) {
        isStunned = true;
        enemyRB.velocity = new Vector2(0f, 0f);
        enemyRB.AddForce(playerDir * knockBackForce, ForceMode2D.Impulse);
        yield return new WaitForSeconds(knockBackDur);
        isStunned = false;
    }

    // Kills the enemy
    IEnumerator Death() {
        hasDied = true;
        if (!isAlerted) {
            ScoreManager.singleton.IncreaseScore(enemyPoints * 2);
        } else {
            ScoreManager.singleton.IncreaseScore(enemyPoints);
        }
        alertedSprite.color = new Color(1f, 0f, 0f, 0f);
        Destroy(fov.gameObject);
        //enemyRB.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezePositionY | RigidbodyConstraints2D.FreezeRotation;
        //enemyCollider.isTrigger = true;

        // 7 = Player Layer, 9 = Enemy Layer
        Physics2D.IgnoreLayerCollision(7, 9, true);
        yield return new WaitForSeconds(destroyDelay);
        meleeScript.RemoveEnemyFromList(enemyCollider);
        Destroy(this.gameObject);
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
            fov.SetStartingAngle(15f);
        } else if (enemyRB.velocity.x < -0.05f) {
            transform.localScale = new Vector3(-1f * Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
            fov.SetStartingAngle(200f);
        }
    }

    #region Attack Functions
    private void Attack() {
        playerIsInMeleeRange = meleeEnemyScript.GetPlayerContactStatus();
        if (playerIsInMeleeRange && isGrounded && CanAttack()) {
            isMeleeing = true;
            playerHealthScript.TakeDmg(dmg, this.transform.position);
        // } else if (playerIsInThrowingRange && isGrounded && CanAttack()) {
        //     isMeleeing = false;
        // }
        }
    }

    private bool CanAttack() {
        return (lastAttack + attackRate <= Time.time) && !isStunned;
    }
    #endregion

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

        if(hasDied) {
            enemyAnim.SetBool("hasDied", true);
        } else {
            enemyAnim.SetBool("hasDied", false);
        }

        // if (firePressed && CanAttack() && numShurikens > 0 && !isDashing) {
        //     isAttacking = true;
        //     enemyAnim.SetBool("isThrowing", true);
        //     lastAttack = Time.time;
        //     numShurikens -= 1;
        //     shurikenTxt.text = "Shurikens: " + numShurikens.ToString();
        //     Invoke("SetIsThrowingFalse", 0.5f);
        // }

        if (isMeleeing) {
           // isAttacking = true;
            enemyAnim.SetBool("isMeleeing", true);
            lastAttack = Time.time;
            Invoke("SetIsMeleeingFalse", 0.5f);
        }

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

    private void SetIsMeleeingFalse() {
        enemyAnim.SetBool("isMeleeing", false);
        //isAttacking = false;
    }
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

    // Sets the alert status of the enemy
    public void SetAlertStatus(bool status) {
        isAlerted = status;
        if (isAlerted) {
            alertedSprite.color = new Color(1f, 0f, 0f, 0.18f);
        }
    }

    // Reduces the enemy's health by dmg.
    public void TakeDmg(int dmg) {
        enemyHealth -= dmg;
        StartCoroutine(KnockBack(new Vector2(playerScript.GetPlayerDir(), 0f)));
        if (enemyHealth <= 0) {
            StartCoroutine("Death");
        } else {
            isAlerted = true;
        }
    }
    #endregion
}
