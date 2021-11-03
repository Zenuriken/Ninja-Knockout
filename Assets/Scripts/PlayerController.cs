using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    #region Movement Variables
    [SerializeField]
    [Tooltip("The moveSpeed of the player.")]
    private float moveSpeed;
    #endregion

    #region Jump Variables
    [SerializeField]
    [Tooltip("The platform layer. Used to check if player is on the ground.")]
    private LayerMask platformLayerMask;

    [SerializeField]
    [Tooltip("The jump velocity of the player.")]
    private float jumpVel;

    [SerializeField]
    [Tooltip("The duration of a jump when holding down the jump key.")]
    private float jumpDur;

    // Private Jump Variables
    private KeyCode jumpKey = KeyCode.Z;
    private float jumpDurTimer;
    private bool isJumping;
    private float jumpCounter;
    #endregion

    #region Dash Variables
    [SerializeField]
    [Tooltip("The duration of a dash (Uses a coroutine).")]
    private float dashDur;

    [SerializeField]
    [Tooltip("The distance of a dash (Uses a coroutine).")]
    private float dashDist;

    // Private Dash Variables
    private KeyCode dashKey = KeyCode.C;
    private float dashCounter;
    private bool isDashing;
    #endregion

    #region Sneaking Variables
    [SerializeField]
    [Tooltip("Speed of the player when sneaking.")]
    private float sneakSpeed;

    // Private Sneaking Variables
    private bool isSneaking;
    #endregion

    #region Wall Climbing Variables

    [SerializeField]
    [Tooltip("The duration of a single wall jump in the Vector(xWallJumpVel, yWallJumpVel) direction.")]
    private float wallJumpDur;

    [SerializeField]
    [Tooltip("The speed player glides down a wall.")]
    private float wallFallSpeed;

    // Private Wall Climbing Variables
    private bool isWallJumping;
    #endregion

    #region Shooting Variables
    [SerializeField]
    [Tooltip("The shuriken fire rate.")]
    private float fireRate;

    [SerializeField]
    [Tooltip("The fire point of the shuriken.")]
    private GameObject firePoint;

    [SerializeField]
    [Tooltip("The shuriken prefab.")]
    private GameObject shurikenPrefab;

    [SerializeField]
    [Tooltip("The speed a shuriken flies through the air.")]
    private float shurikenSpeed;

    [SerializeField]
    [Tooltip("The delay of spawning the shuriken.")]
    private float spawnDelay;


    // Shooting private variables
    private KeyCode fireKey = KeyCode.Space;
    private float lastFire;
    private float firePointDist;
    #endregion

    #region Melee Functions
    private KeyCode meleeKey = KeyCode.X;
    private float meleeSpeed;
    #endregion

    #region General Private Variables
    private Rigidbody2D playerRB;
    private BoxCollider2D boxCollider2D;
    private float xInput;
    private float lastDir;
    private float gravity;
    private float speed;
    #endregion

    #region Animator Private Variables
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
        speed = moveSpeed;
        gravity = playerRB.gravityScale;
        firePointDist = 1.0f;
    }
    #endregion

    #region Update Functions
    // Update is called once per frame
    void Update()
    {
        Move();
        Shoot();
        UpdateSprite();
    }
    #endregion

    #region Movement Functions
    // Controls the player's movement.
    void Move() {

        // Set the current direction of player.
        setDirection();
        
        // Regular movement
        if (!isDashing && !isWallJumping) {
            playerRB.velocity = new Vector2(xInput * speed, playerRB.velocity.y);
        }

        // Tapping the jump button
        if (Input.GetKeyDown(jumpKey) && (IsGrounded() || jumpCounter > 0) && !isDashing && !isWallJumping && !IsAgainstWall()) {
            isJumping = true;
            jumpDurTimer = jumpDur;
            playerRB.velocity = new Vector2(playerRB.velocity.x, jumpVel);
            jumpCounter -= 1;
        }

        // Holding the jump button
        if (Input.GetKey(jumpKey) && isJumping == true && !isDashing && !isWallJumping && !IsAgainstWall()) {
            if (jumpDurTimer > 0) {
                playerRB.velocity = new Vector2(playerRB.velocity.x, jumpVel);
                jumpDurTimer -= Time.deltaTime;
            } else {
                isJumping = false;
            }
        }

        // Releasing the jump button
        if (Input.GetKeyUp(jumpKey)) {
            isJumping = false;
        }

        // Tapping the dash button.
        if (Input.GetKeyDown(dashKey) && !isDashing && !isJumping && (dashCounter > 0 || IsGrounded())) {
            StartCoroutine(Dash());
        }

        // Sneaking
        if (Input.GetKeyDown(KeyCode.DownArrow) && IsGrounded()) {
            isSneaking = true;
            speed = sneakSpeed;
        }

        if (Input.GetKeyUp(KeyCode.DownArrow)) {
            speed = moveSpeed;
            isSneaking = false;
        }

        // Wall Jumping
        if (!isDashing && IsAgainstWall()) {
            playerRB.velocity = new Vector2(0f, -1f * wallFallSpeed);
            if (Input.GetKeyDown(jumpKey)) {
                isWallJumping = true;
                Invoke("SetWallJumpingFalse", wallJumpDur);
            }
        }

        if (isWallJumping) {
            playerRB.velocity = new Vector2(-lastDir * moveSpeed, jumpVel);
        }
    }

    // Sets the variable: lastDir based on the xInput of the player.
    private void setDirection() {
        xInput = Input.GetAxisRaw("Horizontal");
        if (xInput > 0 && !isWallJumping) {
            lastDir = 1;
            SwitchAttackPoint();
        } else if (xInput < 0 && !isWallJumping) {
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
        isDashing = true;
        dashCounter -= 1;
        playerRB.velocity = new Vector2(playerRB.velocity.x, 0f);
        playerRB.AddForce(new Vector2(dashDist * lastDir, 0f), ForceMode2D.Impulse);
        playerRB.gravityScale = 0;
        yield return new WaitForSeconds(dashDur);
        playerRB.gravityScale = gravity;
        isDashing = false;
    }

    // Determines if the player is standing on ground.
    private bool IsGrounded() {
        RaycastHit2D raycastHit2D = Physics2D.BoxCast(boxCollider2D.bounds.center, boxCollider2D.bounds.size, 0f, Vector2.down, 0.1f, platformLayerMask);
        bool onGround = raycastHit2D.collider != null;
        if (onGround) {
            jumpCounter = 2;
            dashCounter = 1;
        }
        return onGround;
    }

    // Returns if the player is jumping against a wall.
    private bool IsAgainstWall() {
        RaycastHit2D raycastHit2D;
        string side;
        if (lastDir == 1) {
           raycastHit2D = Physics2D.BoxCast(boxCollider2D.bounds.center, boxCollider2D.bounds.size, 0f, Vector2.right, 0.1f, platformLayerMask);
           side = "right";
        } else {
            raycastHit2D = Physics2D.BoxCast(boxCollider2D.bounds.center, boxCollider2D.bounds.size, 0f, Vector2.left, 0.1f, platformLayerMask);
            side = "left";
        }
        bool againstWall = raycastHit2D.collider != null;

        if (againstWall) {
            dashCounter = 1;
            jumpCounter = 1;
        }
        return againstWall; 
    }
    #endregion

    #region Shooting Functions
    // Firing
    private void Shoot() {
        if (Input.GetKeyDown(fireKey) && CanFire()) {
            //Invoke("SpawnShuriken", spawnDelay);
            StartCoroutine("SpawnShuriken");
        }
    }

    private bool CanFire() {
        return lastFire + fireRate <= Time.time;
    }

    // private void SpawnShuriken() {
    //     GameObject shuriken = Object.Instantiate(shurikenPrefab, firePoint.transform.position, Quaternion.identity);
    //     Rigidbody2D rb = shuriken.GetComponent<Rigidbody2D>();
    //     rb.velocity = new Vector2(lastDir * shurikenSpeed, 0);
    //     lastFire = Time.time;
    // }

    // private void Melee() {
    //     if (Input.GetKeyDown(meleeKey) && CanFire()) {
    //         StartCoroutine("StartMelee");
    //     }
    // }

    // private IEnumerable StartMelee() {
    
    // }

    private IEnumerator SpawnShuriken() {
        yield return new WaitForSeconds(spawnDelay);
        GameObject shuriken = Object.Instantiate(shurikenPrefab, firePoint.transform.position, Quaternion.identity);
        Rigidbody2D rb = shuriken.GetComponent<Rigidbody2D>();
        rb.velocity = new Vector2(lastDir * shurikenSpeed, 0);
        lastFire = Time.time;
    }

    // Switches the attack point gameObject of the player based on direction.
    private void SwitchAttackPoint() {
        Vector3 pos = firePoint.transform.position;
        if (lastDir == -1) {
            pos.x = this.transform.position.x - firePointDist;
        } else {
            pos.x = this.transform.position.x + firePointDist;
        }
        firePoint.transform.position = pos;
    }
    #endregion

    #region Sprite Rendering Functions
    private void UpdateSprite() {
        if (playerRB.velocity.x > 0) {
            playerSprite.flipX = false;
            playerAnim.SetBool("isMoving", true);
        } else if (playerRB.velocity.x < 0) {
            playerSprite.flipX = true;
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

        if (IsGrounded()) {
            playerAnim.SetBool("isGrounded", true);
        } else {
            playerAnim.SetBool("isGrounded", false);
        }

        if (IsAgainstWall()) {
            playerAnim.SetBool("isWallClimbing", true);
        } else {
            playerAnim.SetBool("isWallClimbing", false);
        }

        if (Input.GetKeyDown(fireKey) && CanFire()) {
            playerAnim.SetBool("isThrowing", true);
            Invoke("SetIsThrowingFalse", 0.5f);
        }

        if (Input.GetKeyDown(meleeKey) && CanFire()) {
            playerAnim.SetBool("isMeleeing", true);
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
    }

    private void SetIsMeleeingFalse() {
        playerAnim.SetBool("isMeleeing", false);
    }
    #endregion

    #region Public Functions
    public float getPlayerDir() {
        return lastDir;
    }
    #endregion
}
