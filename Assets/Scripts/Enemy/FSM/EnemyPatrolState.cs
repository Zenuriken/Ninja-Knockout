using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPatrolState : EnemyState
{
    public enum fovState { PAUSED, ROTATING } 
    private fovState currState;
    private List<Vector2> patrolPath;
    private float fovSpeed = 64f;
    private float lastIdle;
    private float initialAngle;
    private float rotateDir;
    private float anglePauseDur;
    private float holdTime;
    private bool justPaused;
    
    // Constructor for this state.
    public EnemyPatrolState(EnemyStateManager currContext, EnemyStateFactory stateFactory)
    : base (currContext, stateFactory) {}
    
    public override void EnterState() {
        // Debug.Log("PATROLLING");
        ctx.AlertedObj.SetActive(false);
        if (ctx.PatrolEnabled) {
            patrolPath = ctx.AstarScript.CalculatePatrolPath(ctx.MaxNodeDist);
            ctx.InvokeRepeating("UpdatePath", 0f, 0.5f);
        }
        if (ctx.FOVOscillateEnabled) {
            rotateDir = ctx.StartingDir * -1f;
            currState = fovState.ROTATING;
        }
        InitializeDirection();
    }

    public override void UpdateState() {
        CheckSwitchStates();
        if (ctx.CurrentState != this) return;
        ctx.SetAdjustedPos();
        ctx.FOV.SetOrigin(ctx.transform.position);
        if (ctx.PatrolEnabled) ctx.FollowPath(ctx.PatrolSpeed);
        if (ctx.PatrolEnabled && CanIdle()) ctx.StartCoroutine(Idle());
        if (ctx.FOVOscillateEnabled) OscillateFOV();
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
            if (ctx.PatrolEnabled) ctx.TargetPos = patrolPath[1];
        } else if (ctx.StartingDir == -1) {
            ctx.transform.localScale = new Vector3(-1f * Mathf.Abs(ctx.transform.localScale.x), ctx.transform.localScale.y, ctx.transform.localScale.z);
            ctx.FOV.StartingAngle = 200f;
            if (ctx.PatrolEnabled) ctx.TargetPos = patrolPath[0];
        }
        initialAngle = ctx.FOV.StartingAngle;
    }

    // Returns whether the FOV has reached the offset bounds set in the inspector.
    private bool ReachedFOVBounds() {
        return ((ctx.StartingDir == -1f && (ctx.FOV.StartingAngle >= initialAngle + ctx.DownFOVOffset || ctx.FOV.StartingAngle <= initialAngle - ctx.UpFOVOffset)) ||
                (ctx.StartingDir == 1f && (ctx.FOV.StartingAngle >= initialAngle + ctx.UpFOVOffset || ctx.FOV.StartingAngle <= initialAngle - ctx.DownFOVOffset)));
    }

    // Controls the logic for oscillating the FOV.
    private void OscillateFOV() {
        // Actions for each state.
        if (currState == fovState.ROTATING) {
            ctx.FOV.StartingAngle = ctx.FOV.StartingAngle + rotateDir * fovSpeed * Time.deltaTime;
        } else if (currState == fovState.PAUSED) {
            holdTime += Time.deltaTime;
        }
        // Conditions for switching state.
        if (currState == fovState.ROTATING && ReachedFOVBounds() && !justPaused) {
            currState = fovState.PAUSED;
            justPaused = true;
        } else if (currState == fovState.PAUSED && holdTime >= 2f) {
            holdTime = 0f;
            currState = fovState.ROTATING;
            rotateDir *= -1f;
        }
        // Handles case where we want to start rotating but are still at FOV bounds.
        justPaused = currState == fovState.PAUSED || ReachedFOVBounds();
    }
}
