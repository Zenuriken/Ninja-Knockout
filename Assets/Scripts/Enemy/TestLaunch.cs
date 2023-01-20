using System.Collections;
using UnityEngine;

public class TestLaunch : MonoBehaviour
{
    // [SerializeField] float _InitialVelocity;
    // [SerializeField] float _Angle;
    // [SerializeField] float _Height;
    [SerializeField] LineRenderer _Line;
    [SerializeField] float _Step;
    [SerializeField] float _JumpSpeedFactor;

    private Vector3 _InitialPos;
    private Camera _cam;
    private float _HeightOffset = -1.55f;

    private bool isJumping;
    private Rigidbody2D enemyRB;

    private void Start() {
        _InitialPos = new Vector3(transform.position.x, transform.position.y + _HeightOffset, 0);
        _cam = Camera.main;
        enemyRB = this.GetComponent<Rigidbody2D>();
    }

    private void Update() {
        Vector3 targetPos = _cam.ScreenToWorldPoint(Input.mousePosition) - _InitialPos;
        targetPos.z = 0;
        float height = targetPos.y + targetPos.magnitude / 10f;
        height = Mathf.Max(0.01f, height);
        float angle;
        float v0;
        float time;
        CalculatePathWithHeight(targetPos, height, out v0, out angle, out time);
        if (Input.GetKeyDown(KeyCode.Space) && !isJumping) {
            isJumping = true;
            Vector2 dir = GetVectorFromAngle(angle * Mathf.Rad2Deg);
            enemyRB.AddForce(dir * v0, ForceMode2D.Impulse);
            // StopAllCoroutines();
            // StartCoroutine(Coroutine_Movement(v0, angle, time));
        }
    }

    // Draws the path of a jump with the specified number of segments (steps).
    private void DrawPath(float v0, float angle, float time, float step) {
        step = Mathf.Max(0.01f, step);
        _Line.positionCount = (int)(time / step) + 2;
        int count = 0;
        for (float i = 0; i < time; i += step) {
            float x = v0 * i * Mathf.Cos(angle);
            float y = v0 * i * Mathf.Sin(angle) - (1f / 2f) * -Physics.gravity.y * Mathf.Pow(i, 2);
            _Line.SetPosition(count , _InitialPos + new Vector3(x, y, 0));
            count++;
        }
        float xfinal = v0 * time * Mathf.Cos(angle);
        float yfinal = v0 * time * Mathf.Sin(angle) - (1f / 2f) * -Physics.gravity.y * Mathf.Pow(time, 2);
        _Line.SetPosition(count, _InitialPos + new Vector3(xfinal, yfinal, 0));
    }

    // Calculates the path without regards to height.
    // private void CalculatePath(Vector3 targetPos, float angle, out float v0, out float time) {
    //     float xt = targetPos.x;
    //     float yt = targetPos.y;
    //     float g = -Physics.gravity.y;

    //     float v1 = Mathf.Pow(xt, 2) * g;
    //     float v2 = 2 * xt * Mathf.Sin(angle) * Mathf.Cos(angle);
    //     float v3 = 2 * yt * Mathf.Pow(Mathf.Cos(angle), 2);
    //     v0 = Mathf.Sqrt(v1 / (v2 - v3));
    //     time = xt / (v0 * Mathf.Cos(angle));
    // }

    // Returns the quadratic formula.
    private float QuadraticEquation(float a, float b, float c, float sign) {
        return (-b + sign * Mathf.Sqrt(b * b -4 * a * c)) / (2 * a);
    }

    // Calculates and assigns the initial velocity and anlge of a jump path.
    private void CalculatePathWithHeight(Vector3 targetPos, float h, out float v0, out float angle, out float time) {
        float xt = targetPos.x;
        float yt = targetPos.y;
        float g = -Physics.gravity.y * enemyRB.gravityScale;
        float b = Mathf.Sqrt(2 * g * h);
        float a = (-0.5f * g);
        float c = -yt;
        float tplus = QuadraticEquation(a, b, c, 1);
        float tmin = QuadraticEquation(a, b, c, -1);
        time = tplus > tmin ? tplus : tmin;
        angle = Mathf.Atan(b * time / xt);
        v0 = b / Mathf.Sin(angle);

        float g_draw = -Physics.gravity.y;
        float a_draw = (-0.5f * g_draw);
        float b_draw = Mathf.Sqrt(2 * g_draw * h);
        float c_draw = -yt;
        float tplus_draw = QuadraticEquation(a_draw, b_draw, c_draw, 1);
        float tmin_draw = QuadraticEquation(a_draw, b_draw, c_draw, -1);
        float time_draw = tplus_draw > tmin_draw ? tplus_draw : tmin_draw;
        float angle_draw = Mathf.Atan(b_draw * time_draw / xt);
        float v0_draw = b_draw / Mathf.Sin(angle_draw);
        DrawPath(v0_draw, angle_draw, time_draw, _Step);
    }

    // Controls the physics of moving the Enemy along a parabola when jumping.
	IEnumerator Coroutine_Movement(float v0, float angle, float time) {
        float t = 0;
        enemyRB.gravityScale = 0;
        while (t < time) {
            float x = v0 * t * Mathf.Cos(angle);
            float y = v0 * t * Mathf.Sin(angle) - (1f / 2f) * -Physics.gravity.y * Mathf.Pow(t, 2);
            Vector3 newPos = _InitialPos + new Vector3(x, y - _HeightOffset, 0);
            
            Vector2 dir = newPos - transform.position;
            
            
            
            
            transform.position = _InitialPos + new Vector3(x, y - _HeightOffset, 0);
            t += Time.deltaTime * _JumpSpeedFactor;
            yield return null;
        }
    }

    // Returns a vector pointing in the direction of the given angle (in degrees).
    public Vector2 GetVectorFromAngle(float angle) {
        // angle = 0 -> 360
        float angleRad = angle * (Mathf.PI / 180f);
        return new Vector2(Mathf.Cos(angleRad), Mathf.Sin(angleRad));
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