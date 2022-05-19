using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerController : MonoBehaviour
{
    #region Movement Variables
    [SerializeField]
    [Tooltip("The moveSpeed of the player.")]
    private float moveSpeed;
    [SerializeField]
    [Tooltip("The max speed the player can fall.")]
    private float maxFallSpeed;
    [SerializeField]
    [Tooltip("Speed of the player when sneaking.")]
    private float sneakSpeed;
    [Space(10)]
    #endregion

    #region Jump Variables
    [SerializeField]
    [Tooltip("The jump velocity of the player.")]
    private float jumpVel;
    [SerializeField]
    [Tooltip("The duration of a jump when holding down the jump key.")]
    private float jumpDur;
    [Space(10)]
    #endregion

    #region Dash Variables
    [SerializeField]
    [Tooltip("The duration of a dash (Uses a coroutine).")]
    private float dashDur;
    [SerializeField]
    [Tooltip("The delay between dashes.")]
    private float dashDelay;
    [SerializeField]
    [Tooltip("The distance of a dash (Uses a coroutine).")]
    private float dashDist;
    [Space(10)]
    #endregion

    #region Wall Climbing Variables
    [SerializeField]
    [Tooltip("The duration of a single wall jump in the Vector(xWallJumpVel, yWallJumpVel) direction.")]
    private float wallJumpDur;
    [SerializeField]
    [Tooltip("The speed player glides down a wall.")]
    private float wallFallSpeed;
    [Space(10)]
    #endregion

    #region Shooting Variables
    [SerializeField]
    [Tooltip("The shuriken prefab.")]
    private GameObject shurikenPrefab;
    [SerializeField]
    [Tooltip("The attack rate.")]
    private float attackRate;
    [SerializeField]
    [Tooltip("The speed a shuriken flies through the air.")]
    private float shurikenSpeed;
    [SerializeField]
    [Tooltip("The delay of spawning the shuriken.")]
    private float spawnDelay;
    [SerializeField]
    [Tooltip("The number of shurikens player spawns with.")]
    private float numShurikens;
    [SerializeField]
    [Tooltip("The spin speed of shuriken.")]
    private float spinSpeed;
    [Space(10)]
    #endregion

    #region Private Variables
    // Private Input Variables
    private KeyCode jumpKey = KeyCode.Z;
    private KeyCode dashKey = KeyCode.C;
    private KeyCode fireKey = KeyCode.Space;
    private KeyCode meleeKey = KeyCode.X;
    private KeyCode sneakKey = KeyCode.DownArrow;

    // Player Input Variables
    private bool rightPressed;
    private bool leftPressed;
    private bool jumpPressed;
    private bool jumpHolding;
    private bool jumpReleased;
    private bool dashPressed;
    private bool meleePressed;
    private bool firePressed;
    private bool sneakHolding;

    // State Variables
    private bool isJumping;
    private bool isDashing;
    private bool isWallJumping;
    private bool isSneaking;
    private bool isAttacking;
    private bool isStunned;
    private bool isGrounded;
    private bool isAgainstWall;

    // Private Dash Variables
    private int dashCounter;
    private float lastDash;

    // Private shooting variables
    private TMP_Text shurikenTxt;
    private GameObject firePoint;
    private float firePointDist;
    private float lastAttack;

    // Private Jump Variables
    private float jumpDurTimer;
    private float jumpCounter;

    // Melee private variables
    private bool meleeActive;
    private GameObject meleePoint;
    private RectTransform meleePointRectTrans;
    private Melee meleeScript;
    private float meleePointDist;
    private float meleeSpeed;
    private int lastMeleeDir;
    private static int meleeCounter;

    // General private variables
    private Rigidbody2D playerRB;
    private BoxCollider2D boxCollider2D;
    private LayerMask platformLayerMask;
    private float xInput;
    private float gravity;
    private float speed;
    private int lastDir;

    // Private Animator Private Variables
    private Animator playerAnim;
    private SpriteRenderer playerSprite;
    #endregion

/**********************************************************************************/

    #region Initializing Functions
    // Start is called before the first frame update
    void Start()
    {
        playerRB = GetComponent<Rigidbody2D>();
        boxCollider2D = GetComponent<BoxCollider2D>();
        playerAnim = GetComponent<Animator>();
        playerSprite = GetComponent<SpriteRenderer>();
        shurikenTxt = GameObject.Find("Shurikens").GetComponent<TMP_Text>();
        firePoint = this.transform.GetChild(0).gameObject;
        meleePoint = this.transform.GetChild(1).gameObject;
        meleePointRectTrans = meleePoint.GetComponent<RectTransform>();
        meleeScript = meleePoint.GetComponent<Melee>();
        platformLayerMask = LayerMask.GetMask("Platform");

        speed = moveSpeed;
        lastDir = 1;
        lastMeleeDir = 1;
        gravity = playerRB.gravityScale;
        firePointDist = 1.0f;
        meleePointDist = 0.23f;
        isStunned = false;
        meleeActive = false;
        meleeCounter = 0;
    }
    #endregion

    #region Update Functions
    // Update is called once per frame
    void Update()
    {
        // Get Player Input
        xInput = Input.GetAxisRaw("Horizontal");
        jumpPressed = Input.GetKeyDown(jumpKey);
        jumpHolding = Input.GetKey(jumpKey);
        jumpReleased = Input.GetKeyUp(jumpKey);
        dashPressed = Input.GetKeyDown(dashKey);
        sneakHolding = Input.GetKey(sneakKey);
        meleePressed = Input.GetKeyDown(meleeKey);
        firePressed = Input.GetKeyDown(fireKey);
    }

    private void FixedUpdate() {
        IsGrounded();
        IsAgainstWall();
        setDirection();
        Move();
        Attack();
        UpdateSprite();
    }

    #endregion

    #region Movement Functions
    // Controls the player's movement.
    void Move() {

        // Limits the velocity when falling
        playerRB.velocity = new Vector2(playerRB.velocity.x, Mathf.Clamp(playerRB.velocity.y, -maxFallSpeed, maxFallSpeed));
        
        if (!isStunned) {
            // Regular movement
            if (!isDashing && !isWallJumping && !isAttacking) {
                playerRB.velocity = new Vector2(xInput * speed, playerRB.velocity.y);
            }

            // Tapping the jump button
            if (jumpPressed && (isGrounded || jumpCounter > 0) && !isDashing && !isWallJumping && !isAgainstWall) {
                isJumping = true;
                jumpDurTimer = jumpDur;
                playerRB.velocity = new Vector2(playerRB.velocity.x, jumpVel);
                jumpCounter -= 1;
            }

            // Holding the jump button
            if (jumpHolding && isJumping == true && !isDashing && !isWallJumping && !isAgainstWall) {
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

            // Wall Jumping
            if (!isDashing && isAgainstWall) {
                playerRB.velocity = new Vector2(0f, -1f * wallFallSpeed);
                if (jumpPressed) {
                    isWallJumping = true;
                    Invoke("SetWallJumpingFalse", wallJumpDur);
                }
            }

            if (isWallJumping) {
                playerRB.velocity = new Vector2(-lastDir * moveSpeed, jumpVel);
            }
        }
    }

    // Sets the variable: lastDir based on the xInput of the player.
    private void setDirection() {
        if (xInput > 0 && !isWallJumping && !isAttacking && !isDashing) {
            lastDir = 1;
            SwitchAttackPoint();
        } else if (xInput < 0 && !isWallJumping && !isAttacking && !isDashing) {
            lastDir = -1;
            SwitchAttackPoint();
        }
    }

    // Sets Wall Climbing to Flase
    private void SetWallJumpingFalse() {
        isWallJumping = false;
    }

    // Controls the dashing process.
    IEnumerator Dash() {
        if (!isStunned) {
            isDashing = true;
            dashCounter -= 1;
            playerRB.constraints = RigidbodyConstraints2D.FreezePositionY | RigidbodyConstraints2D.FreezeRotation;
            playerRB.velocity = new Vector2(playerRB.velocity.x, 0f);
            playerRB.AddForce(new Vector2(dashDist * lastDir, 0f), ForceMode2D.Impulse);
            playerRB.gravityScale = 0;
            yield return new WaitForSeconds(dashDur);
            playerRB.gravityScale = gravity;
            playerRB.constraints = RigidbodyConstraints2D.FreezeRotation;
            isDashing = false;
        }
    }

    public bool CanDash() {
        return (lastDash + dashDelay <= Time.time) && !isStunned;
    }

    // Determines if the player is standing on ground.
    private void IsGrounded() {
        RaycastHit2D raycastHit2D = Physics2D.BoxCast(boxCollider2D.bounds.center, boxCollider2D.bounds.size, 0f, Vector2.down, 0.1f, platformLayerMask);
        bool onGround = raycastHit2D.collider != null;
        if (onGround) {
            jumpCounter = 2;
            dashCounter = 1;
        }
        isGrounded = onGround;
        return;
    }

    // Returns if the player is jumping against a wall.
    private void IsAgainstWall() {
        RaycastHit2D raycastHit2D;
        // Check right
        if (lastDir == 1) {
           raycastHit2D = Physics2D.BoxCast(boxCollider2D.bounds.center, boxCollider2D.bounds.size, 0f, Vector2.right, 0.1f, platformLayerMask);
        // Check left
        } else {
            raycastHit2D = Physics2D.BoxCast(boxCollider2D.bounds.center, boxCollider2D.bounds.size, 0f, Vector2.left, 0.1f, platformLayerMask);
        }
        bool againstWall = raycastHit2D.collider != null;

        if (againstWall) {
            dashCounter = 1;
            jumpCounter = 1;
        }
        isAgainstWall = againstWall;
        return; 
    }
    #endregion

    #region Shooting Functions
    private void Attack() {
        if (firePressed && CanAttack() && numShurikens > 0 && !isDashing) {
            StartCoroutine("SpawnShuriken");
        } else if (meleePressed && CanAttack()) {
            meleeActive = true;
            meleeCounter += 1;
            Invoke("SetMeleeActiveFalse", 0.18f);

        }

        if (meleeActive) {
            List<Collider2D> enemyColliders = meleeScript.GetEnemyColliders();
            foreach (Collider2D collider in enemyColliders) {
                EnemyController enemy = collider.gameObject.GetComponent<EnemyController>();
                if (!enemy.HasBeenDamaged(meleeCounter)) {
                    if (enemy.IsAlerted()) {
                        enemy.TakeDmg(1);
                    } else {
                        enemy.TakeDmg(5);
                    }
                    enemy.SetDamagedCounter(meleeCounter);
                }
                Debug.Log("Enemy health: " + enemy.GetHealth());
            }

            List<Collider2D> projectileColliders = meleeScript.GetProjectileColliders();
            foreach (Collider2D collider in projectileColliders) {
                Shuriken shuriken = collider.gameObject.GetComponent<Shuriken>();
                if (!shuriken.HasBeenDeflected(meleeCounter)) {
                    shuriken.Deflected();
                    shuriken.SetDeflectedCounter(meleeCounter);
                }
            }
        }
    }

    private void SetMeleeActiveFalse() {
        meleeActive = false;
    }

    public bool CanAttack() {
        return (lastAttack + attackRate <= Time.time) && !isStunned;
    }

    private IEnumerator SpawnShuriken() {
        yield return new WaitForSeconds(spawnDelay);
        GameObject shuriken = Object.Instantiate(shurikenPrefab, firePoint.transform.position, Quaternion.identity);
        Shuriken shurikenScript = shuriken.GetComponent<Shuriken>();
        Rigidbody2D rb = shuriken.GetComponent<Rigidbody2D>();
        shurikenScript.SetShurikenDir(lastDir);
        if (lastDir == -1) {
            rb.AddTorque(spinSpeed, ForceMode2D.Force);
        } else {
            rb.AddTorque(-spinSpeed, ForceMode2D.Force);  
        }  
        rb.velocity = new Vector2(lastDir * shurikenSpeed, 0);

    }

    // Switches the attack point gameObject of the player based on direction.
    private void SwitchAttackPoint() {
        Vector3 firePos = firePoint.transform.position;
        Vector3 meleePos = meleePoint.transform.position;
        if (lastDir == -1) {
            firePos.x = this.transform.position.x - firePointDist;
            meleePos.x = this.transform.position.x - meleePointDist;
        } else {
            firePos.x = this.transform.position.x + firePointDist;
            meleePos.x = this.transform.position.x + meleePointDist;
        }
        if (lastDir != lastMeleeDir) {
            meleePointRectTrans.Rotate(new Vector3(0, 180, 0), Space.Self);
            lastMeleeDir = lastDir;
        }
        firePoint.transform.position = firePos;
        meleePoint.transform.position = meleePos;
    }
    #endregion

    #region Sprite Rendering Functions
    private void UpdateSprite() {
        if (lastDir == 1) {
            playerSprite.flipX = false;
        } else if (lastDir == -1) {
            playerSprite.flipX = true;
        }

        if (Mathf.Abs(playerRB.velocity.x) > 0) {
            playerAnim.SetBool("isMoving", true);
        } else {
            playerAnim.SetBool("isMoving", false);
        }

        if (playerRB.velocity.y > 0) {
            playerAnim.SetBool("isJumping", true);
        } else {
            playerAnim.SetBool("isJumping", false);
        }
        
        if (playerRB.velocity.y < 0) {
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
            playerAnim.SetBool("isWallClimbing", true);
        } else {
            playerAnim.SetBool("isWallClimbing", false);
        }

        if (firePressed && CanAttack() && numShurikens > 0 && !isDashing) {
            isAttacking = true;
            playerAnim.SetBool("isThrowing", true);
            lastAttack = Time.time;
            numShurikens -= 1;
            shurikenTxt.text = "Shurikens: " + numShurikens.ToString();
            Invoke("SetIsThrowingFalse", 0.5f);
        }

        if (meleePressed && CanAttack()) {
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
    }

    private void SetIsThrowingFalse() {
        playerAnim.SetBool("isThrowing", false);
        isAttacking = false;
    }

    private void SetIsMeleeingFalse() {
        playerAnim.SetBool("isMeleeing", false);
        isAttacking = false;
    }
    #endregion

    #region Public Functions
    public int GetPlayerDir() {
        return lastDir;
    }

    public void SetStunned(bool state) {
        isStunned = state;
    }
    #endregion
}
