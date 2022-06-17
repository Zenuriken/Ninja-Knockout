using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class EnemyAI : MonoBehaviour
{
    [Header("Pathfinding")]
    public Transform target;
    public float activateDistance = 50f;
    public float pathUpdateSeconds = 0.5f;

    [Header("Physics")]
    public float speed = 200f;
    public float nextWaypointDistance = 3f;
    public float jumpNodeHeightRequirement = 0.8f;
    public float jumpNodeWidthRequirement = 0.2f;
    public float jumpModifier = 0.3f;
    public float jumpCheckOffset = 0.1f;

    [Header("Customer Behavior")]
    public bool followEnabled = true;
    public bool jumpEnabled = true;
    public bool directionLookEnabled = true;

    private Rigidbody2D targetRb;
    private EnemyController enemyControllerScript;
    private Path path;
    private int currentWaypoint = 0;
    private bool isGrounded = false;
    Seeker seeker;
    Rigidbody2D rb;

    private BoxCollider2D boxCollider2D;
    public LayerMask platformLayerMask;

    // Start is called before the first frame update
    private void Start() {
       seeker = GetComponent<Seeker>();
       rb = GetComponent<Rigidbody2D>();
       boxCollider2D = GetComponent<BoxCollider2D>();
       targetRb = target.gameObject.GetComponent<Rigidbody2D>();
       enemyControllerScript = this.GetComponent<EnemyController>();

       InvokeRepeating("UpdatePath", 0f, pathUpdateSeconds);
    }

    // Update is called once per frame// Determines if the player is standing on ground.
    // private void IsGrounded() {
    //     bool groundStatus = isGrounded;
    //     RaycastHit2D raycastHit2D = Physics2D.BoxCast(boxCollider2D.bounds.center, new Vector2(0.6f, boxCollider2D.bounds.size.y - 0.1f), 0f, Vector2.down, 0.2f, allPlatformsLayerMask);
    //     bool onGround = raycastHit2D.collider != null;
    //     if (onGround) {
    //         jumpCounter = 1;
    //         dashCounter = 1;
    //     }
    //     isGrounded = onGround;
    //     if (groundStatus == false && isGrounded == true) {
    //         CreateDust(0);
    //     }
    //     return;
    // }
    private void Update() {
        if (TargetInDistance() && followEnabled) {
            PathFollow();
        }
    }

    // Called every pathUpdateSeconds. Updates the current path
    private void UpdatePath() {
        if (followEnabled && enemyControllerScript.IsAlerted() && TargetInDistance() && seeker.IsDone()) {
            seeker.StartPath(rb.position, target.position, OnPathComplete);
        }
    }

    private void PathFollow() {   
        if (path == null) {
            return;
        }

        // Reached end of path.
        if (currentWaypoint >= path.vectorPath.Count) {
            Debug.Log("Reached end of path");
            return;
        }


        // See if colliding with anything
        // Determines if the player is standing on ground.
        // isGrounded = Physics2D.Raycast(transform.position, -Vector3.up, GetComponent<Collider2D>().bounds.extents.y + jumpCheckOffset);
        // Debug.Log("Enemy is grounded");
        // Direction Calculation
        Vector2 direction = ((Vector2) path.vectorPath[currentWaypoint] - rb.position).normalized;
        Debug.Log("DirectionX: " + direction.x + "   DirectionY: " + direction.y);
        Vector2 force = direction * speed * Time.deltaTime;
        
        // Jump
        if (jumpEnabled && IsGrounded()) {
            if (target.position.y - 1f > rb.transform.position.y && targetRb.velocity.y == 0 && path.vectorPath.Count < 20) {
                rb.AddForce(Vector2.up * jumpModifier);
                //rb.MovePosition(new Vector3(rb.position.x, target.position.y + 5f));
            }
        }

        // Movement
        //rb.AddForce(force);
        if (enemyControllerScript.IsAlerted()) {
            if (direction.x > 0) {
                rb.velocity = new Vector2(speed, rb.velocity.y);
            } else {
                rb.velocity = new Vector2(-speed, rb.velocity.y);
            }
        }

        // Next Waypoint
        float distance = Vector2.Distance(rb.position, path.vectorPath[currentWaypoint]);
        if (distance < nextWaypointDistance) {
            currentWaypoint++;
        }

        // Direction Graphics Handling
        if (directionLookEnabled) {
            if (rb.velocity.x > 0.05f) {
                transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
            } else if (rb.velocity.x < -0.05f) {
                transform.localScale = new Vector3(-1f * Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
            }
        }
    }
    
    // Returns if target is within activate distance
    private bool TargetInDistance() {
        return Vector3.Distance(transform.position, target.transform.position) < activateDistance;
    }

    // Called when the path is completed. Updates the current path and resets the path index to 0.
    private void OnPathComplete(Path p) {
        if (!p.error) {
            path = p;
            currentWaypoint = 0;
        }
    }

    private bool IsGrounded() {
        RaycastHit2D raycastHit2D = Physics2D.BoxCast(boxCollider2D.bounds.center, boxCollider2D.bounds.size, 0f, Vector2.down, 0.1f, platformLayerMask);
        bool onGround = raycastHit2D.collider != null;
        return onGround;
    }
}
