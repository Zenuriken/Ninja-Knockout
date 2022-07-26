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
    private float pursueAirborneVelX;
    [SerializeField]
    [Tooltip("The horizontal airborne velocity of the enemy when patrolling.")]
    private float patrolAirborneVelX;
    [SerializeField]
    [Tooltip("Dust particle effect when on the ground.")]
    private ParticleSystem groundDust;
    [SerializeField]
    [Tooltip("Question mark effect when pursuing player.")]
    private ParticleSystem questionMark;
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
    //private SpriteRenderer alertedSprite;
    private SpriteRenderer enemySprite;
    private PolygonCollider2D alertedCol;
    private Animator enemyAnim;
    private BoxCollider2D enemyCollider;
    private Rigidbody2D enemyRB;
    private AStar astarScript;
    private FieldOfView fov;
    private MeleeEnemy meleeEnemyScript;
    private AlertedSight alertedSightScript;

    // Private variables
    private LayerMask allPlatformsLayerMask;
    private LayerMask playerAndPlatformLayerMask;
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
    public bool isAlerted;
    private bool hasDied;
    private bool isStunned;
    private bool isGrounded;
    private bool isIdling;
    private bool playerIsInMeleeRange;
    private bool playerIsInThrowingRange;
    private bool isMeleeing;
    private bool isThrowing;
    private bool unreachable;
    private bool isReturningToPatrolPos;
    #endregion

    #region Initializaiton Functions
    void Awake() {
        alertedObj = this.transform.GetChild(0).gameObject;
        //alertedSprite = alertedObj.GetComponent<SpriteRenderer>();
        alertedCol = alertedObj.GetComponent<PolygonCollider2D>();
        alertedSightScript = this.transform.GetChild(0).GetComponent<AlertedSight>();

        enemySprite = this.GetComponent<SpriteRenderer>();
        enemyCollider = this.GetComponent<BoxCollider2D>();
        enemyRB = this.GetComponent<Rigidbody2D>();
        enemyAnim = this.GetComponent<Animator>();
        astarScript = this.GetComponent<AStar>();
        meleeEnemyScript = this.transform.GetChild(1).GetComponent<MeleeEnemy>();
        firePointTrans = this.transform.GetChild(2).transform;
    }

    // Start is called before the first frame update
    void Start()
    {
        playerScript = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        playerHealthScript = playerScript.gameObject.GetComponent<Health>();
        meleeScript = playerScript.transform.GetChild(1).GetComponent<Melee>();
        allPlatformsLayerMask = LayerMask.GetMask("Platform", "OneWayPlatform");
        playerAndPlatformLayerMask = LayerMask.GetMask("Player", "Platform", "OneWayPlatform");

        // Determines whether the enemy will begin patrolling left or right.
        float value = Random.Range(0, 100);
        if (value < 50) {
            startingDir = -1;
        } else {
            startingDir = 1;
        }
        Debug.Log("starting dir: " + value);

        GameObject lineOfSight = GameObject.Instantiate(lineOfSightObj);
        fov = lineOfSight.GetComponent<FieldOfView>();
        fov.InitializeEnemyScript(this);

        adjustedPos = astarScript.GetAdjustedPosition();
        patrolPath = astarScript.CalculatePatrolPath(maxNodeDist);
        leftPatrolEnd = adjustedPos.x - maxNodeDist;
        rightPatrolEnd = adjustedPos.x + maxNodeDist;
        InvokeRepeating("UpdatePursuePath", 0f, 0.5f);

        alertedObj.SetActive(false);

    }
    #endregion

    private void Update() {
        // Clamps the enemie's vertical velocity to 25
        enemyRB.velocity = new Vector2(enemyRB.velocity.x, Mathf.Clamp(enemyRB.velocity.y, -25, 25));

        if (!hasDied) {
            // Initializing states
            fov.SetOrigin(transform.position);
            IsGrounded();
            adjustedPos = astarScript.GetAdjustedPosition();

            // Executing Actions
            if (!isStunned && !isMeleeing && !isThrowing) {
                SetDirection();
                if (isAlerted && !playerScript.IsHiding()) {
                    Attack();
                }
                Move();
            }
        }
        UpdateSprite();
    }

    #region Movement Functions
    // Controls the enemy's movments.
    private void Move() {
        if (!isAlerted && !isReturningToPatrolPos) {
            Patrol();
        } else if (!isAlerted && isReturningToPatrolPos) {
            if (astarScript.IsAtSpawnPos()) {
                isReturningToPatrolPos = false;
            } else {
                Pursue(patrolSpeed);
            }
        } else if (isAlerted) {
            Pursue(pursueSpeed);
        }

        // Handles the case in which the enemy gets stuck on an edge.
        if (isAlerted && astarScript.IsStuck() && Mathf.Abs(enemyRB.velocity.x) < 0.5 && Mathf.Abs(enemyRB.velocity.y) < 0.5) {
            Vector2 moveDir = astarScript.GetMoveDir();
            enemyRB.velocity = new Vector2(moveDir.x * pursueSpeed, 0f);
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
    private void Pursue(float speed) {
        if (pursuePath == null || currPathIndex >= pursuePath.Count) {
            return;
        }
        Vector2 nextPos = pursuePath[currPathIndex];
        Vector2 dir = (nextPos - adjustedPos).normalized;
        // Move/Drop right
        if (dir.x > 0 && dir.y <= 0) {
            enemyRB.velocity = new Vector2(speed, enemyRB.velocity.y);
        // Move/Drop left
        } else if (dir.x < 0 && dir.y <= 0) {
            enemyRB.velocity = new Vector2(-speed, enemyRB.velocity.y);
        // Jump right
        } else if (dir.x > 0 && dir.y > 0) {
            if (speed == patrolSpeed) {
                enemyRB.velocity = new Vector2(patrolAirborneVelX, jumpVelY);
            } else {
                enemyRB.velocity = new Vector2(pursueAirborneVelX, jumpVelY);
            }
        // Jump left
        } else if (dir.x < 0 && dir.y > 0) {
            if (speed == patrolSpeed) {
                enemyRB.velocity = new Vector2(-patrolAirborneVelX, jumpVelY);
            } else {
                enemyRB.velocity = new Vector2(-pursueAirborneVelX, jumpVelY);
            }
        }
        // Create dust when running on the ground.
        if (Mathf.Abs(enemyRB.velocity.x) > 0f && isGrounded && speed == pursueSpeed) {
            CreateDust();
        }

        // Set horizontal velocity when falling.
        if (enemyRB.velocity.y < -0.05) {
            if (enemyRB.velocity.x < -0.05) {
                if (speed == patrolSpeed) {
                    enemyRB.velocity = new Vector2(-patrolAirborneVelX, enemyRB.velocity.y);
                } else {
                    enemyRB.velocity = new Vector2(-pursueAirborneVelX, enemyRB.velocity.y);
                }
            } else if (enemyRB.velocity.x > 0.05) {
                if (speed == patrolSpeed) {
                    enemyRB.velocity = new Vector2(patrolAirborneVelX, enemyRB.velocity.y);
                } else {
                    enemyRB.velocity = new Vector2(pursueAirborneVelX, enemyRB.velocity.y);
                }
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
            unreachable = false;
        } else {
            unreachable = true;
            if (playerScript.IsHiding() && Mathf.Abs(enemyRB.velocity.x) < 0.05f && isAlerted) {
                CreateQuestionMark();
                if (!isReturningToPatrolPos) {
                    StartCoroutine("ReturnToPatrols");
                }
            }

        }
        //Debug.Log(unreachable);
    }

    IEnumerator ReturnToPatrols() {
        Debug.Log("Returning!");
        isReturningToPatrolPos = true;
        yield return new WaitForSeconds(5f);
        if (playerScript.IsHiding() && Mathf.Abs(enemyRB.velocity.x) < 0.05f && isAlerted) {
            SetAlertStatus(false);
            astarScript.SetReturnToPatrolPos(true);
        }
    }

    void CreateDust() {
        groundDust.Play();
    }

    void CreateQuestionMark() {
        questionMark.Play();
    }
    #endregion

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
            ScoreManager.singleton.IncreaseScoreBy(enemyPoints * 2);
        } else {
            ScoreManager.singleton.IncreaseScoreBy(enemyPoints);
        }
        alertedObj.SetActive(false);
        if (playerIsInThrowingRange && isAlerted) {
            playerScript.IncreaseAlertedNumBy(-1);
        }
        Destroy(fov.gameObject);
        this.gameObject.layer = 12; // Set the gameObject to layer 12

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
        playerIsInMeleeRange = meleeEnemyScript.IsTouchingMeleeTrigger();
        playerIsInThrowingRange = alertedSightScript.IsTouchingAlertedTrigger();
        if (playerIsInMeleeRange && isGrounded && CanAttack()) {
            isMeleeing = true;
            playerHealthScript.TakeDmg(dmg, this.transform.position);
        } else if (playerIsInThrowingRange && isGrounded && CanAttack() && unreachable) {
            Vector2 dir = (playerScript.transform.position - firePointTrans.position).normalized;
            RaycastHit2D raycastHit2D = Physics2D.Raycast(this.transform.position, dir, 15f, playerAndPlatformLayerMask);
            if (raycastHit2D && raycastHit2D.collider.name == "Player" ){//&& IsWithinVectorBounds(dir)) {
                StartCoroutine(Throw(dir));
            }
        }
    }

    private bool CanAttack() {
        return (lastAttack + attackRate <= Time.time) && !isStunned;
    }

    IEnumerator Throw(Vector2 dir) {
        isThrowing = true;
        if (dir.x >= 0) {
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        } else {
            transform.localScale = new Vector3(-1f * Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
        yield return new WaitForSeconds(spawnDelay);
        GameObject shuriken = Instantiate(shurikenPrefab, firePointTrans.position, Quaternion.identity);
        ShurikenEnemy shurikenScript = shuriken.GetComponent<ShurikenEnemy>();
        shurikenScript.SetShurikenVelocity(dir);
    }

    private bool IsWithinVectorBounds(Vector2 dir) {
        float dot;
        if (dir.x >= 0) {
            dot = Vector2.Dot(dir, Vector2.right);
        } else {
            dot = Vector2.Dot(dir, Vector2.left);
        }
        return dot >= 0.5f; // 0.5 is the dot product value for 60 degrees
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

        if (isThrowing) {
            enemyAnim.SetBool("isThrowing", true);
            lastAttack = Time.time;
            Invoke("SetIsThrowingFalse", 0.5f);
        }

        if (isMeleeing) {
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

    private void SetIsThrowingFalse() {
        enemyAnim.SetBool("isThrowing", false);
        isThrowing = false;
    }

    private void SetIsMeleeingFalse() {
        enemyAnim.SetBool("isMeleeing", false);
        //isAttacking = false;
        isMeleeing = false;
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
            //alertedSprite.color = new Color(1f, 0f, 0f, 0.18f);
            alertedObj.SetActive(true);
            isReturningToPatrolPos = false;
            astarScript.SetReturnToPatrolPos(false);
        } else {
            alertedObj.SetActive(false);
        }
    }

    // Reduces the enemy's health by dmg.
    public void TakeDmg(int dmg) {
        enemyHealth -= dmg;
        StartCoroutine(KnockBack(new Vector2(playerScript.GetPlayerAttackDir(), 0f)));
        if (enemyHealth <= 0) {
            StartCoroutine("Death");
        } else {
            SetAlertStatus(true);
        }
    }
    #endregion
}
