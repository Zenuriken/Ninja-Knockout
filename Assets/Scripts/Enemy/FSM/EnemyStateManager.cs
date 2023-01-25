using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStateManager : MonoBehaviour
{
    #region Movement Variables
    [Header("Movement")]
    [SerializeField][Tooltip("The number of tiles the enemy can stray from starting pos while in patrol mode.")]
    private int maxNodeDist;
    [SerializeField][Tooltip("How long the enemy is in an idle state before patrolling.")]
    private float idleDur;
    [SerializeField][Tooltip("The speed of the enemy when patrolling.")]
    private float patrolSpeed;
    [SerializeField][Tooltip("The speed of the enemy when pursuing.")]
    private float pursueSpeed;
    [SerializeField][Tooltip("The delay before the enemy is alerted to the player's presence.")]
    private float alertedDelay;
    [SerializeField][Tooltip("Dust particle effect when on the ground.")]
    private ParticleSystem groundDust;
    [SerializeField][Tooltip("Question mark effect when pursuing player.")]
    private ParticleSystem questionMark;
    [SerializeField][Tooltip("Exclamation mark effect when alerted by the player.")]
    private ParticleSystem exclamationMark;

    [SerializeField][Tooltip("THe line renderer to display the jump path.")]
    private LineRenderer _Line;
    [SerializeField][Tooltip("The precision/resolution of the line renderer.")]
    private float _Step;
    [SerializeField][Tooltip("The multiplier for the time it takes to jump a path.")]
    private float _JumpSpeedFactor;
    [SerializeField][Tooltip("The delay between jumps.")]
    private float jumpDelay;
    [Space(5)]
    #endregion
    
    #region Health Variables
    [Header("Health")]
    [SerializeField][Tooltip("How much health this enemy has.")]
    private int enemyHealth;
    [SerializeField][Tooltip("The delay before the enemy is destroyed")]
    private float destroyDelay;
    [SerializeField][Tooltip("The delay before the body splat sound effect is played after death")]
    private float bodySplatDelay;
    [SerializeField][Tooltip("The delay before the enemy's body starts to fade away.")]
    private float fadeAwayDelay;
    [SerializeField][Tooltip("The speed in which the enemy's body fades away.")]
    private float fadeAwaySpeed;
    [SerializeField][Tooltip("The amount this enemy's death will increse your score.")]
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
    private Vector3 spawnPos;
    private string gruntSound;
    private string deathSound;
    private float lastJump;

    // Pathfinding Variables
    private List<Vector2> targetPath;
    private List<Vector2> newPath;
    private List<Vector2> patrolPath;
    private int currPathIndex;
    private float leftPatrolEnd;
    private float rightPatrolEnd;
    private float heightOffset = -1.55f;
    private Vector3 initialPos;

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

        RaycastHit2D raycastHit2D = Physics2D.Raycast(this.transform.position, Vector2.down, 100f, allPlatformsLayerMask);
        spawnPos = raycastHit2D.point;
        spawnPos.y += 0.5f;
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
        // Time.timeScale = 0.25f;
    }
    #endregion
    
    // Update is called once per frame
    void Update() {
        SetGrounded();
        SetDirection();
        SetAdjustedPos();
        currentState.UpdateState();
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
        // If there is no path or our current path index is greater than the length of our path, check if enemy is stuck. Otherwise, terminate.
        if (targetPath == null || currPathIndex >= targetPath.Count) {
            // Handles the case in which the enemy gets stuck on an edge.
            int moveDir = astarScript.Unstuck();
            if (moveDir != 0 && Mathf.Abs(enemyRB.velocity.x) < 0.05f && Mathf.Abs(enemyRB.velocity.y) < 0.05f) {
                enemyRB.velocity = new Vector2(moveDir * pursueSpeed, enemyRB.velocity.y);
            }
            unreachable = true;
            return;
        }
        unreachable = false;
        Vector2 nextPos = targetPath[currPathIndex];
        Vector2 dir = (nextPos - adjustedPos);

        // Move Right or Left.
        if (dir.y == 0 && !isJumping && isGrounded) {
            enemyRB.velocity = new Vector2(Mathf.Sign(dir.x) * speed, enemyRB.velocity.y);
        } else if (!isJumping && isGrounded && CanJump()) {
            Jump(nextPos);
        }

        // Increment path counter if enemy has reached the current path node.
        if (adjustedPos == targetPath[currPathIndex]) {
            currPathIndex++;
        }
    }  


    // Updates the enemy's pursue path.
    public IEnumerator UpdatePath(Vector3 targetPos) {
        while (true) {
            Debug.Log("UPDATING PATH");
            newPath = astarScript.CalculatePath(targetPos);
            if (newPath != null) {
                targetPath = newPath;
                currPathIndex = 0;
                // unreachable = false;
            }
            // unreachable = true;
            yield return new WaitForSeconds(0.5f);
        }
    }

    // Handles the logic for jumping to a location.
    private void Jump(Vector2 nextPos) {
        isJumping = true;
        lastJump = Time.time;
        enemyRB.velocity = Vector2.zero;
        Vector3 targetPos = (Vector3)nextPos - this.transform.position;
        targetPos.y -= heightOffset;
        float height = targetPos.y + targetPos.magnitude / 10f;
        height = Mathf.Max(1f, height);
        float angle;
        float v0;
        float time;
        CalculatePathWithHeight(targetPos, height, out v0, out angle, out time);
        Vector2 dir = GetVectorFromAngle(angle * Mathf.Rad2Deg);
        enemyRB.AddForce(dir * v0, ForceMode2D.Impulse);
    }

    // Draws the path of a jump with the specified number of segments (steps).
    private void DrawPath(float v0, float angle, float time, float step) {
        step = Mathf.Max(0.01f, step);
        _Line.positionCount = (int)(time / step) + 2;
        int count = 0;
        for (float i = 0; i < time; i += step) {
            float x = v0 * i * Mathf.Cos(angle);
            float y = v0 * i * Mathf.Sin(angle) - (1f / 2f) * -Physics.gravity.y * Mathf.Pow(i, 2);
            _Line.SetPosition(count , this.transform.position + new Vector3(x, y, 0));
            count++;
        }
        float xfinal = v0 * time * Mathf.Cos(angle);
        float yfinal = v0 * time * Mathf.Sin(angle) - (1f / 2f) * -Physics.gravity.y * Mathf.Pow(time, 2);
        _Line.SetPosition(count, this.transform.position + new Vector3(xfinal, yfinal, 0));
    }

    // Returns the quadratic formula.
    private float QuadraticEquation(float a, float b, float c, float sign) {
        return (-b + sign * Mathf.Sqrt(b * b -4 * a * c)) / (2 * a);
    }

    // Calculates and assigns the initial velocity and anlge of a jump path.
    private void CalculatePathWithHeight(Vector3 targetPos, float h, out float v0, out float angle, out float time) {
        float xt = targetPos.x;
        float yt = targetPos.y;
        float g = -Physics.gravity.y * enemyRB.gravityScale;
        float b = Mathf.Sqrt(2 * g * h);
        float a = (-0.5f * g);
        float c = -yt;
        float tplus = QuadraticEquation(a, b, c, 1);
        float tmin = QuadraticEquation(a, b, c, -1);
        time = tplus > tmin ? tplus : tmin;
        angle = Mathf.Atan(b * time / xt);
        v0 = b / Mathf.Sin(angle);

        // float g_draw = -Physics.gravity.y;
        // float a_draw = (-0.5f * g_draw);
        // float b_draw = Mathf.Sqrt(2 * g_draw * h);
        // float c_draw = -yt;
        // float tplus_draw = QuadraticEquation(a_draw, b_draw, c_draw, 1);
        // float tmin_draw = QuadraticEquation(a_draw, b_draw, c_draw, -1);
        // float time_draw = tplus_draw > tmin_draw ? tplus_draw : tmin_draw;
        // float angle_draw = Mathf.Atan(b_draw * time_draw / xt);
        // float v0_draw = b_draw / Mathf.Sin(angle_draw);
        // DrawPath(v0_draw, angle_draw, time_draw, _Step);
    }

    // Returns a vector pointing in the direction of the given angle (in degrees).
    private Vector2 GetVectorFromAngle(float angle) {
        // angle = 0 -> 360
        float angleRad = angle * (Mathf.PI / 180f);
        return new Vector2(Mathf.Cos(angleRad), Mathf.Sin(angleRad));
    }

    // Returns if the player is able to jump.
    private bool CanJump() {
        return (lastJump + jumpDelay <= Time.time) && !isStunned;
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
    public Vector3 SpawnPos {get{return spawnPos;}}
    public int MaxNodeDist {get{return maxNodeDist;}}
    public int StartingDir {get{return startingDir;} set{startingDir = value;}}
    public float PursueSpeed {get{return pursueSpeed;}}
    public float PatrolSpeed {get{return patrolSpeed;}}
    public float IdleDur {get{return idleDur;}}
    public float AlertedDelay {get{return alertedDelay;}}
    public bool IsDetectingPlayer {get{return isDetectingPlayer;} set{isDetectingPlayer = value;}}
    public bool IsAlerted {get{return isAlerted;} set{isAlerted = value;}}
    public bool HasDied {get{return hasDied;} set{hasDied = value;}}
    public bool Unreachable {get{return unreachable;}}
    #endregion


}
