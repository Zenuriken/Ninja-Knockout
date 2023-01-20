using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestLaunch : MonoBehaviour
{
    [SerializeField]
    float _InitialVelocity;
    [SerializeField]
    float _Angle;
    [SerializeField]
    LineRenderer _Line;
    [SerializeField]
    float _Step;

    private float _HeightOffset = -1.55f;

    private void Update() {
        float angle = _Angle * Mathf.Deg2Rad;
        DrawPath(_InitialVelocity, angle, _Step);

        if (Input.GetKeyDown(KeyCode.Space)) {
            StopAllCoroutines();
            StartCoroutine(Coroutine_Movement(_InitialVelocity, angle));
        }
    }

    private void DrawPath(float v0, float angle, float step) {
        step = Mathf.Max(0.01f, step);
        float totalTime = 10;
        _Line.positionCount = (int)(totalTime / step) + 2;
        int count = 0;
        for (float i = 0; i < totalTime; i += step) {
            float x = v0 * i * Mathf.Cos(angle);
            float y = v0 * i * Mathf.Sin(angle) - (1f / 2f) * -Physics.gravity.y * Mathf.Pow(i, 2);
            _Line.SetPosition(count , new Vector3(x, y, 0));
            count++;
        }
        float xfinal = v0 * totalTime * Mathf.Cos(angle);
        float yfinal = v0 * totalTime * Mathf.Sin(angle) - (1f / 2f) * -Physics.gravity.y * Mathf.Pow(totalTime, 2);
        _Line.SetPosition(count, new Vector3(xfinal, yfinal, 0));
    }


	IEnumerator Coroutine_Movement(float v0, float angle) {
        float t = 0;
        while (t < 100) {
            float x = v0 * t * Mathf.Cos(angle);
            float y = v0 * t * Mathf.Sin(angle) - (1f / 2f) * -Physics.gravity.y * Mathf.Pow(t, 2);

            transform.position = new Vector3(x, y, 0);
            t += Time.deltaTime;
            yield return null;

        }
    }
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    // // [SerializeField]
    // // Transform target;

	// // [SerializeField]
	// // float verticalOffset;

	// // [SerializeField]
	// // float initialVelocity;
 
	// // private Vector2 startingPos;
	// // private Rigidbody2D rigid;
	// // //private float initialAngle = 85;


	// // private void Start() {
	// // 	rigid = GetComponent<Rigidbody2D>();
	// // 	startingPos = new Vector2(this.transform.position.x, this.transform.position.y + verticalOffset);
	// // }

	// // private void Update() {
	// // 	if (Input.GetKeyDown(KeyCode.Space)) {
	// // 		CalculateInitialAngle();
	// // 		//Launch();
	// // 	}
	// // }
 
    // // void Launch () {

	// // 	//initialAngle = ComputeInitialAngle();

    // //     Vector2 p = new Vector2(target.position.x, target.position.y);
    // //     float gravity = Physics2D.gravity.magnitude * rigid.gravityScale;
    // //     // Selected angle in radians
    // //     float angle = initialAngle * Mathf.Deg2Rad;
 
    // //     // Positions of this object and the target on the same plane
    // //     Vector2 planarTarget = new Vector2(p.x, p.y);
    // //     Vector2 planarPostion = new Vector2(transform.position.x, transform.position.y + verticalOffset);
 
    // //     // Planar distance between objects
    // //     float distance = Vector2.Distance(planarTarget, planarPostion);
    // //     // Distance along the y axis between objects
    // //     float yOffset = transform.position.y + verticalOffset - p.y;
 
    // //     float initialVelocity = (1 / Mathf.Cos(angle)) * Mathf.Sqrt((0.5f * gravity * Mathf.Pow(distance, 2)) / (distance * Mathf.Tan(angle) + yOffset));
 
    // //     Vector2 velocity = new Vector2(initialVelocity * Mathf.Cos(angle), initialVelocity * Mathf.Sin(angle));
 
    // //     // Rotate our velocity to match the direction between the two objects
    // //     float angleBetweenObjects = Vector2.Angle(Vector2.right, planarTarget - planarPostion);
    // //     Vector2 finalVelocity = Quaternion.AngleAxis(angleBetweenObjects, Vector2.right) * velocity;
    // //     // Fire!
    // //     rigid.velocity = finalVelocity * jumpMultiplier;
 
    // //     // Alternative way:
    // //     //rigid.AddForce(finalVelocity * rigid.mass, ForceMode2D.Impulse);
    // // }


	// private void CalculateInitialAngle() {
    //     float gravity = Physics2D.gravity.magnitude * rigid.gravityScale;

	// 	// Horizontal Distance
	// 	float x = Mathf.Abs(target.position.x - this.transform.position.x);
	// 	// Relative height of starting position to target.
	// 	float h = this.transform.position.y + verticalOffset - target.position.y;
	// 	// Initial velocity
	// 	float v0 = initialVelocity;

	// 	float phase = Mathf.Atan(x / h);
 
	// 	float numerator = gravity * Mathf.Pow(x, 2) / Mathf.Pow(v0, 2) - h;
	// 	float denominator = Mathf.Sqrt(Mathf.Pow(h, 2) + Mathf.Pow(x, 2));

	// 	float initialAngle = (Mathf.Acos(numerator / denominator) + phase) / 2;

	// 	Debug.Log("Calculated angle: " + initialAngle);


    //     // // Positions of this object and the target on the same plane
    //     // Vector2 planarTarget = new Vector2(p.x, p.y);
    //     // Vector2 planarPostion = new Vector2(transform.position.x, transform.position.y + verticalOffset);
 
    //     // // Planar distance between objects
    //     // float distance = Vector2.Distance(planarTarget, planarPostion);
    //     // // Distance along the y axis between objects
    //     // float yOffset = transform.position.y + verticalOffset - p.y;
 
    //     // float initialVelocity = (1 / Mathf.Cos(angle)) * Mathf.Sqrt((0.5f * gravity * Mathf.Pow(distance, 2)) / (distance * Mathf.Tan(angle) + yOffset));
 
    //     // Vector2 velocity = new Vector2(initialVelocity * Mathf.Cos(angle), initialVelocity * Mathf.Sin(angle));
 
    //     // // Rotate our velocity to match the direction between the two objects
    //     // float angleBetweenObjects = Vector2.Angle(Vector2.right, planarTarget - planarPostion);
    //     // Vector2 finalVelocity = Quaternion.AngleAxis(angleBetweenObjects, Vector2.right) * velocity;
    //     // // Fire!
    //     // rigid.velocity = finalVelocity * jumpMultiplier;
 
    //     // // Alternative way:
    //     // //rigid.AddForce(finalVelocity * rigid.mass, ForceMode2D.Impulse);
	// }


}