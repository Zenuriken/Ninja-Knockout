using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    #region Serialized Variables
    [SerializeField]
    [Tooltip("The Camera's camera object.")]
    private Camera cam;

    [SerializeField]
    [Tooltip("The proportion of the camera to move in the x dimension.")]
    private float xProportion;

    [SerializeField]
    [Tooltip("The Camera's horiztonal threshold for movement.")]
    private float xThreshold;

    [SerializeField]
    [Tooltip("The Camera's vertical threshold for movement.")]
    private float yThreshold;

    [SerializeField]
    [Tooltip("The time it takes for the camera to move to the player's last recorded position.")]
    private float smoothTime;
    #endregion

    #region Private Variables
    private GameObject player;
    private PlayerController playerScript;
    private int playerDir;
    private float yVelocity;
    private float xVelocity;
    #endregion

    #region Initializing Functions
    // Start is called before the first frame update
    void Start()
    {
        cam = GetComponent<Camera>();
        player = GameObject.FindGameObjectWithTag("Player");
        playerScript = player.GetComponent<PlayerController>();
        playerDir = playerScript.GetPlayerDir();
    }

    // Draws the bounds for the thresholds of the camera
    private void OnDrawGizmos() {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(this.transform.position, new Vector2(cam.orthographicSize * 2 * xThreshold, cam.orthographicSize * 2 * yThreshold));
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(this.transform.position, new Vector2(cam.orthographicSize * 2 * xProportion, cam.orthographicSize * 2));
    }
    #endregion

    #region Update Functions
    // Update is called once per frame
    void LateUpdate() {
        Vector3 pos = this.transform.position;
        Vector3 targetPos = player.transform.position;
        playerDir = playerScript.GetPlayerDir();
        float xOffset = cam.orthographicSize * 2 * xProportion;
        if (playerDir == 1) {
            targetPos = new Vector3(targetPos.x + xOffset, targetPos.y, targetPos.z);
        } else {
            targetPos = new Vector3(targetPos.x - xOffset, targetPos.y, targetPos.z);
        }

        // if (playerPos.y > pos.y + cam.orthographicSize * yThreshold
        //     || playerPos.y < pos.y - cam.orthographicSize * yThreshold) {
        //         pos.y = Mathf.SmoothDamp(pos.y, playerPos.y, ref yVelocity, smoothTime);
        // }
        
        if (targetPos.x > pos.x + cam.orthographicSize * xThreshold
            || targetPos.x < pos.x - cam.orthographicSize * xThreshold) {
                pos.x = Mathf.SmoothDamp(pos.x, targetPos.x, ref xVelocity, smoothTime);
        }
        this.transform.position = pos;
    }
    #endregion
}
