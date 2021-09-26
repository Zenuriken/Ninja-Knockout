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
    [Tooltip("The gravity the player experiences when wall climbing.")]
    private float wallClimbingGravity;

    [SerializeField]
    [Tooltip("The horizontal velocity applied to the player when jumping from a wall.")]
    private float xWallJumpVel;

    [SerializeField]
    [Tooltip("The vertical velocity applied to the player when jumping from a wall.")]
    private float yWallJumpVel;

    [SerializeField]
    [Tooltip("The duration of a single wall jump in the Vector(xWallJumpVel, yWallJumpVel) direction.")]
    private float wallJumpDur;

    // Private Wall Climbing Variables
    private bool isWallClimbing;
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

    // Shooting private variables
    private KeyCode fireKey = KeyCode.Space;
    private float lastFire;
    private float firePointDist;
    #endregion

    #region General Private Variables
    private Rigidbody2D playerRB;
    private BoxCollider2D boxCollider2D;
    private float xInput;
    private float lastDir;
    private float gravity;
    private float speed;
    #endregion

/**********************************************************************************/

    #region Initializing Functions
    // Start is called before the first frame update
    void Start()
    {
        playerRB = GetComponent<Rigidbody2D>();
        boxCollider2D = GetComponent<BoxCollider2D>();
        speed = moveSpeed;
        gravity = playerRB.gravityScale;
        firePointDist = 0.5f;
    }
    #endregion

    #region Update Functions
    // Update is called once per frame
    void Update()
    {
        Move();
        Shoot();
    }
    #endregion

    #region Movement Functions
    // Controls the player's movement.
    void Move() {
        xInput = Input.GetAxisRaw("Horizontal");
        Debug.Log("Last Direction: " + lastDir.ToString());
        if (xInput > 0) {
            lastDir = 1;
            SwitchAttackPoint();
        } else if (xInput < 0) {
            lastDir = -1;
            SwitchAttackPoint();
        }

        if (!isDashing) {
            playerRB.velocity = new Vector2(xInput * speed, playerRB.velocity.y);
        }

        // Tapping the jump button
        if (Input.GetKeyDown(jumpKey) && (IsGrounded() || jumpCounter > 0) && !isDashing && !isWallClimbing && !IsAgainstWall(lastDir)) {
            //Debug.Log("Regular Jump from ground.");
            isJumping = true;
            jumpDurTimer = jumpDur;
            playerRB.velocity = new Vector2(playerRB.velocity.x, jumpVel);
            jumpCounter -= 1;
        }

        // Holding the jump button
        if (Input.GetKey(jumpKey) && isJumping == true && !isDashing && !isWallClimbing && !IsAgainstWall(lastDir)) {
            if (jumpDurTimer > 0) {
                //Debug.Log("Regular jump in air.");
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
            StartCoroutine(Dash(lastDir));
        }

        // Sneaking
        if (Input.GetKeyDown(KeyCode.DownArrow) && IsGrounded()) {
            isSneaking = true;
            speed = sneakSpeed;
        }

        if (Input.GetKeyUp(KeyCode.DownArrow)) {
            speed = moveSpeed;
        }

        // Wall Climbing
        if (!isDashing && IsAgainstWall(lastDir)) {
            playerRB.gravityScale = wallClimbingGravity;
            if (Input.GetKeyDown(jumpKey)) {
                isWallClimbing = true;
                Invoke("SetWallClimbingFalse", wallJumpDur);
            }
        } else if (!isDashing) {
            playerRB.gravityScale = gravity;
        }

        if (isWallClimbing) {
            playerRB.velocity = new Vector2(-lastDir * xWallJumpVel, yWallJumpVel);
        }
    }

    private void SetWallClimbingFalse() {
        isWallClimbing = false;
    }

    // Controls the dashing process.
    IEnumerator Dash(float dir) {
        isDashing = true;
        dashCounter -= 1;
        playerRB.velocity = new Vector2(playerRB.velocity.x, 0f);
        playerRB.AddForce(new Vector2(dashDist * dir, 0f), ForceMode2D.Impulse);
        playerRB.gravityScale = 0;
        yield return new WaitForSeconds(dashDur);
        playerRB.gravityScale = gravity;
        isDashing = false;
    }

    // Determines if the player is standing on ground.
    private bool IsGrounded() {
        RaycastHit2D raycastHit2D = Physics2D.BoxCast(boxCollider2D.bounds.center, boxCollider2D.bounds.size, 0f, Vector2.down, 0.1f, platformLayerMask);
        bool onGround = raycastHit2D.collider != null;
        //Debug.Log("Touching ground");
        if (onGround) {
            jumpCounter = 2;
            dashCounter = 1;
        }
        return onGround;
    }


    // TODO: Make the wall jump the same consistency as a long jump
    // If the directions is still on the wall have a second jump in the opposite direction



    // Determines if the player is jumping against a wall.
    private bool IsAgainstWall(float dir) {
        RaycastHit2D raycastHit2D;
        string side;
        if (dir == 1) {
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
        if (Input.GetKey(fireKey) && CanFire()) {
            GameObject shuriken = Object.Instantiate(shurikenPrefab, firePoint.transform.position, Quaternion.identity);
            Rigidbody2D rb = shuriken.GetComponent<Rigidbody2D>();
            rb.velocity = new Vector2(lastDir * shurikenSpeed, 0);
            lastFire = Time.time;
        }
    }

    private bool CanFire() {
        return lastFire + fireRate <= Time.time;
    }

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

    #region Public Functions
    public float getPlayerDir() {
        return lastDir;
    }
    #endregion
}
