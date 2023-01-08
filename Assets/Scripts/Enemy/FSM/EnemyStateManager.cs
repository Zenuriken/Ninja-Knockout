using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStateManager : MonoBehaviour
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
    [Tooltip("The velocity multiplier for jumping.")]
    private float jumpMultiplier;
    [SerializeField]
    [Tooltip("The horizontal airborne velocity of the enemy when pursuing.")]
    private float pursueAirborneVelX;
    [SerializeField]
    [Tooltip("The horizontal airborne velocity of the enemy when patrolling.")]
    private float patrolAirborneVelX;
    [SerializeField]
    [Tooltip("The delay before the enemy is alerted to the player's presence.")]
    private float alertedDelay;
    [SerializeField]
    [Tooltip("Dust particle effect when on the ground.")]
    private ParticleSystem groundDust;
    [SerializeField]
    [Tooltip("Question mark effect when pursuing player.")]
    private ParticleSystem questionMark;
    [SerializeField]
    [Tooltip("Exclamation mark effect when alerted by the player.")]
    private ParticleSystem exclamationMark;
    [SerializeField]
    [Tooltip("The vertical cell offset for jumping 1-2 cells high.")]
    private float jumpOffset1;
    [SerializeField]
    [Tooltip("The vertical cell offset for jumping 3 cells high.")]
    private float jumpOffset2;
    [SerializeField]
    [Tooltip("The vertical cell offset for jumping 4-5 cells high.")]
    private float jumpOffset3;
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
    [Tooltip("The delay before the body splat sound effect is played after death")]
    private float bodySplatDelay;
    [SerializeField]
    [Tooltip("The delay before the enemy's body starts to fade away.")]
    private float fadeAwayDelay;
    [SerializeField]
    [Tooltip("The speed in which the enemy's body fades away.")]
    private float fadeAwaySpeed;
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
    [SerializeField]
    [Tooltip("The shuriken drop prefab.")]
    private GameObject shurikenDropPrefab;
    [Space(5)]
    #endregion

    #region Toggle Variables
    [Header("Toggle Variables")]
    [SerializeField]
    [Tooltip("Controls if the enemy is alerted.")]
    private bool isAlerted;
    [SerializeField]
    [Tooltip("Controls whether the enemy will patrol.")]
    private bool patrolEnabled;
    [SerializeField]
    [Tooltip("Controls the starting direction of the enemy (-1 for left, 0 for random, 1 for right).")]
    private int startingDir;
    [SerializeField]
    [Tooltip("Controls the chances of the enemy dropping a shuriken (0 - 100).")]
    private float dropChance;
    [Space(5)]
    #endregion
    
    #region Private Variables
    // Player cached references
    private PlayerController playerScript;
    private Health playerHealthScript;
    private Melee meleeScript;
    private GameObject fieldOfViewParent;

    // Cached components
    private GameObject alertedObj;
    private Transform firePointTrans;
    private SpriteRenderer enemySprite;
    private Animator enemyAnim;
    private CapsuleCollider2D enemyCollider;
    private Rigidbody2D enemyRB;
    private AStar astarScript;
    private FieldOfView fov;
    private MeleeEnemy meleeEnemyScript;
    private AlertedSight alertedSightScript;
    private SoundManager sounds;
    private GameObject highLight;

    // Private variables
    private LayerMask allPlatformsLayerMask;
    private LayerMask playerAndPlatformLayerMask;
    private Vector2 adjustedPos;
    private float shurikenRadius = 0.335f;
    private int damageCounter;
    private float lastIdle;
    private float lastAttack;
    private string gruntSound;
    private string deathSound;

    // Pathfinding Variables
    private List<Vector2> targetPath;
    private List<Vector2> newPath;
    private List<Vector2> patrolPath;
    private int currPathIndex;
    private float leftPatrolEnd;
    private float rightPatrolEnd;

    private Vector2 jumpTarget;
    private Vector2 jumpDir;

    // Condition Variables
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
    private bool playedBodySplat;
    private bool bodySplatDelayPast;
    private bool isDetectingPlayer;
    private bool isInPlayerMeleeRange;
    private bool beganCalculatingPath;
    private bool isJumping;
    #endregion

    #region State Variables
    EnemyStateFactory states;
    EnemyState currentState;
    #endregion

    #region Initializaiton Functions
    void Awake() {
        // Assign component values.
        alertedObj = this.transform.GetChild(0).gameObject;
        alertedSightScript = this.transform.GetChild(0).GetComponent<AlertedSight>();
        sounds = this.transform.GetChild(6).GetComponent<SoundManager>();
        enemySprite = this.GetComponent<SpriteRenderer>();
        enemyCollider = this.GetComponent<CapsuleCollider2D>();
        enemyRB = this.GetComponent<Rigidbody2D>();
        enemyAnim = this.GetComponent<Animator>();
        astarScript = this.GetComponent<AStar>();
        meleeEnemyScript = this.transform.GetChild(1).GetComponent<MeleeEnemy>();
        firePointTrans = this.transform.GetChild(2).transform;
        highLight = this.transform.GetChild(7).gameObject;
    }

    // Start is called before the first frame update
    void Start()
    {
        // Set outside references.
        playerScript = PlayerController.singleton;
        playerHealthScript = playerScript.gameObject.GetComponent<Health>();
        meleeScript = playerScript.transform.GetChild(1).GetComponent<Melee>();
        allPlatformsLayerMask = LayerMask.GetMask("Platform", "OneWayPlatform");
        playerAndPlatformLayerMask = LayerMask.GetMask("Player", "Platform", "OneWayPlatform");
        fieldOfViewParent = GameObject.Find("FieldOfViews");

        // Determines whether the enemy will be male or female.
        float value = Random.Range(0, 100);
        gruntSound = (value < 50) ? "MaleGrunt" : "FemaleGrunt";
        deathSound = (value < 50) ? "MaleDeath" : "FemaleDeath";

        // Initialize FOV.
        GameObject lineOfSight = GameObject.Instantiate(lineOfSightObj, fieldOfViewParent.transform);
        fov = lineOfSight.GetComponent<FieldOfView>();
        fov.InitializeEnemyScript(this);

        // Starting state for the state machine
        states = new EnemyStateFactory(this);
        currentState = states.Patrol();
        currentState.EnterState(); 
    }
    #endregion
    
    // Update is called once per frame
    void Update() {
        currentState.UpdateState();
        SetGrounded();
        SetDirection();
        SetAdjustedPos();
        UpdateSprite();
    }

    #region State Functions
    // Sets whether the enemy is grounded.
    private void SetGrounded() {
        bool lastGroundStatus = isGrounded;
        RaycastHit2D raycastHit2D = Physics2D.BoxCast(enemyCollider.bounds.center, new Vector2(0.6f, enemyCollider.bounds.size.y - 0.1f), 0f, Vector2.down, 0.2f, allPlatformsLayerMask);
        isGrounded = raycastHit2D.collider != null;
        if (!lastGroundStatus && isGrounded) isJumping = false;
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

    // Sets the adjusted position of the enemy (the tile above the platform it stands on).
    private void SetAdjustedPos() {
        adjustedPos = astarScript.GetAdjustedPosition();
    }

    // Creates question marks if the enemy doesn't have an exclamation mark.
    public void CreateQuestionMark() {
        if (!exclamationMark.isPlaying) {
            questionMark.Play();
        } else {
            questionMark.Clear();
        }
    }

    // Moves the enemy according to the Pursue Path.
    public void FollowPath(float speed) {
        // If there is no path or our current path index is greater than the length of our path, terminate.
        if (targetPath == null || currPathIndex >= targetPath.Count || isJumping) return;

        Vector2 nextPos = targetPath[currPathIndex];
        Vector2 dir = (nextPos - adjustedPos);

        // Move Right or Left.
        if (dir.y == 0) {
            enemyRB.velocity = new Vector2(Mathf.Sign(dir.x) * speed, enemyRB.velocity.y);
        } else {
            isJumping = true;
            enemyRB.velocity = Vector2.zero;
            Vector2 jumpForce = CalculateJumpForce(dir);
            enemyRB.AddForce(jumpForce, ForceMode2D.Impulse);
        }

        // Increment path counter if enemy has reached the current path node.
        if (adjustedPos == targetPath[currPathIndex]) {
            currPathIndex++;
        }
    }

    // Updates the enemy's pursue path.
    public void UpdatePursuePath() {
        newPath = astarScript.CalculatePath();
        if (newPath != null) {
            targetPath = newPath;
            currPathIndex = 0;
            unreachable = false;
        } else {
            unreachable = true;
            if (!hasDied && playerScript.IsHiding() && Mathf.Abs(enemyRB.velocity.x) < 0.05f && isAlerted) {
                CreateQuestionMark();
                if (!isReturningToPatrolPos) {
                    StartCoroutine("ReturnToPatrols");
                }
            }

        }
    }

    public Vector2 CalculateJumpForce(Vector2 nextPos) {
        
        return Vector2.up * 10f;
    }

    // Updates the player's sprites based on input/state.
    private void UpdateSprite() {
        enemyAnim.SetBool("isMoving", Mathf.Abs(enemyRB.velocity.x) > 0.05f);
        enemyAnim.SetBool("isJumping", enemyRB.velocity.y > 0.05f);
        enemyAnim.SetBool("isFalling", enemyRB.velocity.y < -0.05f);
        enemyAnim.SetBool("isGrounded", isGrounded);
        enemyAnim.SetBool("isStunned", isStunned);
        enemyAnim.SetBool("hasDied", hasDied);
        enemyAnim.SetBool("isThrowing", isThrowing);
        enemyAnim.SetBool("isMeleeing", isMeleeing);
        enemyAnim.SetBool("isAlerted", isAlerted);
    }
    #endregion


    #region Getters/Setters
    public FieldOfView FOV {get{return fov;}}
    public SoundManager Sounds {get{return sounds;}}
    public AStar AstarScript {get{return astarScript;}}
    public EnemyState CurrentState {get{return currentState;} set{currentState = value;}}
    public ParticleSystem QuestionMarks {get{return questionMark;}}
    public ParticleSystem ExclamationMark {get{return exclamationMark;}}
    public GameObject AlertedObj {get{return alertedObj;}}
    public Rigidbody2D EnemyRB {get{return enemyRB;}}
    public int MaxNodeDist {get{return maxNodeDist;}}
    public int StartingDir {get{return startingDir;} set{startingDir = value;}}
    public float PursueSpeed {get{return pursueSpeed;}}
    public float PatrolSpeed {get{return patrolSpeed;}}
    public float IdleDur {get{return idleDur;}}
    public float AlertedDelay {get{return alertedDelay;}}
    public bool IsDetectingPlayer {get{return isDetectingPlayer;} set{isDetectingPlayer = value;}}
    public bool IsAlerted {get{return isAlerted;} set{isAlerted = value;}}
    public bool HasDied {get{return hasDied;} set{hasDied = value;}}
    #endregion


}
