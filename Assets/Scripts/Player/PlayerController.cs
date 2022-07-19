using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.IO.IsolatedStorage;

public class PlayerController : MonoBehaviour
{
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
    [Tooltip("The number of shurikens player spawns with.")]
    private int numShurikens;
    [SerializeField]
    [Tooltip("The knock back force when clashing with a platform or enemy.")]
    private float knockBackForce;
    [SerializeField]
    [Tooltip("The knock back duration when clashing with a platform or enemy.")]
    private float knockBackDur;
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

    #region Private Variables
    // Private Input Variables
    private KeyCode jumpKey = KeyCode.Z;
    private KeyCode dashKey = KeyCode.C;
    private KeyCode fireKey = KeyCode.Space;
    private KeyCode meleeKey = KeyCode.X;
    private KeyCode sneakKey = KeyCode.LeftControl;
    private KeyCode upKey = KeyCode.UpArrow;
    private KeyCode downKey = KeyCode.DownArrow;

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
    private bool isCovered;
    private bool isHiding;
    private bool isDamaged;
    private bool hasDied;
    private bool isBuffering;

    // Private Dash Variables
    private int dashCounter;
    private float lastDash;

    // Private shooting variables
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

    // Private Jump Variables
    private float jumpDurTimer;
    private float jumpCounter;
    private float lastJump;

    // Melee private variables
    private Melee meleeScript;
    private bool meleeActive;
    private float meleeSpeed;
    private static int meleeCounter;
    private bool hitPlatform;
    private Transform point0;
    private Transform point1;
    private Transform point2;

    // General private variables
    private Rigidbody2D playerRB;
    private Collider2D boxCollider2D;
    private LayerMask platformLayerMask;
    private LayerMask allPlatformsLayerMask;
    private TrailRenderer dashTrail;
    private TrailRenderer doubleJumpTrail;

    private float xInput;
    private float gravity;
    private float speed;
    private int lastDir;
    private int side;
    private int alertedNum;

    // Private Animator Private Variables
    private Animator playerAnim;
    private SpriteRenderer playerSprite;
    #endregion

/**********************************************************************************/

    #region Initializing Functions
    // Awake is called before Start
    private void Awake() {
        // Getting Player Components
        playerRB = GetComponent<Rigidbody2D>();
        boxCollider2D = GetComponent<Collider2D>();
        playerAnim = GetComponent<Animator>();
        playerSprite = GetComponent<SpriteRenderer>();
        dashTrail = this.transform.GetChild(2).GetChild(1).GetComponent<TrailRenderer>();
        doubleJumpTrail = this.transform.GetChild(2).GetChild(2).GetComponent<TrailRenderer>();
        platformLayerMask = LayerMask.GetMask("Platform");
        allPlatformsLayerMask = LayerMask.GetMask("Platform", "OneWayPlatform");
        meleeScript = this.transform.GetChild(1).GetComponent<Melee>();
        
        firePointTrans = this.transform.GetChild(0).transform;
        skillShotTrans = firePointTrans.GetChild(0).transform;
        skillShotSprite = skillShotTrans.GetComponent<SpriteRenderer>();

        wallFirePointTrans = this.transform.GetChild(5).transform;
        wallSkillShotTrans = wallFirePointTrans.GetChild(0).transform;
        wallSkillShotSprite = wallSkillShotTrans.GetComponent<SpriteRenderer>();

        point0 = this.transform.GetChild(1).GetChild(1).transform;
        point1 = this.transform.GetChild(1).GetChild(2).transform;
        point2 = this.transform.GetChild(1).GetChild(3).transform;
    }

    // Start is called before the first frame update
    void Start()
    {
        speed = moveSpeed;
        lastDir = 1;
        gravity = playerRB.gravityScale;
        isStunned = false;
        meleeActive = false;
        meleeCounter = 0;
        dashTrail.emitting = false;
        doubleJumpTrail.emitting = false;
        //meleeTrail.emitting = false;

        ScoreManager.singleton.UpdateShurikenNum(numShurikens);
    }
    #endregion

    #region Update Functions
    // Update is called once per frame
    void Update()
    {
        //Debug.Log("num: " + alertedNum);
        
        // Limits the velocity when falling
        playerRB.velocity = new Vector2(playerRB.velocity.x, Mathf.Clamp(playerRB.velocity.y, -maxFallSpeed, maxFallSpeed));

        if (!hasDied) {
            // Get Player Input
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

            Move();
            IsGrounded();
            IsAgainstWall();
            SetDirection();
            if (isWallClimbing) {
                WallClimbAttack();
            } else {
                Attack();
            }
            CoverPlayer();
        }
        UpdateSprite();
    }
    #endregion

