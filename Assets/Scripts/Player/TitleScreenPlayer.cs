using UnityEngine;

public class TitleScreenPlayer : MonoBehaviour {

    #region Movement Variables
    [Header("Movement")]
    [SerializeField][Tooltip("The moveSpeed of the player.")]
    private float moveSpeed;
    [SerializeField][Tooltip("The max speed the player can fall.")]
    private float maxFallSpeed;
    #endregion

    #region Particle Variables
    [Header("Particles")]
    [SerializeField][Tooltip("Dust particle effect when on the ground.")]
    private ParticleSystem groundDust;
    #endregion
   
    private bool isGrounded;
    // General private variables
    private LayerMask allPlatformsLayerMask;
    private Rigidbody2D playerRB;
    private Collider2D boxCollider2D;
    private float speed;
    // Private Animator Private Variables
    private Animator playerAnim;

    #region Initializing Functions
    // Awake is called before Start
    private void Awake() {
        // Getting Player Components
        playerRB = GetComponent<Rigidbody2D>();
        boxCollider2D = GetComponent<Collider2D>();
        playerAnim = GetComponent<Animator>();
    }

    // Start is called before the first frame update
    void Start() {
        speed = moveSpeed;
        allPlatformsLayerMask = LayerMask.GetMask("Platform", "OneWayPlatform");
    }
    #endregion

    #region Update Functions
    // Update is called once per frame
    void Update() {
        // Limits the velocity when falling
        playerRB.velocity = new Vector2(playerRB.velocity.x, Mathf.Clamp(playerRB.velocity.y, -maxFallSpeed, maxFallSpeed));
        // If the player is on the title screen.
        IsGrounded();
        playerRB.velocity = new Vector2(speed, playerRB.velocity.y);
        if (isGrounded) CreateDust(0);
        UpdateSprite();
    }
    #endregion

    #region Movement Functions

    // Creates dust around the player.
    void CreateDust(int s) {
        // Create ground dust
        if (s == 0) groundDust.Play();
    }
    #endregion

    #region State Functions
    // Determines if the player is standing on ground.
    private void IsGrounded() {
        bool lastGroundStatus = isGrounded;
        RaycastHit2D raycastHit2D = Physics2D.BoxCast(boxCollider2D.bounds.center, new Vector2(0.6f, boxCollider2D.bounds.size.y - 0.1f), 0f, Vector2.down, 0.2f, allPlatformsLayerMask);
        isGrounded = raycastHit2D.collider != null;

        // If the player has landed on the grounded.
        if (lastGroundStatus == false && isGrounded == true && raycastHit2D.collider.tag == "Platform") {
            CreateDust(0);
        } else if (isGrounded && raycastHit2D.collider.tag == "Platform" && Mathf.Abs(playerRB.velocity.x) > 0.05f) {
            CreateDust(0);
        }
        return;
    }
    #endregion

    #region Sprite Rendering Functions
    // Updates the player's sprites based on input/state.
    private void UpdateSprite() {
        playerAnim.SetBool("isMoving", Mathf.Abs(playerRB.velocity.x) > 0.05f);
        playerAnim.SetBool("isJumping", playerRB.velocity.y > 0.05);
        playerAnim.SetBool("isFalling", playerRB.velocity.y < -0.05);
        playerAnim.SetBool("isGrounded", isGrounded);
    }
    #endregion
}