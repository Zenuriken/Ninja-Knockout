using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldOfView : MonoBehaviour
{
    // Private variables.
    private PolygonCollider2D polyCol;
    private EnemyStateManager enemyScript;
    private PlayerController playerScript;
    private LayerMask platformsLayerMask;
    private GradientColorKey[] colorKeys;
    private MeshRenderer meshRenderer;
    private Mesh mesh;
    private Vector3 origin;
    private Vector3 lastOrigin;
    private float lastDetectTimer;
    private float currDetectTimer;
    private bool seesPlayer;

    // Public variables.
    public Vector3 originOffset;
    public float startingAngle;
    public float fov;
    public int rayCount;
    public float viewDistance;
    public float detectionTime;

    [SerializeField]
    [Tooltip("The gradient of the FOV.")]
    private Gradient gradient;
    
    // Start is called before the first frame update
    void Start()
    {
        platformsLayerMask = LayerMask.GetMask("Platform");
        mesh = new Mesh();
        this.GetComponent<MeshFilter>().mesh = mesh;
        meshRenderer = this.GetComponent<MeshRenderer>();
        playerScript = GameObject.Find("Player").GetComponent<PlayerController>();
        polyCol = this.GetComponent<PolygonCollider2D>();
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (enemyScript == null) return;
        // If the enemy is not allerted and hasn't died, update the line of sight.
        if (!enemyScript.IsAlerted && !enemyScript.HasDied) {
            if (!lastOrigin.Equals(origin)) {
                UpdateFOVShape();
            }
            UpdateFOVColor();

            // If the detection time is met, set the enemy to alerted.
            if (currDetectTimer >= detectionTime && !playerScript.IsHiding()) {
                seesPlayer = true;
                SetColorKeys(1f);
                enemyScript.IsDetectingPlayer = true;
            }
        // When the player is detected, reset detecting variables.
        } else {
            mesh.Clear();
            currDetectTimer = 0f;
            SetColorKeys(0f);
            seesPlayer = false;
            polyCol.enabled = false;
        }

        lastOrigin = origin;
        //lastDetectTimer = currDetectTimer;
    }

    #region Private Functions
    // Updates the shape of the FOV.
    private void UpdateFOVShape() {
        // Initialize values.
        float angle = startingAngle;
        float angleIncrease = fov / rayCount;
        Vector3[] vertices_3D = new Vector3[rayCount + 1 + 1];
        Vector2[] vertices_2D = new Vector2[vertices_3D.Length];
        Vector2[] uv = new Vector2[vertices_3D.Length];
        int[] triangles = new int[rayCount * 3];
        vertices_3D[0] = origin;
        vertices_2D[0] = (Vector2)origin;
        int vertexIndex = 1;
        int triangleIndex = 0;

        // Assign vertex positions based on collision.
        for (int i = 0; i <= rayCount; i++) {
            Vector3 vertex;
            RaycastHit2D raycastHit2D = Physics2D.Raycast(origin, GetVectorFromAngle(angle), viewDistance, platformsLayerMask);
            if (raycastHit2D.collider == null) {
                // No hit
                vertex = origin + GetVectorFromAngle(angle) * viewDistance;
            } else {
                // Hit object
                vertex = raycastHit2D.point;
            }
            // Assign vertices.
            vertices_3D[vertexIndex] = vertex;
            vertices_2D[vertexIndex] = (Vector2)vertex;
            if (i > 0) {
                triangles[triangleIndex + 0] = 0;
                triangles[triangleIndex + 1] = vertexIndex - 1;
                triangles[triangleIndex + 2] = vertexIndex;
                triangleIndex += 3;
            }
            vertexIndex++;
            angle -= angleIncrease;
        }
        
        // Update mesh components.
        mesh.vertices = vertices_3D;
        mesh.uv = uv;
        mesh.triangles = triangles;
        mesh.bounds = new Bounds(origin, Vector3.one * 1000f);

        // Assign vertices of Polygon Collider.
        if (!polyCol.enabled) {
            polyCol.enabled = true;
        }

        // Update Polygon collider vertices if the enemy has moved.
        polyCol.SetPath(0, vertices_2D);
    }

    // Sets the color of the FOV.
    private void UpdateFOVColor() {
        // Calculate the gradient color based on current detection time.
        if (!seesPlayer) {
            float prop = currDetectTimer / detectionTime;
            SetColorKeys(prop); 
        }

        // Assign the color gradient of the LineOfSight.
        var colors = new Color[mesh.uv.Length];
        // Instead if vertex.y we use uv.x
        for (var i = 0; i < mesh.uv.Length; i++) {
            float distance = Vector2.Distance((Vector2) mesh.vertices[i], (Vector2) mesh.vertices[0]);
            float proportion = distance / viewDistance;
            colors[i] = gradient.Evaluate(proportion);
        }
        mesh.colors = colors; 
    }

    // Sets the gradient to a color from Yellow -> Red based on proportion.
    private void SetColorKeys(float prop) {
        colorKeys = gradient.colorKeys;
        colorKeys[0].color = new Color(1f, 1f - prop, 0f, 0.2941f);
        colorKeys[1].color = new Color(1f, 1f - prop, 0f, 0f);
        gradient.colorKeys = colorKeys;
    }

    // Returns a directional vector based on the given angle.
    private Vector3 GetVectorFromAngle(float angle) {
        // angle = 0 -> 360
        float angleRad = angle * (Mathf.PI/180f);
        return new Vector3(Mathf.Cos(angleRad), Mathf.Sin(angleRad));
    }

    // Returns an angle in degrees based on the given direction.
    private float GetAngleFromVectorFloat(Vector3 dir) {
        // angle = 0 -> 360
        dir = dir.normalized;
        float n = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        if (n < 0) n += 360;

        return n;
    }

    // Creates question mark above enemy's head when seeing player.
    private void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.tag == "Player" && !playerScript.IsHiding() && !seesPlayer && !enemyScript.IsAlerted) {
            enemyScript.CreateQuestionMark();
        }
    }

    // Increase the detection timer if in contact with the player.
    private void OnTriggerStay2D(Collider2D other) {
        if (other.gameObject.tag == "Player" && !playerScript.IsHiding()) {
            float distProp = Vector2.Distance((Vector2)playerScript.transform.position, (Vector2)origin) / viewDistance;
            currDetectTimer += (1f / distProp) * Time.deltaTime;
        }
    }

    // Reset the detection timer
    private void OnTriggerExit2D(Collider2D other) {
        if (other.gameObject.tag == "Player") {
            currDetectTimer = 0;
        }
    }
    #endregion

    #region Public Functions
    // Sets the origin where the mesh begins.
    public void SetOrigin(Vector3 origin) {
        this.origin = origin + originOffset;
    }

    // Sets the starting angle based on direction of the enemy.
    public void SetStartingAngle(float angle) {
        this.startingAngle = angle;
    }

    // Sets the enemyScript reference for this FOV game object.
    public void InitializeEnemyScript(EnemyStateManager enemy) {
        this.enemyScript = enemy; 
    }
    #endregion
}
