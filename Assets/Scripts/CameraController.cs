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
    [Tooltip("The Camera's horiztonal follow speed.")]
    private float speedX;
    [SerializeField]
    [Tooltip("The Camera's vertical follow speed.")]
    private float speedY;
    [SerializeField]
    [Tooltip("The Camera's horiztonal threshold for movement.")]
    private float xThreshold;
    [SerializeField]
    [Tooltip("The Camera's vertical threshold for movement.")]
    private float yThreshold;
    #endregion

    #region Private Variables
    private GameObject player;
    #endregion

    #region Initializing Functions
    // Start is called before the first frame update
    void Start()
    {
        cam = GetComponent<Camera>();
        player = GameObject.FindGameObjectWithTag("Player");
    }

    // Draws the bounds for the thresholds of the camera
    private void OnDrawGizmos() {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(this.transform.position, new Vector2(cam.orthographicSize * 2 * xThreshold, cam.orthographicSize * 2 * yThreshold ));
    }
    #endregion

    #region Update Functions
    // Update is called once per frame
    void LateUpdate()
    {
        float interpolationX = speedX * Time.deltaTime;
        float interpolationY = speedY * Time.deltaTime;
        Vector3 pos = this.transform.position;
        Vector3 playerPos = player.transform.position;

        if (playerPos.y > pos.y + cam.orthographicSize * yThreshold
            || playerPos.y < pos.y - cam.orthographicSize * yThreshold) {
                pos.y = Mathf.Lerp(pos.y, playerPos.y, interpolationY);
        }
        if (playerPos.x > pos.x + cam.orthographicSize * xThreshold
            || playerPos.x < pos.x - cam.orthographicSize * xThreshold) {
                pos.x = Mathf.Lerp(pos.x, playerPos.x, interpolationX);
        }

        this.transform.position = pos;
    }
    #endregion
}