    #region Movement Functions
    // Controls the player's movement.
    void Move() {
        if (!isStunned) {
            // Regular movement
            if (!isDashing && !isWallJumping && !isAttacking) {
                playerRB.velocity = new Vector2(xInput * speed, playerRB.velocity.y);
                if (isGrounded && Mathf.Abs(playerRB.velocity.x) > 0 && !isSneaking) {
                    CreateDust(0);
                }
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

                if (!isGrounded) {
                    doubleJumpTrail.emitting = true;
                    doubleJumpTrail.time = 0.25f;
                    StartCoroutine(ReduceTrail(doubleJumpTrail, false));
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
            if (jumpReleased) {
                isJumping = false;
            }

            // Tapping the dash button.
            if (dashPressed && !isDashing && !isJumping && !isWallJumping && (dashCounter > 0 || isGrounded) && CanDash()) {
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
                    CreateDust(1);
                } else {
                    isWallClimbing = false;
                }
                // Setting Wall Jumping to true
                if (jumpPressed && CanJump()) {
                    isWallJumping = true;
                    Invoke("SetWallJumpingFalse", wallJumpDur);
                }
            } else {
                isWallClimbing = false;
            }

            // Wall Jumping
            if (isWallJumping) {
                playerRB.velocity = new Vector2(-lastDir * moveSpeed, jumpVel);
                lastJump = Time.time;
            }
        }
    }

    // Sets the variable: lastDir based on the xInput of the player.
    private void SetDirection() {
        if (!isWallJumping && !isAttacking && !isDashing && !isStunned) {
            if (xInput > 0) {
                lastDir = 1;
            } else if (xInput < 0) {
                lastDir = -1;
            }
            FlipPlayer();
        }
    }

    // Sets the direction of the player to where it's moving.
    private void FlipPlayer() {
        if (lastDir == 1) {
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        } else if (lastDir == -1) {
            transform.localScale = new Vector3(-1f * Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
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
            if (isGrounded) {
                CreateDust(0);
            }
            playerRB.gravityScale = 0;
            dashTrail.emitting = true;
            dashTrail.time = 0.25f;
            playerRB.AddForce(new Vector2(dashForce * lastDir, 0f), ForceMode2D.Impulse);
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

        if (destroy) {
            Destroy(trail.gameObject);
        }
    }

    void CreateDust(int s) {
        // Create ground dust
        if (s == 0) {
            groundDust.Play();
        // Create wall dust on appropriate side.
        } else if (s == 1) {
            wallClimbDust.Play();
        }
    }

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
    // Determines if the player is standing on ground.
    private void IsGrounded() {
        bool groundStatus = isGrounded;
        RaycastHit2D raycastHit2D = Physics2D.BoxCast(boxCollider2D.bounds.center, new Vector2(0.6f, boxCollider2D.bounds.size.y - 0.1f), 0f, Vector2.down, 0.2f, allPlatformsLayerMask);
        bool onGround = raycastHit2D.collider != null;
        if (onGround) {
            jumpCounter = 1;
            dashCounter = 1;
        }
        isGrounded = onGround;
        if (groundStatus == false && isGrounded == true) {
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
    private void Attack() {
        if (CanAttack() && numShurikens > 0 && !isDashing && !isWallClimbing && !isWallJumping) {
            if (fireHolding && HoldTimeMet()) {
                skillShotSprite.enabled = true;
                wallSkillShotSprite.enabled = false;
                if (upHolding) {
                    angleRaw += skillShotSpeed * Time.deltaTime;
                } else if (downHolding) {
                    angleRaw -= skillShotSpeed * Time.deltaTime;
                }
                angleRaw = Mathf.Clamp(angleRaw, -60f, 60f);
                if (lastDir == 1) {
                    angleAdjusted = angleRaw;
                } else {
                    angleAdjusted = 180f - angleRaw;
                }
                skillShotTrans.rotation = Quaternion.Euler(0, 0, angleAdjusted - 90f);
            } else if (fireHolding) {
                currHoldTime += Time.deltaTime;
            }

            if (fireReleased && HoldTimeMet()) {
                Vector2 shootDir = GetVectorFromAngle(angleAdjusted);
                StartCoroutine(SpawnShuriken(shootDir, firePointTrans.position));
                currHoldTime = 0f;
                angleRaw = 0f;
            } else if (fireReleased) {
                Vector2 shootDir = new Vector2(lastDir, 0f);
                StartCoroutine(SpawnShuriken(shootDir, firePointTrans.position));
                currHoldTime = 0f;
                angleRaw = 0f;
            }

        } else if (meleePressed && CanAttack() && isGrounded) {
            skillShotSprite.enabled = false;
            if (isGrounded) {
                CreateDust(0);
            }
            meleeActive = true;
            meleeCounter += 1;
            lastAttackDir = lastDir;
            StartCoroutine("MeleeTrail");
            Invoke("SetMeleeActiveFalse", 0.18f);
        } else {
            skillShotSprite.enabled = false;
        }

        if (meleeActive) {
            bool contact = false;
            bool sparks = false;
            List<Collider2D> enemyColliders = meleeScript.GetEnemyColliders();
            foreach (Collider2D collider in enemyColliders) {
                EnemyController enemy = collider.gameObject.GetComponent<EnemyController>();
                if (!enemy.HasBeenDamaged(meleeCounter) && !enemy.HasDied()) {
                    if (enemy.IsAlerted()) {
                        enemy.TakeDmg(1);
                    } else {
                        enemy.TakeDmg(5);
                    }
                    enemy.SetDamagedCounter(meleeCounter);
                }
                contact = true;
            }

            List<Collider2D> projectileColliders = meleeScript.GetProjectileColliders();
            foreach (Collider2D collider in projectileColliders) {
                Shuriken shuriken = collider.gameObject.GetComponent<Shuriken>();
                if (!shuriken.HasBeenDeflected(meleeCounter)) {
                    shuriken.Deflected();
                    shuriken.SetDeflectedCounter(meleeCounter);
                }
                contact = true;
                sparks = true;
            }

            List<Collider2D> platformColliders = meleeScript.GetPlatformColliders();
            if (platformColliders.Count > 0) {
                contact = true;
                sparks = true;
                hitPlatform = true;
            } else {
                hitPlatform = false;
            }

            if (contact) {
                Vector2 dir = new Vector2(-lastDir, 0f);
                //StartCoroutine(KnockBack(dir));
            }

            if (sparks) {
                CreateSparks();
            }
        }
    }

    // Controls the Shuriken attack when wallClimbing
    private void WallClimbAttack() {
        if (CanAttack() && numShurikens > 0 && !isDashing) {
            if (fireHolding && HoldTimeMet()) {
                wallSkillShotSprite.enabled = true;
                skillShotSprite.enabled = false;
                if (upHolding) {
                    angleRaw += skillShotSpeed * Time.deltaTime;
                } else if (downHolding) {
                    angleRaw -= skillShotSpeed * Time.deltaTime;
                }
                angleRaw = Mathf.Clamp(angleRaw, -60f, 60f);
                if (lastDir == -1) {
                    angleAdjusted = angleRaw;
                } else {
                    angleAdjusted = 180f - angleRaw;
                }
                wallSkillShotTrans.rotation = Quaternion.Euler(0, 0, angleAdjusted - 90f);
            } else if (fireHolding) {
                currHoldTime += Time.deltaTime;
            }

            if (fireReleased && HoldTimeMet()) {
                Vector2 shootDir = GetVectorFromAngle(angleAdjusted);
                StartCoroutine(SpawnShuriken(shootDir, wallFirePointTrans.position));
                currHoldTime = 0f;
                angleRaw = 0f;
            } else if (fireReleased) {
                Vector2 shootDir = new Vector2(-lastDir, 0f);
                StartCoroutine(SpawnShuriken(shootDir, wallFirePointTrans.position));
                currHoldTime = 0f;
                angleRaw = 0f;
            }
        } else {
            wallSkillShotSprite.enabled = false;
        }
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

    // Makes the trail for the melee attack.
    IEnumerator MeleeTrail() {
        Vector3 newPos = new Vector3(0f, 0f, 0f);
        Vector3 p0 = point0.position;
        Vector3 p1 = point1.position;
        Vector3 p2 = point2.position;

        //GameObject meleeBall = GameObject.Instantiate(meleeBallPrefab, meleePointRectTrans, false);
        GameObject meleeBall = GameObject.Instantiate(meleeBallPrefab, p0, Quaternion.identity);
        TrailRenderer meleeTrail = meleeBall.GetComponent<TrailRenderer>();
        //Rigidbody2D meleeBallRB = meleeBall.GetCOmponent<Rigidbody2D>();
        meleeTrail.emitting = true;
        meleeTrail.time = meleeTrailTime;

        float lastTValue = 0f;
        // Uses Bezier Curves to interpolate the ball's position along three points.
        for (float t = 0; t <= 1.0f; t += Time.deltaTime * meleeBallSpeed) {
            lastTValue = t;
            if (hitPlatform) {
                meleeTrail.emitting = false;
                Destroy(meleeBall);
                yield break;
            }
            newPos = Mathf.Pow(1 - t, 2) * p0 + 2 * (1 - t) * t * p1 + Mathf.Pow(t, 2) * p2;
            meleeBall.transform.position = newPos;
            yield return new WaitForEndOfFrame();
        }

        // Sets the last position of the melee ball if it did not reach the full swing.
        if (lastTValue < 1.0f) {
            meleeBall.transform.position = p2;
            yield return new WaitForEndOfFrame();
        }

        //meleeBall.transform.parent = null;
        StartCoroutine(ReduceTrail(meleeTrail, true));
    }

    // Knocks the player back when attacking an enemy or platform.
    IEnumerator KnockBack(Vector2 dir) {
        yield return new WaitForSeconds(0.001f);
        playerRB.velocity = new Vector2(0f, 0f);
        playerRB.AddForce(dir * knockBackForce, ForceMode2D.Impulse);
        yield return new WaitForSeconds(knockBackDur);
    }

    // Spawns the shuriken at the fire point of the player.
    private IEnumerator SpawnShuriken(Vector2 shootDir, Vector3 spawnPos) {
        shootDir.Normalize();
        if (shootDir.x >= 0f) {
            lastAttackDir = 1;
        } else {
            lastAttackDir = -1;
        }
        yield return new WaitForSeconds(spawnDelay);
        GameObject shuriken = Object.Instantiate(shurikenPrefab, spawnPos, Quaternion.identity);
        Shuriken shurikenScript = shuriken.GetComponent<Shuriken>();
        shurikenScript.SetShurikenVelocity(shootDir);
    }
    #endregion

    #region Sprite Rendering Functions
    // Updates the player's sprites based on input/state.
    private void UpdateSprite() {
        if (Mathf.Abs(playerRB.velocity.x) > 0) {
            playerAnim.SetBool("isMoving", true);
        } else {
            playerAnim.SetBool("isMoving", false);
        }

        if (playerRB.velocity.y > 0.001) {
            playerAnim.SetBool("isJumping", true);
        } else {
            playerAnim.SetBool("isJumping", false);
        }
        
        if (playerRB.velocity.y < -0.001) {
            playerAnim.SetBool("isFalling", true);
        } else {
            playerAnim.SetBool("isFalling", false);
        }

        if (isGrounded) {
            playerAnim.SetBool("isGrounded", true);
        } else {
            playerAnim.SetBool("isGrounded", false);
        }

        if (isAgainstWall) {
            playerAnim.SetBool("isAgainstWall", true);
        } else {
            playerAnim.SetBool("isAgainstWall", false);
        }

        if (fireReleased && CanAttack() /*&& IsCharged()*/ && numShurikens > 0 && !isDashing) {
            isAttacking = true;
            playerAnim.SetBool("isThrowing", true);
            lastAttack = Time.time;
            //currChargeTime = 0f;
            numShurikens -= 1;
            ScoreManager.singleton.UpdateShurikenNum(numShurikens);
            Invoke("SetIsThrowingFalse", 0.5f);
        }

        if (meleePressed && CanAttack() && isGrounded) {
            isAttacking = true;
            playerAnim.SetBool("isMeleeing", true);
            lastAttack = Time.time;
            Invoke("SetIsMeleeingFalse", 0.5f);
        }

        if (isDashing) {
            playerAnim.SetBool("isDashing", true);
        } else {
            playerAnim.SetBool("isDashing", false);
        }

        if (isWallJumping) {
            playerAnim.SetBool("isWallJumping", true);
        } else {
            playerAnim.SetBool("isWallJumping", false);
        }

        if (isSneaking) {
            playerAnim.SetBool("isSneaking", true);
        } else {
            playerAnim.SetBool("isSneaking", false);
        }

        if (isStunned) {
            playerAnim.SetBool("isStunned", true);
        } else {
            playerAnim.SetBool("isStunned", false);
        }

        if (hasDied) {
            playerAnim.SetBool("hasDied", true);
        } else {
            playerAnim.SetBool("hasDied", false);
        }
    }

    private void SetIsThrowingFalse() {
        playerAnim.SetBool("isThrowing", false);
        isAttacking = false;
    }

    private void SetIsMeleeingFalse() {
        playerAnim.SetBool("isMeleeing", false);
        isAttacking = false;
    }

    private void CoverPlayer() {
        if (isCovered && isSneaking && alertedNum <= 0) {
            isHiding = true;
            playerSprite.color = new Color(1f, 1f, 1f, 0.5f);
            Physics2D.IgnoreLayerCollision(7, 9, true);
            Physics2D.IgnoreLayerCollision(0, 9, true);
        } else if (!isBuffering) {
            playerSprite.color = new Color(1f, 1f, 1f, 1f);
            isHiding = false;
            Physics2D.IgnoreLayerCollision(7, 9, false);
            Physics2D.IgnoreLayerCollision(0, 9, false);
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

    public void SetCoverStatus(bool status) {
        isCovered = status;
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
    #endregion

    #region Misc Functions
    public Vector2 GetVectorFromAngle(float angle) {
        // angle = 0 -> 360
        float angleRad = angle * (Mathf.PI/180f);
        return new Vector2(Mathf.Cos(angleRad), Mathf.Sin(angleRad));
    }
    #endregion

}