using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldOfView : MonoBehaviour
{
    
    private EnemyController enemyScript;
    private PlayerController playerScript;
    private Transform playerTrans;
    private LayerMask platformsLayerMask;
    private LayerMask playerAndPlatformLayerMask;
    private Mesh mesh;
    private Vector3 origin;
    public Vector3 originOffset;
    public float startingAngle;
    private Vector3 aimDirection;
    public float fov;
    public int rayCount;
    public float viewDistance;
    // public Material detectedMat;
    // public Material undetectedMat;
    private MeshRenderer meshRenderer;

    [SerializeField]
    [Tooltip("The gradient of the FOV.")]
    private Gradient gradient;

    private GradientColorKey[] colorKeys;

    public float detectionTime;

    private float currDetectTimer;

    private bool seesPlayer;

    
    // Start is called before the first frame update
    void Start()
    {
        platformsLayerMask = LayerMask.GetMask("Platform");
        playerAndPlatformLayerMask = LayerMask.GetMask("Player", "Platform");
        mesh = new Mesh();
        this.GetComponent<MeshFilter>().mesh = mesh;
        meshRenderer = this.GetComponent<MeshRenderer>();
        playerTrans = GameObject.Find("Player").transform;
        playerScript = playerTrans.GetComponent<PlayerController>();
    }

    // Update is called once per frame
    void LateUpdate()
    {
        // If the enemy is not allerted and hasn't died, update the line of sight.
        if (!enemyScript.IsAlerted() && !enemyScript.HasDied()) {
            float angle = startingAngle;
            float angleIncrease = fov / rayCount;

            Vector3[] vertices = new Vector3[rayCount + 1 + 1];
            Vector2[] uv = new Vector2[vertices.Length];
            int[] triangles = new int[rayCount * 3];

            vertices[0] = origin;

            int vertexIndex = 1;
            int triangleIndex = 0;
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

                vertices[vertexIndex] = vertex;

                if (i > 0) {
                    triangles[triangleIndex + 0] = 0;
                    triangles[triangleIndex + 1] = vertexIndex - 1;
                    triangles[triangleIndex + 2] = vertexIndex;

                    triangleIndex += 3;
                }

                vertexIndex++;
                angle -= angleIncrease;
            }

            mesh.vertices = vertices;
            mesh.uv = uv;
            mesh.triangles = triangles;
            mesh.bounds = new Bounds(origin, Vector3.one * 1000f);

            var colors = new Color[uv.Length];
     
            // Instead if vertex.y we use uv.x
            //colors[0] = gradient.colorKeys[0].color;
            for (var i = 0; i < uv.Length; i++) {
                float distance = GetDistance((Vector2) mesh.vertices[i], (Vector2) mesh.vertices[0]);
                float proportion = distance / viewDistance;
                colors[i] = gradient.Evaluate(proportion);
            }
            mesh.colors = colors;            

            Vector3 dirOfPlayer = Vector3.zero;
            if (playerTrans != null) {
                dirOfPlayer = playerTrans.position - origin;
            }
            float endingAngle = startingAngle - fov;
            if (endingAngle < 0) {
                endingAngle += 360;
            }
            float playerAngle = GetAngleFromVectorFloat(dirOfPlayer);

            // If the player is in viewing angle.
            if ((startingAngle == 15f && playerAngle <= startingAngle + 360f && playerAngle >= endingAngle) ||
                (startingAngle == 200f && playerAngle <= startingAngle && playerAngle >= endingAngle)) {
                RaycastHit2D playerRaycastHit2D = Physics2D.Raycast(origin, GetVectorFromAngle(playerAngle), viewDistance, playerAndPlatformLayerMask);
                if (playerRaycastHit2D.collider != null && playerRaycastHit2D.collider.name == "Player" && !playerScript.IsHiding()) {
                    if (currDetectTimer >= detectionTime) {
                        enemyScript.SetAlertStatus(true);
                    } else {
                        float distProp = Vector2.Distance((Vector2)playerScript.transform.position, (Vector2)origin) / viewDistance;
                        currDetectTimer += (1f / distProp) * Time.deltaTime;
                    }
                } else {
                    currDetectTimer = 0f;
                }
            } else {
                currDetectTimer = 0f;
            }

            if (!seesPlayer) {
                float prop = currDetectTimer / detectionTime;
                SetColorKeys(prop);
            }

        // When the player is detected.
        } else {
            mesh.Clear();
            currDetectTimer = 0f;
            SetColorKeys(0f);
            seesPlayer = false;
        }
    }


    private float GetDistance(Vector2 p0, Vector2 p1) {
        return Mathf.Sqrt(Mathf.Pow(p0.x - p1.x, 2f) + Mathf.Pow(p0.y - p1.y, 2f));
    }


    // Sets the gradient to a color from Yellow -> Red based on proportion.
    private void SetColorKeys(float prop) {
        colorKeys = gradient.colorKeys;
        colorKeys[0].color = new Color(1f, 1f - prop, 0f, 0.2941f);
        colorKeys[1].color = new Color(1f, 1f - prop, 0f, 0f);
        gradient.colorKeys = colorKeys;
    }

    public Vector3 GetVectorFromAngle(float angle) {
        // angle = 0 -> 360
        float angleRad = angle * (Mathf.PI/180f);
        return new Vector3(Mathf.Cos(angleRad), Mathf.Sin(angleRad));
    }

    public float GetAngleFromVectorFloat(Vector3 dir) {
        // angle = 0 -> 360
        dir = dir.normalized;
        float n = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        if (n < 0) n += 360;

        return n;
    }

    public void SetOrigin(Vector3 origin) {
        this.origin = origin + originOffset;
    }

    public void SetStartingAngle(float angle) {
        this.startingAngle = angle;
    }

    public void SetAimDirection(Vector3 aimDirection) {
        startingAngle = GetAngleFromVectorFloat(aimDirection) - fov / 2f;
    }

    public void InitializeEnemyScript(EnemyController enemy) {
        this.enemyScript = enemy; 
    }

    public void SetMeshRendererToAlertGrad() {
        seesPlayer = true;
        SetColorKeys(1f);
    }
}
