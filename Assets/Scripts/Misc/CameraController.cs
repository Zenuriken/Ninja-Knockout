using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public static CameraController singleton;

    #region Serialized Variables
    [SerializeField]
    [Tooltip("The Camera's camera object.")]
    private Camera cam;
    [SerializeField]
    [Tooltip("The proportion of the camera's width size to move in the x dimension. (For increasing player's line of sight)")]
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
    [SerializeField]
    [Tooltip("Left bounds of the camera.")]
    private float leftXLimit;
    [SerializeField]
    [Tooltip("The right bounds of the camera.")]
    private float rightXLimit;
    [SerializeField]
    [Tooltip("Determines whether the camera will follow the player.")]
    private bool followEnabled;
    #endregion

    #region Private Variables
    private GameObject player;
    private PlayerController playerScript;
    private int playerDir;
    private float yVelocity;
    private float xVelocity;
    private float currSmoothTime;
    private float cameraHalfWidth;

    private bool titleScreenModeEnabled;
    #endregion

    #region Initializing Functions
    private void Awake() {
        if (singleton != null && singleton != this) { 
            Destroy(this.gameObject); 
        } 
        else { 
            singleton = this;
            DontDestroyOnLoad(this.gameObject);
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        cam = GetComponent<Camera>();
        player = GameObject.FindGameObjectWithTag("Player");
        playerScript = player.GetComponent<PlayerController>();
        playerDir = playerScript.GetPlayerDir();
        titleScreenModeEnabled = playerScript.GetTitleScreenModeStatus();

        currSmoothTime = smoothTime;

        float screenAspect = (float)Screen.width / (float)Screen.height;
        //float cameraHalfHeight = cam.orthographicSize;
        cameraHalfWidth = cam.orthographicSize * screenAspect;

        followEnabled = true;
    }

    // Draws the bounds for the thresholds of the camera
    // private void OnDrawGizmos() {
    //     Gizmos.color = Color.blue;
    //     Gizmos.DrawWireCube(this.transform.position, new Vector2(cam.orthographicSize * 2 * xThreshold, cam.orthographicSize * 2 * yThreshold));
    //     Gizmos.color = Color.red;
    //     Gizmos.DrawWireCube(this.transform.position, new Vector2(cam.orthographicSize * 2 * xProportion, cam.orthographicSize * 2));
    // }
    #endregion

    #region Update Functions
    // Update is called once per frame
    void LateUpdate() {
        if (followEnabled) {
            Vector3 pos = this.transform.position;
            Vector3 targetPos = player.transform.position;
            playerDir = playerScript.GetPlayerDir();
            SetSmoothTime(playerScript.IsSneaking());
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
            if (!titleScreenModeEnabled) {
                if ((targetPos.x > pos.x + cam.orthographicSize * xThreshold && pos.x < rightXLimit - cameraHalfWidth)
                    || (targetPos.x < pos.x - cam.orthographicSize * xThreshold && pos.x > leftXLimit + cameraHalfWidth)) {
                        //Debug.Log("Moving cam");
                        pos.x = Mathf.SmoothDamp(pos.x, targetPos.x, ref xVelocity, currSmoothTime);
                }
            } else {
                if ((targetPos.x > pos.x + cam.orthographicSize * xThreshold)
                    || (targetPos.x < pos.x - cam.orthographicSize * xThreshold)) {
                        //Debug.Log("Moving cam");
                        pos.x = Mathf.SmoothDamp(pos.x, targetPos.x, ref xVelocity, currSmoothTime);
                }
            }
            this.transform.position = pos;
        }
    }
    #endregion

    public void SetSmoothTime(bool isSneaking) {
        if (isSneaking) {
            currSmoothTime = smoothTime * 2;
        } else {
            currSmoothTime = smoothTime;
        }
    }

    public void SetFollowEnabled(bool state) {
        followEnabled = state;
    }

    public IEnumerator SwitchTime() {
        yield return new WaitForEndOfFrame();
    }
}
