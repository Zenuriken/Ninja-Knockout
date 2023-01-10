using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestLaunch : MonoBehaviour
{
	[SerializeField]
    Transform target;

	[SerializeField]
	Transform midPoint;

	[SerializeField]
	float verticalOffset;
 
	private Vector2 startingPos;
	private Rigidbody2D rigid;
	float initialAngle;


	// private float A;
	// private float B;
	// private float C;

	private void Start() {
		rigid = GetComponent<Rigidbody2D>();
		startingPos = new Vector2(this.transform.position.x, this.transform.position.y + verticalOffset);
		initialAngle = Vector2.Angle(Vector2.right, (Vector2)midPoint.position - startingPos);
		Debug.Log("Initial angle: " + initialAngle);
	}

	private void Update() {
		if (Input.GetKeyDown(KeyCode.Space)) {
			Launch();
		}
	}
 
    void Launch () {
        Vector2 p = new Vector2(target.position.x, target.position.y);
        float gravity = Physics2D.gravity.magnitude * rigid.gravityScale;
        // Selected angle in radians
        float angle = initialAngle * Mathf.Deg2Rad;
 
        // Positions of this object and the target on the same plane
        Vector2 planarTarget = new Vector2(p.x, p.y);
        Vector2 planarPostion = new Vector3(transform.position.x, transform.position.y + verticalOffset);
 
        // Planar distance between objects
        float distance = Vector2.Distance(planarTarget, planarPostion);
        // Distance along the y axis between objects
        float yOffset = transform.position.y + verticalOffset - p.y;
 
        float initialVelocity = (1 / Mathf.Cos(angle)) * Mathf.Sqrt((0.5f * gravity * Mathf.Pow(distance, 2)) / (distance * Mathf.Tan(angle) + yOffset));
 
        Vector2 velocity = new Vector2(initialVelocity * Mathf.Cos(angle), initialVelocity * Mathf.Sin(angle));
 
        // Rotate our velocity to match the direction between the two objects
        float angleBetweenObjects = Vector2.Angle(Vector2.right, planarTarget - planarPostion);
        Vector2 finalVelocity = Quaternion.AngleAxis(angleBetweenObjects, Vector2.right) * velocity;
        // Fire!
        rigid.velocity = finalVelocity;
 
        // Alternative way:
        //rigid.AddForce(finalVelocity * rigid.mass, ForceMode2D.Impulse);
    }

	// // Assigns the coefficients of our parabola give three points.
	// private void CalcParabolaEquation(Vector2 p1, Vector2 p2, Vector2 p3) {
	// 	float x1 = p1.x;
	// 	float x2 = p2.x;
	// 	float x3 = p3.x;
	// 	float y1 = p1.y;
	// 	float y2 = p2.y;
	// 	float y3 = p3.y;
	// 	float denom = (x1-x2) * (x1-x3) * (x2-x3);
	// 	A = (x3 * (y2-y1) + x2 * (y1-y3) + x1 * (y3-y2)) / denom;
	// 	B = (x3*x3 * (y1-y2) + x2*x2 * (y3-y1) + x1*x1 * (y2-y3)) / denom;
	// 	C = (x2 * x3 * (x2-x3) * y1+x3 * x1 * (x3-x1) * y2+x1 * x2 * (x1-x2) * y3) / denom;
	// }

	// // Returns the Y value along the parabola given an x-value.
	// private float CalcPositionAlongParabola(float x) {
	// 	return A * Mathf.Pow(x, 2) + B * x + C;
	// }


}