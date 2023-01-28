using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {
    public static PlayerController singleton;

    #region Movement Variables
    [Header("Movement")]
    [SerializeField]
    [Tooltip("The moveSpeed of the player.")]
    private float moveSpeed;
    [SerializeField]
    [Tooltip("The max speed the player can fall.")]
    private float maxFallSpeed;
    [SerializeField]
    [Tooltip("Speed of the player when sneaking.")]
    private float sneakSpeed;
    [Space(5)]
    #endregion

    #region Jump Variables
    [Header("Jumping")]
    [SerializeField]
    [Tooltip("The jump velocity of the player.")]
    private float jumpVel;
    [SerializeField]
    [Tooltip("The duration of a jump when holding down the jump key.")]
    private float jumpDur;
    [SerializeField]
    [Tooltip("The delay between jumps.")]
    private float jumpDelay;
    [SerializeField]
    [Tooltip("The distance the player must falling to create landing sound effect.")]
    private float fallSoundDist;
    [Space(5)]
    #endregion

    #region Dash Variables
    [Header("Dash")]
    [SerializeField]
    [Tooltip("The duration of a dash (Uses a coroutine).")]
    private float dashDur;
    [SerializeField]
    [Tooltip("The delay between dashes.")]
    private float dashDelay;
    [SerializeField]
    [Tooltip("The force applied to the player when dashing (Uses a coroutine).")]
    private float dashForce;
    [SerializeField]
    [Tooltip("How long the trail lingers after a dash.")]
    private float trailDur;
    [Space(5)]
    #endregion

    #region Wall Climbing Variables
    [Header("Wall Climbing")]
    [SerializeField]
    [Tooltip("The duration of a single wall jump in the Vector(xWallJumpVel, yWallJumpVel) direction.")]
    private float wallJumpDur;
    [SerializeField]
    [Tooltip("The speed player glides down a wall.")]
    private float wallFallSpeed;
    [SerializeField]
    [Tooltip("The amount of time to wallClimb before sound starts playing.")]
    private float wallClimbSoundTime;
    [Space(5)]
    #endregion

    #region Attack Variables
    [Header("Attacking")]
    [SerializeField]
    [Tooltip("The shuriken prefab.")]
    private GameObject shurikenPrefab;
    [SerializeField]
    [Tooltip("The attack rate.")]
    private float attackRate;
    [SerializeField]
    [Tooltip("The delay of spawning the shuriken.")]
    private float spawnDelay;
    [SerializeField]
    [Tooltip("The amount of time the player needs to hold the shoot button to aim a shuriken.")]
    private float holdTime;
    [SerializeField]
    [Tooltip("The speed in which the skillshot arrow moves.")]
    private float skillShotSpeed;
    [SerializeField]
    [Tooltip("The max number of shurikens player can hold.")]
    private int maxShurikens;
    [SerializeField]
    [Tooltip("The starting number of shurikens the player holds.")]
    private int startingShurikens;
    [SerializeField]
    [Tooltip("The radius of the shuriken for collision detection.")]
    private float shurikenRadius;

    // [SerializeField]
    // [Tooltip("The knock back force when clashing with a platform or enemy.")]
    // private float knockBackForce;
    // [SerializeField]
    // [Tooltip("The knock back duration when clashing with a platform or enemy.")]
    // private float knockBackDur;
    [Space(5)]
    #endregion

    #region Particle Variables
    [Header("Particles")]
    [SerializeField]
    [Tooltip("Dust particle effect when on the ground.")]
    private ParticleSystem groundDust;
    [SerializeField]
    [Tooltip("Dust particle effect when wall climbing on the left.")]
    private ParticleSystem wallClimbDust;
    [SerializeField]
    [Tooltip("Spark particle effect when clashing with an enemy.")]
    private ParticleSystem sparks;
    [Space(5)]
    #endregion

    #region Melee Trail Variables
    [Header("Trail")]
    [SerializeField]
    private GameObject meleeBallPrefab;
    [SerializeField]
    private float meleeBallSpeed;
    [SerializeField]
    private float meleeNumIters;
    [SerializeField]
    private float meleeTime;
    [SerializeField]
    private float meleeTrailTime;
    [Space(5)]
    #endregion

    #region Inspector Variables
    [Header("Toggles")]
    [SerializeField]
    private bool playerInputEnabled;
    [SerializeField]
    private bool titleScreenModeEnabled;
    [SerializeField]
    private bool dashEnabled;
    #endregion

    #region Private Variables
    // Private Input Variables
    private KeyCode jumpKey = KeyCode.Z;
    private KeyCode dashKey = KeyCode.C;
    private KeyCode fireKey = KeyCode.Space;
    private KeyCode meleeKey = KeyCode.X;
    private KeyCode sneakKey = KeyCode.LeftShift;
    private KeyCode upKey = KeyCode.UpArrow;
    private KeyCode downKey = KeyCode.DownArrow;
    private KeyCode escapeKey = KeyCode.Escape;
    private KeyCode enterKey = KeyCode.Return;

    // Player Input Variables
    private bool rightPressed;
    private bool leftPressed;
    private bool jumpPressed;
    private bool jumpHolding;
    private bool jumpReleased;
    private bool dashPressed;
    private bool meleePressed;
    private bool fireReleased;
    private bool fireHolding;
    private bool sneakHolding;
    private bool upHolding;
    private bool downHolding;
    private bool closePressed;
    private bool continuePressed;

    // State Variables
    private bool isJumping;
    private bool isDashing;
    private bool isWallJumping;
    private bool isWallClimbing;
    private bool isSneaking;
    private bool isAttacking;
    private bool isStunned;
    private bool isGrounded;
    private bool isAgainstWall;
    private bool isHiding;
    private bool isDamaged;
    private bool hasDied;
    private bool isBuffering;
    private bool isPlayingWallClimbingNoise;
    private bool isPlayingEnteringBushesNoise;
    private bool isPlayingLeavingBushesNoise;
    private bool isPlayingWallJumpingNoise;
    private bool isPlayingStealthMeleeKill;
    private bool isThrowing;
    private bool isMeleeing;
    private int numCovered;

    // Private Dash Variables
    private int dashCounter;
    private float lastDash;

    // Private shooting variables
    private HighLight lastTarget;
    private Transform firePointTrans;
    private Transform skillShotTrans;
    private SpriteRenderer skillShotSprite;
    private Transform wallFirePointTrans;
    private Transform wallSkillShotTrans;
    private SpriteRenderer wallSkillShotSprite;
    private float lastAttack;
    private float currHoldTime;
    private float angleRaw;
    private float angleAdjusted;
    private int lastAttackDir;
    private int numShurikens;

    // Private Jump Variables
    private Vector2 lastJumpPos;
    private float jumpDurTimer;
    private float jumpCounter;
    private float lastJump;
    private float lastYVel;

    // Melee private variables
    private Melee meleeScript;
    private bool meleeActive;
    private float meleeSpeed;
    private static int meleeCounter;

    // Private WallClimbing Variables
    private float currWallClimbTime;

    // General private variables
    private Rigidbody2D playerRB;
    private Collider2D boxCollider2D;
    private LayerMask platformLayerMask;
    private LayerMask allPlatformsLayerMask;
    private LayerMask enemyPlatformLeverLayerMask;
    private TrailRenderer dashTrail;
    private TrailRenderer doubleJumpTrail;
    private SoundManager sounds;
    private Health healthScript;
    private GameObject highLight;

    private float xInput;
    private float gravity;
    private float speed;
    private int lastDir;
    private int side;
    private int alertedNum;
    private int gold;

    // Private Animator Private Variables
    private Animator playerAnim;
    private SpriteRenderer playerSprite;

    // Private Respawn Variables
    private Vector2 spawnLocation;
    private Vector2 lastCampFirePos;
    #endregion

    /**********************************************************************************/

    #region Initializing Functions
    // Awake is called before Start
    private void Awake() {
        if (singleton != null && singleton != this) {
            Destroy(this.gameObject);
        } else {
            singleton = this;
            DontDestroyOnLoad(this.gameObject);
            // Getting Player Components
            playerRB = GetComponent<Rigidbody2D>();
            boxCollider2D = GetComponent<Collider2D>();
            playerAnim = GetComponent<Animator>();
            playerSprite = GetComponent<SpriteRenderer>();
            healthScript = GetComponent<Health>();
            sounds = this.transform.GetChild(6).GetComponent<SoundManager>();
            dashTrail = this.transform.GetChild(2).GetChild(1).GetComponent<TrailRenderer>();
            doubleJumpTrail = this.transform.GetChild(2).GetChild(2).GetComponent<TrailRenderer>();
            meleeScript = this.transform.GetChild(1).GetComponent<Melee>();
            firePointTrans = this.transform.GetChild(0).transform;
            skillShotTrans = firePointTrans.GetChild(0).transform;
            skillShotSprite = skillShotTrans.GetComponent<SpriteRenderer>();
            wallFirePointTrans = this.transform.GetChild(5).transform;
            wallSkillShotTrans = wallFirePointTrans.GetChild(0).transform;
            wallSkillShotSprite = wallSkillShotTrans.GetComponent<SpriteRenderer>();
            highLight = this.transform.GetChild(7).gameObject;
        }
    }

    // Start is called before the first frame update
    void Start() {
        speed = moveSpeed;
        lastDir = 1;
        gravity = playerRB.gravityScale;
        isStunned = false;
        meleeActive = false;
        meleeCounter = 0;
        dashTrail.emitting = false;
        doubleJumpTrail.emitting = false;
        numShurikens = startingShurikens;
        platformLayerMask = LayerMask.GetMask("Platform");
        allPlatformsLayerMask = LayerMask.GetMask("Platform", "OneWayPlatform");
        enemyPlatformLeverLayerMask = LayerMask.GetMask("Enemy", "Platform", "OneWayPlatform", "Lever");
        if (!titleScreenModeEnabled) {
            UIManager.singleton.UpdateShurikenNum(numShurikens);
            UIManager.singleton.InitializeShurikenBackground(maxShurikens);
        }
    }
    #endregion

    #region Update Functions
    // Update is called once per frame
    void Update() {
        // Limits the velocity when falling
        playerRB.velocity = new Vector2(playerRB.velocity.x, Mathf.Clamp(playerRB.velocity.y, -maxFallSpeed, maxFallSpeed));
        // If the player is on the title screen.
        if (titleScreenModeEnabled) {
            playerRB.velocity = new Vector2(speed, playerRB.velocity.y);
            if (isGrounded) {
                CreateDust(0);
            }
            return;
        }
        // If the player is not on title screen.
        if (!hasDied) {
            GetPlayerInput();
            IsGrounded();
            IsAgainstWall();
            SetDirection();
            Move();
            Melee();
            Throw();
            HidePlayer();
        }
        UpdateSprite();
    }
    #endregion

    #region Movement Functions
    // Controls the player's movement.
    void Move() {
        if (isStunned) return;
        
        // Regular movement
        if (!isDashing && !isWallJumping && !isAttacking) {
            playerRB.velocity = new Vector2(xInput * speed, playerRB.velocity.y);
        } else if (isAttacking) {
            playerRB.velocity = new Vector2(0f, playerRB.velocity.y);
        }

        // Tapping the jump button
        if (jumpPressed && (isGrounded || jumpCounter > 0) && !isDashing && !isWallJumping && !isAgainstWall && !isAttacking && CanJump()) {
            isJumping = true;
            jumpDurTimer = jumpDur;
            playerRB.velocity = new Vector2(playerRB.velocity.x, jumpVel);
            jumpCounter -= 1;
            lastJump = Time.time;
            if (!isGrounded && jumpCounter == 0) {
                sounds.Play("DoubleJumping");
                doubleJumpTrail.emitting = true;
                doubleJumpTrail.time = 0.25f;
                StartCoroutine(ReduceTrail(doubleJumpTrail, false));
            } else {
                sounds.Play("Jumping");
            }
        }

        // Holding the jump button
        if (jumpHolding && isJumping && !isDashing && !isWallJumping && !isAgainstWall) {
            if (jumpDurTimer > 0) {
                playerRB.velocity = new Vector2(playerRB.velocity.x, jumpVel);
                jumpDurTimer -= Time.deltaTime;
            } else {
                isJumping = false;
            }
        }

        // Releasing the jump button
        if (jumpReleased) isJumping = false;

        // Tapping the dash button.
        if (dashEnabled && dashPressed && !isDashing && !isJumping && !isWallJumping && !isAttacking && (dashCounter > 0 || isGrounded) && CanDash()) {
            lastDash = Time.time;
            StartCoroutine("Dash");
        }

        // Sneaking
        if (sneakHolding && isGrounded) {
            isSneaking = true;
            speed = sneakSpeed;
        } else if (!sneakHolding) {
            speed = moveSpeed;
            isSneaking = false;
        }

        // Wall Climbing
        if (!isDashing && isAgainstWall) {
            playerRB.velocity = new Vector2(0f, -1f * wallFallSpeed);
            if (!isGrounded) {
                isWallClimbing = true;
                currWallClimbTime += Time.deltaTime;
                if (!isPlayingWallClimbingNoise && WallClimbTimeMet()) {
                    sounds.Play("WallClimbing");
                    isPlayingWallClimbingNoise = true;
                }
                CreateDust(1);
            } else {
                isWallClimbing = false;
                sounds.Stop("WallClimbing");
                currWallClimbTime = 0f;
                isPlayingWallClimbingNoise = false;
            }
            // Setting Wall Jumping to true
            if (jumpPressed && CanJump()) {
                isWallJumping = true;
                sounds.Stop("WallClimbing");
                currWallClimbTime = 0f;
                isPlayingWallClimbingNoise = false;
                Invoke("SetWallJumpingFalse", wallJumpDur);
            }
        } else {
            isWallClimbing = false;
            sounds.Stop("WallClimbing");
            currWallClimbTime = 0f;
            isPlayingWallClimbingNoise = false;
        }

        // Wall Jumping
        if (isWallJumping) {
            playerRB.velocity = new Vector2(-lastDir * moveSpeed, jumpVel);
            lastJump = Time.time;
            if (!isPlayingWallJumpingNoise) {
                sounds.Play("WallJumping");
                isPlayingWallJumpingNoise = true;
            }
        } else {
            isPlayingWallJumpingNoise = false;
        }

        // Recording jump position and Y velocity for playing noise / creating dust.
        if (lastYVel > 0.05f && this.playerRB.velocity.y < -0.05f) lastJumpPos = this.transform.position;
        lastYVel = this.playerRB.velocity.y;
    }

    // Sets the variable: lastDir based on the xInput of the player.
    private void SetDirection() {
        if (!isWallJumping && !isAttacking && !isDashing && !isStunned) {
            if (xInput > 0) {
                lastDir = 1;
            } else if (xInput < 0) {
                lastDir = -1;
            }
            transform.localScale = new Vector3(lastDir * Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
    }

    // Sets Wall Climbing to false.
    private void SetWallJumpingFalse() {
        isWallJumping = false;
    }

    // Controls the dashing process.
    IEnumerator Dash() {
        if (!isStunned) {
            isDashing = true;
            dashCounter -= 1;
            playerRB.velocity = new Vector2(playerRB.velocity.x, 0f);
            if (isGrounded) CreateDust(0);
            playerRB.gravityScale = 0;
            dashTrail.emitting = true;
            dashTrail.time = 0.25f;
            playerRB.AddForce(new Vector2(dashForce * lastDir, 0f), ForceMode2D.Impulse);
            sounds.Play("Dashing");
            yield return new WaitForSeconds(dashDur);
            playerRB.gravityScale = gravity;
            isDashing = false;
            StartCoroutine(ReduceTrail(dashTrail, false));
        }
    }

    IEnumerator ReduceTrail(TrailRenderer trail, bool destroy) {
        trail.emitting = true;
        for (float t = 0.25f; t >= 0; t -= 0.05f) {
            trail.time = t;
            yield return new WaitForSeconds(trailDur);
        }
        trail.emitting = false;
        if (destroy) Destroy(trail.gameObject);
    }

    // Creates dust around the player.
    void CreateDust(int s) {
        // Create ground dust
        if (s == 0) {
            groundDust.Play();
        // Create wall dust on appropriate side.
        } else if (s == 1) {
            wallClimbDust.Play();
        }
    }

    // Creates sparks around the player's sword.
    void CreateSparks() {
        sparks.Play();
    }

    // Returns if the player is able to dash.
    public bool CanDash() {
        return (lastDash + dashDelay <= Time.time) && !isStunned;
    }

    // Returns if the player is able to jump.
    public bool CanJump() {
        return (lastJump + jumpDelay <= Time.time) && !isStunned;
    }
    #endregion

    #region State Functions
    // Gets the Player Input
    private void GetPlayerInput() {
        closePressed = Input.GetKeyDown(escapeKey) || Input.GetKeyDown(enterKey);
        continuePressed = Input.anyKeyDown;
        if (closePressed) UIManager.singleton.ExitPopUp();
        if (playerInputEnabled) {
            xInput = Input.GetAxisRaw("Horizontal");
            jumpPressed = Input.GetKeyDown(jumpKey);
            jumpHolding = Input.GetKey(jumpKey);
            jumpReleased = Input.GetKeyUp(jumpKey);
            dashPressed = Input.GetKeyDown(dashKey);
            sneakHolding = Input.GetKey(sneakKey);
            meleePressed = Input.GetKeyDown(meleeKey);
            fireReleased = Input.GetKeyUp(fireKey);
            fireHolding = Input.GetKey(fireKey);
            upHolding = Input.GetKey(upKey);
            downHolding = Input.GetKey(downKey);
        } else {
            xInput = 0f;
            jumpPressed = false;
            jumpHolding = false;
            jumpReleased = false;
            dashPressed = false;
            sneakHolding = false;
            meleePressed = false;
            fireReleased = false;
            fireHolding = false;
            upHolding = false;
            downHolding = false;
        }
    }

    // Determines if the player is standing on ground.
    private void IsGrounded() {
        bool lastGroundStatus = isGrounded;
        RaycastHit2D raycastHit2D = Physics2D.BoxCast(boxCollider2D.bounds.center, new Vector2(0.6f, boxCollider2D.bounds.size.y - 0.1f), 0f, Vector2.down, 0.2f, allPlatformsLayerMask);
        isGrounded = raycastHit2D.collider != null;
        if (lastGroundStatus == false && isGrounded == true) {
            jumpCounter = 2;
            dashCounter = 1;
        }
        // If the player has landed on the grounded.
        if (lastGroundStatus == false && isGrounded == true && FallDistanceMet(this.transform.position) && raycastHit2D.collider.tag == "Platform" && !titleScreenModeEnabled) {
            CreateDust(0);
            sounds.Play("Landing");
        } else if (isGrounded && raycastHit2D.collider.tag == "Platform" && Mathf.Abs(playerRB.velocity.x) > 0.05f && !isSneaking) {
            CreateDust(0);
        }
        return;
    }

    // Returns if the player is jumping against a wall.
    private void IsAgainstWall() {
        int currSide = 0;
        RaycastHit2D raycastHit2D;
        // Check right
        if (lastDir == 1) {
            raycastHit2D = Physics2D.BoxCast(boxCollider2D.bounds.center, new Vector2(boxCollider2D.bounds.size.x, 0.6f), 0f, Vector2.right, 0.1f, platformLayerMask);
            currSide = 1;
            // Check left
        } else {
            raycastHit2D = Physics2D.BoxCast(boxCollider2D.bounds.center, new Vector2(boxCollider2D.bounds.size.x, 0.6f), 0f, Vector2.left, 0.1f, platformLayerMask);
            currSide = -1;
        }
        bool againstWall = raycastHit2D.collider != null;
        if (againstWall) {
            dashCounter = 1;
            jumpCounter = 1;
            side = currSide;
        }
        isAgainstWall = againstWall;
        return;
    }
    #endregion

    #region Attack Functions
    // Main attack function.
    private void Melee() {
        if (meleePressed && CanAttack() && isGrounded && !isDashing && !isWallClimbing && !isWallJumping) {
            skillShotSprite.enabled = false;
            if (isGrounded) CreateDust(0);
            meleeActive = true;
            meleeCounter += 1;
            lastAttackDir = lastDir;
            isAttacking = true;
            isMeleeing = true;
            lastAttack = Time.time;
            sounds.Play("Meleeing");
            Invoke("SetIsMeleeingFalse", 0.5f);
            Invoke("SetMeleeActiveFalse", 0.18f);
        }

        // Checking for collisions.
        if (!meleeActive) return;
        
        bool sparks = false;
        List<Collider2D> enemyColliders = meleeScript.GetEnemyColliders();
        foreach (Collider2D collider in enemyColliders) {
            EnemyStateManager enemy = collider.gameObject.GetComponent<EnemyStateManager>();
            if (!enemy.HasBeenDamaged(meleeCounter) && !enemy.HasDied) {
                if (enemy.IsAlerted || enemy.IsDetectingPlayer) {
                    enemy.TakeDmg(1);
                } else {
                    enemy.TakeDmg(5);
                    if (!isPlayingStealthMeleeKill) {
                        sounds.Play("StealthMeleeKill");
                        isPlayingStealthMeleeKill = true;
                    }
                }
                enemy.SetDamagedCounter(meleeCounter);
            }
        }
        // Checking projetile collisions.
        // List<Collider2D> projectileColliders = meleeScript.GetProjectileColliders();
        // foreach (Collider2D collider in projectileColliders) {
        //     Shuriken shuriken = collider.gameObject.GetComponent<Shuriken>();
        //     if (!shuriken.HasBeenDeflected(meleeCounter)) {
        //         shuriken.Deflected();
        //         shuriken.SetDeflectedCounter(meleeCounter);
        //     }
        //     sparks = true;
        // }
        // Checking platform collisions.
        List<Collider2D> platformColliders = meleeScript.GetPlatformColliders();
        if (platformColliders.Count > 0) sparks = true;
        // Checking lever collisions.
        List<Collider2D> leverColliders = meleeScript.GetLeverColliders();
        foreach (Collider2D collider in leverColliders) {
            Lever lever = collider.gameObject.GetComponent<Lever>();
            if (!lever.HasBeenDamaged(meleeCounter)) {
                lever.Switch();
                lever.SetMeleeCounter(meleeCounter);
            }
        }
        // Checking destrutible collisions.
        List<Collider2D> destructibleColliders = meleeScript.GetDestructibleColliders();
        foreach (Collider2D collider in destructibleColliders) {
            Destructible obj = collider.gameObject.GetComponent<Destructible>();
            if (!obj.HasBeenDamaged(meleeCounter) && obj.CanBreak()) {
                obj.Break();
                obj.SetMeleeCounter(meleeCounter);
            }
        }
        // Check if sparks should apppear.
        if (sparks) CreateSparks();
    }

    // When the player aims or throws a shuriken while standing/wall climbing.
    private void Throw() {
        if (!(CanAttack() && numShurikens > 0 && !isDashing)) return;

        // Initialize variables.
        Transform currTrans;
        Transform currFirePointTrans;
        SpriteRenderer currSprite;
        SpriteRenderer otherSprite;
        int scalar;
        // Set variables based on wallClimb status.
        if (!isWallClimbing && !isWallJumping) {
            currTrans = skillShotTrans;
            currSprite = skillShotSprite;
            otherSprite = wallSkillShotSprite;
            currFirePointTrans = firePointTrans;
            scalar = 1;
        } else {
            currTrans = wallSkillShotTrans;
            currSprite = wallSkillShotSprite;
            otherSprite = skillShotSprite;
            currFirePointTrans = wallFirePointTrans;
            scalar = -1;
        }
        // Logic for aiming skillshot.
        if (fireHolding && HoldTimeMet()) {
            currSprite.enabled = true;
            otherSprite.enabled = false;
            if (upHolding) {
                angleRaw += skillShotSpeed * Time.deltaTime;
            } else if (downHolding) {
                angleRaw -= skillShotSpeed * Time.deltaTime;
            }
            angleRaw = Mathf.Clamp(angleRaw, -60f, 60f);
            angleAdjusted = (lastDir == scalar) ? angleRaw : 180f - angleRaw;  
            currTrans.rotation = Quaternion.Euler(0, 0, angleAdjusted - 90f);

            Vector2 shootDir = GetVectorFromAngle(angleAdjusted);
            RaycastHit2D raycastHit2D = Physics2D.CircleCast(currFirePointTrans.position, shurikenRadius, GetVectorFromAngle(angleAdjusted), 50f, enemyPlatformLeverLayerMask, 0f, 0f);
            SetTargetHighlight(raycastHit2D);
        } else if (fireHolding) {
            currHoldTime += Time.deltaTime;
        } else {
            SetTargetHighlight(new RaycastHit2D());
        } 

        // Releasing the fire button.
        if (fireReleased) {
            Vector2 shootDir = (fireReleased && HoldTimeMet()) ? GetVectorFromAngle(angleAdjusted) : new Vector2(scalar * lastDir, 0f);
            StartCoroutine(SpawnShuriken(shootDir, currFirePointTrans.position));
            currHoldTime = 0f;
            angleRaw = 0f;
            currSprite.enabled = false;
            isAttacking = true;
            isThrowing = true;
            isThrowing = true;
            lastAttack = Time.time;
            numShurikens -= 1;
            UIManager.singleton.UpdateShurikenNum(numShurikens);
            Invoke("SetIsThrowingFalse", 0.5f);
        }
    }

    // Sets the highlight for the skillshot raycast.
    private void SetTargetHighlight(RaycastHit2D raycastHit2D) {
        // If the raycast hits nothing or a platform, set last target's highlight to null.
        if (raycastHit2D.collider == null || raycastHit2D.collider.tag == "Platform") {
            if (lastTarget != null) lastTarget.SetHighLight(false);
            lastTarget = null;
            return;
        }
        // Set the target's highlight and disable the last target's highlight.
        HighLight target = raycastHit2D.collider.GetComponent<HighLight>();        
        if (!ReferenceEquals(lastTarget, target) && lastTarget != null) lastTarget.SetHighLight(false);
        target.SetHighLight(true);
        lastTarget = target;
    }

    // Sets meleeActive to false to end enemy damaging.
    private void SetMeleeActiveFalse() {
        meleeActive = false;
    }

    // Returns whether the player can attack.
    private bool CanAttack() {
        return (lastAttack + attackRate <= Time.time) && !isStunned;
    }

    // Returns whether the player held the shoot button long enough to aim.
    private bool HoldTimeMet() {
        return currHoldTime >= holdTime && !isStunned;
    }

    // The amount of time needed to wallclimb to play its sound.
    private bool WallClimbTimeMet() {
        return currWallClimbTime >= wallClimbSoundTime && !isStunned;
    }

    // The distance needed to fall to play landing sound.
    private bool FallDistanceMet(Vector2 pos) {
        if (lastJumpPos == Vector2.zero) {
            return false;
        }
        float dist = Mathf.Sqrt(Mathf.Pow(lastJumpPos.x - pos.x, 2) + Mathf.Pow(lastJumpPos.y - pos.y, 2));
        return dist >= fallSoundDist && !isStunned;
    }

    // Spawns the shuriken at the fire point of the player.
    private IEnumerator SpawnShuriken(Vector2 shootDir, Vector3 spawnPos) {
        shootDir.Normalize();
        lastAttackDir = (shootDir.x >= 0f) ? 1 : -1;
        yield return new WaitForSeconds(spawnDelay);
        GameObject shuriken = Object.Instantiate(shurikenPrefab, spawnPos, Quaternion.identity);
        Shuriken shurikenScript = shuriken.GetComponent<Shuriken>();
        shurikenScript.SetShurikenVelocity(shootDir);
    }
    #endregion

    #region Sprite Rendering Functions
    // Updates the player's sprites based on input/state.
    private void UpdateSprite() {
        playerAnim.SetBool("isMoving", Mathf.Abs(playerRB.velocity.x) > 0.05f);
        playerAnim.SetBool("isJumping", playerRB.velocity.y > 0.05);
        playerAnim.SetBool("isFalling", playerRB.velocity.y < -0.05);
        playerAnim.SetBool("isGrounded", isGrounded);
        playerAnim.SetBool("isAgainstWall", isAgainstWall);
        playerAnim.SetBool("isDashing", isDashing);
        playerAnim.SetBool("isWallJumping", isWallJumping);
        playerAnim.SetBool("isSneaking", isSneaking);
        playerAnim.SetBool("isStunned", isStunned);
        playerAnim.SetBool("hasDied", hasDied);
        playerAnim.SetBool("isThrowing", isThrowing);
        playerAnim.SetBool("isMeleeing", isMeleeing);
    }

    // Sets the throwing state to false.
    private void SetIsThrowingFalse() {
        playerAnim.SetBool("isThrowing", false);
        isAttacking = false;
        isThrowing = false;
    }

    // Sets the melee state to false.
    private void SetIsMeleeingFalse() {
        playerAnim.SetBool("isMeleeing", false);
        isAttacking = false;
        isMeleeing = false;
        isPlayingStealthMeleeKill = false;
    }

    // Hides the player if crouching in front of cover.
    private void HidePlayer() {
        if (numCovered > 0 && isSneaking && alertedNum <= 0 && !isAttacking) {
            isHiding = true;
            playerSprite.color = new Color(1f, 1f, 1f, 0.5f);
            Physics2D.IgnoreLayerCollision(7, 9, true);
            Physics2D.IgnoreLayerCollision(0, 9, true);
            if (!isPlayingEnteringBushesNoise) {
                sounds.Stop("LeavingBushes");
                sounds.Play("EnteringBushes");
                isPlayingEnteringBushesNoise = true;
                isPlayingLeavingBushesNoise = false;
            }
        } else if (!isBuffering && isHiding && (numCovered <= 0 || !isSneaking || isAttacking)) {
            playerSprite.color = new Color(1f, 1f, 1f, 1f);
            isHiding = false;
            Physics2D.IgnoreLayerCollision(7, 9, false);
            Physics2D.IgnoreLayerCollision(0, 9, false);
            if (!isPlayingLeavingBushesNoise) {
                sounds.Stop("EnteringBushes");
                sounds.Play("LeavingBushes");
                isPlayingLeavingBushesNoise = true;
                isPlayingEnteringBushesNoise = false;
            }
        }
    }
    #endregion

    #region Public Functions
    public int GetPlayerDir() {
        return lastDir;
    }

    public int GetPlayerAttackDir() {
        return lastAttackDir;
    }

    public void SetStunned(bool state) {
        isStunned = state;
    }

    public void SetCoverStatus(int status) {
        numCovered += status;
    }

    public bool IsHiding() {
        return isHiding;
    }

    public bool IsSneaking() {
        return isSneaking;
    }

    public void SetHasDied(bool state) {
        hasDied = state;
    }

    public void IncreaseAlertedNumBy(int num) {
        alertedNum += num;
    }

    public void SetPlayerBuffer(bool state) {
        isBuffering = state;
    }

    public void IncreaseShurikenNumBy(int num) {
        numShurikens += num;
        UIManager.singleton.UpdateShurikenNum(numShurikens);
    }

    public void IncreaseGoldBy(int num) {
        gold += num;
        UIManager.singleton.UpdateGold(gold);
    }

    public bool CanPickUpShuriken() {
        return numShurikens < maxShurikens;
    }

    public void SetPlayerInput(bool state) {
        playerInputEnabled = state;
    }

    public void SetTitleScreenMode(bool state) {
        titleScreenModeEnabled = state;
    }

    public void SetCampFirePos(Vector2 pos) {
        lastCampFirePos = pos;
    }

    // Reset the player to the last campfire position.
    public void Reset(bool justStarted) {
        playerInputEnabled = false;
        titleScreenModeEnabled = false;
        alertedNum = 0;
        playerRB.velocity = Vector2.zero;
        playerRB.position = (justStarted) ? Vector2.zero : lastCampFirePos;
        numShurikens = startingShurikens;
        healthScript.ResetHealth(justStarted);
        UIManager.singleton.UpdateShurikenNum(numShurikens);

        skillShotSprite.enabled = false;
        wallSkillShotSprite.enabled = false;
        numCovered = 0;

        if (lastTarget != null) {
            lastTarget.SetHighLight(false);
            lastTarget = null;
        }
    }

    // Respawns the player to the last saved game state.
    public void Respawn() {
        playerInputEnabled = false;
        titleScreenModeEnabled = false;
        alertedNum = 0;
        playerRB.velocity = Vector2.zero;
        playerRB.position = spawnLocation;

        skillShotSprite.enabled = false;
        wallSkillShotSprite.enabled = false;
        numCovered = 0;

        if (lastTarget != null) {
            lastTarget.SetHighLight(false);
            lastTarget = null;
        }
    }

    // Saves the last game state of the player.
    public void SetSpawnLocation(Vector2 location) {
        spawnLocation = location;
    }

    // Increases Health by the specified amount.
    public void IncreaseHealthBy(int num) {
        if (healthScript.CanPickUpHealth()) {
            sounds.Play("HealthRegen");
            healthScript.IncreasePlayerHealth(num);
        }
    }

    // Sets the healing particles to state.
    public void SetHealthParticles(bool state) {
        highLight.SetActive(state);
    }

    // Returns whether the player has pressed continue.
    public bool HasPressedContinue() {
        return continuePressed;
    }

    // Removes an enemy from the melee list upon death.
    public void RemoveEnemyFromList(CapsuleCollider2D enemyCollider) {
        meleeScript.RemoveEnemyFromList(enemyCollider);
    }
    #endregion

    #region Misc Functions
    // Returns a vector pointing in the direction of the given angle (in degrees).
    public Vector2 GetVectorFromAngle(float angle) {
        // angle = 0 -> 360
        float angleRad = angle * (Mathf.PI / 180f);
        return new Vector2(Mathf.Cos(angleRad), Mathf.Sin(angleRad));
    }

    // Returns whether the player is set to title screen mode.
    public bool GetTitleScreenModeStatus() {
        return this.titleScreenModeEnabled;
    }
    #endregion

}