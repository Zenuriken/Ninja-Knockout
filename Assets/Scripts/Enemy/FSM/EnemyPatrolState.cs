using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPatrolState : EnemyState
{
    private List<Vector2> patrolPath;
    private float fovSpeed = 32f;
    private float lastIdle;
    private float initialAngle;
    private float dir;
    private float anglePauseDur;
    private bool shouldWait;
    
    // Constructor for this state.
    public EnemyPatrolState(EnemyStateManager currContext, EnemyStateFactory stateFactory)
    : base (currContext, stateFactory) {}
    
    public override void EnterState() {
        // Debug.Log("PATROLLING");
        patrolPath = ctx.AstarScript.CalculatePatrolPath(ctx.MaxNodeDist);
        ctx.AlertedObj.SetActive(false);
        dir = ctx.StartingDir;
        InitializeDirection();
        ctx.InvokeRepeating("UpdatePath", 0f, 0.5f);
    }

    public override void UpdateState() {
        CheckSwitchStates();
        if (ctx.CurrentState != this) return;
        ctx.SetAdjustedPos();
        if (ctx.PatrolEnabled) ctx.FollowPath(ctx.PatrolSpeed);
        ctx.FOV.SetOrigin(ctx.transform.position);

        // If we reached the bounds of the FOV
        if (ReachedFOVBounds() && !shouldWait) {
            anglePauseDur += Time.deltaTime;
        } else {
            ctx.FOV.StartingAngle = ctx.FOV.StartingAngle + dir * fovSpeed * Time.deltaTime;
        }
        
        if (anglePauseDur >= 1f) {
            dir *= -1f;
            anglePauseDur = 0f;
        }



        if (CanIdle()) ctx.StartCoroutine(Idle());
    }
    
    public override void ExitState() {
    }

    // Enemy should exit Patrol if alerted.
    public override void CheckSwitchStates() {
        if (ctx.HasDied) {
            SwitchState(factory.Death());
        } else if (ctx.IsDetectingPlayer) {
            SwitchState(factory.Detect());
        }
    }

    public override void InitializeSubState() {
        
    }

    // Controls the player's Idle state when reaching the end of a platform.
    IEnumerator Idle() {
        //Debug.Log("IDLING");
        lastIdle = Time.time;
        ctx.EnemyRB.velocity = new Vector2(0f, 0f);
        yield return new WaitForSeconds(ctx.IdleDur);
        ctx.StartingDir *= -1;
        InitializeDirection();
    }

    // Returns whether the enemy can start idling at the end of a platform
    private bool CanIdle() {
        return ctx.Unreachable && (lastIdle + ctx.IdleDur * 2f) < Time.time;
    }

    // Sets the direction of the enemy to where it's moving.
    private void InitializeDirection() {
        if (ctx.StartingDir == 1) {
            ctx.transform.localScale = new Vector3(Mathf.Abs(ctx.transform.localScale.x), ctx.transform.localScale.y, ctx.transform.localScale.z);
            ctx.FOV.StartingAngle = 15f;
            ctx.TargetPos = patrolPath[1];
        } else if (ctx.StartingDir == -1) {
            ctx.transform.localScale = new Vector3(-1f * Mathf.Abs(ctx.transform.localScale.x), ctx.transform.localScale.y, ctx.transform.localScale.z);
            ctx.FOV.StartingAngle = 200f;
            ctx.TargetPos = patrolPath[0];
        }
        initialAngle = ctx.FOV.StartingAngle;
    }

    private bool ReachedFOVBounds() {
        return ((ctx.StartingDir == -1f && (ctx.FOV.StartingAngle >= initialAngle + ctx.DownFOVOffset || ctx.FOV.StartingAngle <= initialAngle - ctx.UpFOVOffset)) ||
                (ctx.StartingDir == 1f && (ctx.FOV.StartingAngle >= initialAngle + ctx.UpFOVOffset || ctx.FOV.StartingAngle <= initialAngle - ctx.DownFOVOffset)));
    }
}
