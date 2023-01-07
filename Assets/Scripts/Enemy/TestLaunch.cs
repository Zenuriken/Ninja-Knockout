using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestLaunch : MonoBehaviour
{
    public Rigidbody2D ball;
	public Transform target;

	public float h = 25;
	public float gravity = -18;

	public bool debugPath;

	void Start() {
		ball.gravityScale = 0;
	}
    
    void Update() {
		if (Input.GetKeyDown (KeyCode.Space)) {
			Launch ();
		}

		if (debugPath) {
			DrawPath ();
		}
	}

	void Launch() {
		Physics.gravity = Vector2.up * gravity;
		// ball.gravityScale = true;
		ball.velocity = CalculateLaunchData ().initialVelocity;
	}

	LaunchData CalculateLaunchData() {
		float displacementY = target.position.y - ball.position.y;
		Vector2 displacementXZ = new Vector2 (target.position.x - ball.position.x, target.position.y - ball.position.y);
		float time = Mathf.Sqrt(-2*h/gravity) + Mathf.Sqrt(2*(displacementY - h)/gravity);
		Vector2 velocityY = Vector2.up * Mathf.Sqrt (-2 * gravity * h);
		Vector2 velocityXZ = displacementXZ / time;

		return new LaunchData(velocityXZ + velocityY * -Mathf.Sign(gravity), time);
	}

	void DrawPath() {
		LaunchData launchData = CalculateLaunchData ();
		Vector2 previousDrawPoint = ball.position;

		int resolution = 30;
		for (int i = 1; i <= resolution; i++) {
			float simulationTime = i / (float)resolution * launchData.timeToTarget;
			Vector2 displacement = launchData.initialVelocity * simulationTime + Vector2.up *gravity * simulationTime * simulationTime / 2f;
			Vector2 drawPoint = ball.position + displacement;
			Debug.DrawLine (previousDrawPoint, drawPoint, Color.green);
			previousDrawPoint = drawPoint;
		}
	}

    struct LaunchData {
        public readonly Vector2 initialVelocity;
        public readonly float timeToTarget;

        public LaunchData (Vector2 initialVelocity, float timeToTarget) {
            this.initialVelocity = initialVelocity;
            this.timeToTarget = timeToTarget;
        }
    }
}