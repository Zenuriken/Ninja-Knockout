using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public static CameraController singleton;

    #region Serialized Variables
    [SerializeField][Tooltip("The Camera's camera object.")]
    private Camera cam;
    [SerializeField][Tooltip("Whether we are on the title screen.")]
    private bool titleScreenModeEnabled;
    [SerializeField][Tooltip("The proportion of the camera's width size to move in the x dimension. (For increasing player's line of sight)")]
    private float xProportion;
    [SerializeField][Tooltip("The Camera's horiztonal threshold for movement.")]
    private float xThreshold;
    [SerializeField][Tooltip("The Camera's vertical threshold for movement.")]
    private float yThreshold;
    [SerializeField][Tooltip("The time it takes for the camera to move to the player's last recorded position.")]
    private float smoothTime;
    [SerializeField][Tooltip("Left bounds of the camera.")]
    private float leftXLimit;
    [SerializeField][Tooltip("The right bounds of the camera.")]
    private float rightXLimit;
    [SerializeField][Tooltip("Determines whether the camera will follow the player.")]
    private bool followEnabled;
    [SerializeField][Tooltip("How fast the moon and sun will rotate")]
    private float switchSpeed;
    [SerializeField][Tooltip("The moon.")]
    private GameObject moon;
    [SerializeField][Tooltip("The sun.")]
    private GameObject sun;
    #endregion

    #region Private Variables
    private GameObject player;
    private PlayerController playerScript;
    private int playerDir;
    private float xVelocity;
    private float currSmoothTime;
    private float cameraHalfWidth;
    #endregion

    #region Initializing Functions
    // Start is called before the first frame update
    void Start()
    {
        cam = GetComponent<Camera>();
        player = GameObject.FindGameObjectWithTag("Player");
        if (!titleScreenModeEnabled) playerScript = player.GetComponent<PlayerController>();
        currSmoothTime = smoothTime;
        float screenAspect = (float)Screen.width / (float)Screen.height;
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
        if (titleScreenModeEnabled) {
            transform.position += Vector3.right * 10f * Time.deltaTime;
            return;
        }

        if (followEnabled && player != null) {
            Vector3 pos = this.transform.position;
            Vector3 targetPos = player.transform.position;
            playerDir = playerScript.GetPlayerDir();
            SetSmoothTime(playerScript.IsSneaking());
            float xOffset = cam.orthographicSize * 2 * xProportion;
            float xPos = playerDir == 1 ? targetPos.x + xOffset : targetPos.x - xOffset;
            targetPos = new Vector3(xPos, targetPos.y, targetPos.z);
          
            if ((targetPos.x > pos.x + cam.orthographicSize * xThreshold && pos.x < rightXLimit - cameraHalfWidth)
                || (targetPos.x < pos.x - cam.orthographicSize * xThreshold && pos.x > leftXLimit + cameraHalfWidth)) {
                    pos.x = Mathf.SmoothDamp(pos.x, targetPos.x, ref xVelocity, currSmoothTime);
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

    // public IEnumerator SwitchTime() {
    //     // for (float t = 0; t <= 180f; t += Time.deltaTime * rotationSpeed) {
    //     //     rotationObject.transform.Rotate(0f, 0f, Time.deltaTime * rotationSpeed);
    //     //     yield return new WaitForEndOfFrame();
    //     // }
    //     for (float t = 32f; t >= 0f; t -= Time.deltaTime * switchSpeed) {
    //         moon.transform.localPosition = new Vector3(0, t, 5);
    //         sun.transform.localPosition = new Vector3(0, -t * 2, 5);
    //         yield return new WaitForEndOfFrame();
    //     }
    // }
}
