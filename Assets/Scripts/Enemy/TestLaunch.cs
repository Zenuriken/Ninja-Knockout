using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestLaunch : MonoBehaviour
{
	// public Rigidbody2D ball;
	// public Transform target;

	// public float h = 25;
	// public float gravity = -18;

	// public bool debugPath;

	// void Start() {
	// 	// ball.useGravity = false;
	// 	//gravity = Physics2D.gravity * ball.gravityScale;
	// }

	// void Update() {
	// 	if (Input.GetKeyDown (KeyCode.Space)) {
	// 		Launch ();
	// 	}

	// 	if (debugPath) {
	// 		DrawPath ();
	// 	}
	// }

	// void Launch() {
	// 	//Physics2D.gravity = Vector2.up * gravity * ball.gravityScale;
	// 	ball.velocity = CalculateLaunchData ().initialVelocity;
	// }

	// LaunchData CalculateLaunchData() {
	// 	float displacementY = target.position.y - ball.position.y;
	// 	Vector2 displacementXZ = new Vector2 (target.position.x - ball.position.x, target.position.y - ball.position.y);
	// 	float time = Mathf.Sqrt(-2*h/gravity) + Mathf.Sqrt(2*(displacementY - h)/gravity);
	// 	Vector2 velocityY = Vector2.up * Mathf.Sqrt (-2 * gravity * h);
	// 	Vector2 velocityXZ = displacementXZ / time;

	// 	return new LaunchData(velocityXZ + velocityY * -Mathf.Sign(gravity), time);
	// }

	// void DrawPath() {
	// 	LaunchData launchData = CalculateLaunchData ();
	// 	Vector2 previousDrawPoint = ball.position;

	// 	int resolution = 30;
	// 	for (int i = 1; i <= resolution; i++) {
	// 		float simulationTime = i / (float)resolution * launchData.timeToTarget;
	// 		Vector2 displacement = launchData.initialVelocity * simulationTime + Vector2.up *gravity * simulationTime * simulationTime / 2f;
	// 		Vector2 drawPoint = ball.position + displacement;
	// 		Debug.DrawLine (previousDrawPoint, drawPoint, Color.green);
	// 		previousDrawPoint = drawPoint;
	// 	}
	// }

	// struct LaunchData {
	// 	public readonly Vector2 initialVelocity;
	// 	public readonly float timeToTarget;

	// 	public LaunchData (Vector2 initialVelocity, float timeToTarget)
	// 	{
	// 		this.initialVelocity = initialVelocity;
	// 		this.timeToTarget = timeToTarget;
	// 	}
		
	// }
	[SerializeField]
    Transform target;

	[SerializeField]
	Transform midPoint;
 
    [SerializeField]
    float initialAngle;

	[SerializeField]
	float force;

	private Rigidbody2D rigid;

	private float A;
	private float B;
	private float C;

	private void Start() {
		rigid = GetComponent<Rigidbody2D>();
		//CalculateInitialAngle();
		CalcParabolaEquation((Vector2)this.transform.position, (Vector2)midPoint.position, (Vector2)target.position);
	}

	private void Update() {
		DrawPath();
		if (Input.GetKeyDown(KeyCode.Space)) {
			Launch();
		}
	}

	private void DrawPath() {
		// int resolution = 30;
		// for (int i = 1; i <= resolution; i++) {
		// 	float simulationTime = i / (float)resolution * launchData.timeToTarget;
		// 	Vector2 displacement = launchData.initialVelocity * simulationTime + Vector2.up *gravity * simulationTime * simulationTime / 2f;
		// 	Vector2 drawPoint = ball.position + displacement;
		// 	Debug.DrawLine (previousDrawPoint, drawPoint, Color.green);
		// 	previousDrawPoint = drawPoint;
		// }

		Vector2 previousDrawPoint = (Vector2)this.transform.position;
		int resolution = 30;
		for (int i = 1; i <= resolution; i++) {
			
			float simulationTime = i / (float)resolution * launchData.timeToTarget;
			Vector2 displacement = launchData.initialVelocity * simulationTime + Vector2.up *gravity * simulationTime * simulationTime / 2f;
			Vector2 drawPoint = ball.position + displacement;
			Debug.DrawLine (previousDrawPoint, drawPoint, Color.green);
			previousDrawPoint = drawPoint;
		}
	}

	private void CalculateInitialAngle() {
		initialAngle = Vector2.Angle(Vector2.right, midPoint.position - this.transform.position);
		Debug.Log("Initial angle: " + initialAngle);
	}
 
    void Launch () {
        Vector2 p = new Vector2(target.position.x, target.position.y);
 
        float gravity = Physics2D.gravity.magnitude * rigid.gravityScale;
        // Selected angle in radians
        float angle = initialAngle * Mathf.Deg2Rad;
 
        // Positions of this object and the target on the same plane
        Vector2 planarTarget = new Vector2(p.x, p.y);
        Vector2 planarPostion = new Vector3(transform.position.x, transform.position.y);
 
        // Planar distance between objects
        float distance = Vector2.Distance(planarTarget, planarPostion);
        // Distance along the y axis between objects
        float yOffset = transform.position.y - p.y;
 
        float initialVelocity = (1 / Mathf.Cos(angle)) * Mathf.Sqrt((0.5f * gravity * Mathf.Pow(distance, 2)) / (distance * Mathf.Tan(angle) + yOffset));
 
        Vector2 velocity = new Vector2(initialVelocity * Mathf.Cos(angle), initialVelocity * Mathf.Sin(angle));
 
        // Rotate our velocity to match the direction between the two objects
        float angleBetweenObjects = Vector2.Angle(Vector2.right, planarTarget - planarPostion);
        Vector2 finalVelocity = Quaternion.AngleAxis(angleBetweenObjects, Vector2.right) * velocity;
        // Fire!
        rigid.velocity = finalVelocity * force;
 
        // Alternative way:
        //rigid.AddForce(finalVelocity * rigid.mass, ForceMode2D.Impulse);
    }

	// Assigns the coefficients of our parabola give three points.
	private void CalcParabolaEquation(Vector2 p1, Vector2 p2, Vector2 p3) {
		float x1 = p1.x;
		float x2 = p2.x;
		float x3 = p3.x;
		float y1 = p1.y;
		float y2 = p2.y;
		float y3 = p3.y;
		float denom = (x1-x2) * (x1-x3) * (x2-x3);
		A = (x3 * (y2-y1) + x2 * (y1-y3) + x1 * (y3-y2)) / denom;
		B = (x3*x3 * (y1-y2) + x2*x2 * (y3-y1) + x1*x1 * (y2-y3)) / denom;
		C = (x2 * x3 * (x2-x3) * y1+x3 * x1 * (x3-x1) * y2+x1 * x2 * (x1-x2) * y3) / denom;
	}

	// Returns the Y value along the parabola given an x-value.
	private float CalcPositionAlongParabola(float x) {
		return A * Mathf.Pow(x, 2) + B * x + C;
	}


}