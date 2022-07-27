using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldOfView : MonoBehaviour
{
    
    private EnemyController enemyScript;
    private PlayerController playerScript;
    private Transform playerTrans;
    private LayerMask allPlatformsLayerMask;
    private LayerMask playerAndPlatformLayerMask;
    private Mesh mesh;
    private Vector3 origin;
    public Vector3 originOffset;
    public float startingAngle;
    private Vector3 aimDirection;
    public float fov;
    public int rayCount;
    public float viewDistance;
    public Material detectedMat;
    public Material undetectedMat;
    private MeshRenderer meshRenderer;
    
    // Start is called before the first frame update
    void Start()
    {
        allPlatformsLayerMask = LayerMask.GetMask("Platform", "OneWayPlatform");
        playerAndPlatformLayerMask = LayerMask.GetMask("Player", "Platform", "OneWayPlatform");
        mesh = new Mesh();
        this.GetComponent<MeshFilter>().mesh = mesh;
        meshRenderer = this.GetComponent<MeshRenderer>();
        playerTrans = GameObject.Find("Player").transform;
        playerScript = playerTrans.GetComponent<PlayerController>();
    }

    // Update is called once per frame
    void LateUpdate()
    {
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
                
                RaycastHit2D raycastHit2D = Physics2D.Raycast(origin, GetVectorFromAngle(angle), viewDistance, allPlatformsLayerMask);
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
            mesh.bounds = new Bounds(origin, Vector3. one * 1000f);

            Vector3 dirOfPlayer = playerTrans.position - origin;
            float endingAngle = startingAngle - fov;
            if (endingAngle < 0) {
                endingAngle += 360;
            }
            float playerAngle = GetAngleFromVectorFloat(dirOfPlayer);
            if ((startingAngle == 15f && playerAngle <= startingAngle + 360f && playerAngle >= endingAngle) ||
                (startingAngle == 200f && playerAngle <= startingAngle && playerAngle >= endingAngle)) {
                RaycastHit2D playerRaycastHit2D = Physics2D.Raycast(origin, GetVectorFromAngle(playerAngle), viewDistance, playerAndPlatformLayerMask);
                if (playerRaycastHit2D.collider != null && playerRaycastHit2D.collider.name == "Player" && !playerScript.IsHiding()) {
                    meshRenderer.material = detectedMat;
                    enemyScript.SetAlertStatus(true); 
                }
            }
        } else {
            mesh.Clear();
            meshRenderer.material = undetectedMat;
        }
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
}
