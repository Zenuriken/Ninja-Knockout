using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPatrolState : EnemyState
{
    Vector2 adjustedPos;
    private List<Vector2> patrolPath;
    private float lastIdle;
    
    // Constructor for this state.
    public EnemyPatrolState(EnemyStateManager currContext, EnemyStateFactory stateFactory)
    : base (currContext, stateFactory) {}
    
    public override void EnterState() {
        // Debug.Log("PATROLLING");
        adjustedPos = ctx.AstarScript.GetAdjustedPosition();
        patrolPath = ctx.AstarScript.CalculatePatrolPath(ctx.MaxNodeDist);
        ctx.AlertedObj.SetActive(false);
        InitializeDirection();
    }

    public override void UpdateState() {
        CheckSwitchStates();
        if (ctx.CurrentState != this) return;
        if (ctx.PatrolEnabled) Patrol();
        adjustedPos = ctx.AstarScript.GetAdjustedPosition();
        ctx.FOV.SetOrigin(ctx.transform.position);
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


    // The enemy's patrolling state
    private void Patrol() {
        if (patrolPath == null) {
            ctx.EnemyRB.velocity = new Vector2(0f, ctx.EnemyRB.velocity.y);
        } else if ((adjustedPos.x < patrolPath[1].x && ctx.StartingDir == 1) || 
                    (adjustedPos.x > patrolPath[0].x && ctx.StartingDir == -1)) {
            ctx.EnemyRB.velocity = new Vector2(ctx.StartingDir * ctx.PatrolSpeed, ctx.EnemyRB.velocity.y);
        } else if (CanIdle()) {
            ctx.StartCoroutine(Idle());
        }
    }

    // Controls the player's Idle state when reaching the end of a platform.
    IEnumerator Idle() {
        lastIdle = Time.time;
        ctx.EnemyRB.velocity = new Vector2(0f, 0f);
        yield return new WaitForSeconds(ctx.IdleDur);
        ctx.StartingDir *= -1;
    }

    // Returns whether the enemy can start idling at the end of a platform
    private bool CanIdle() {
        return (lastIdle + ctx.IdleDur * 2f) < Time.time;
    }

    // Sets the direction of the enemy to where it's moving.
    private void InitializeDirection() {
        if (ctx.StartingDir == 1) {
            ctx.transform.localScale = new Vector3(Mathf.Abs(ctx.transform.localScale.x), ctx.transform.localScale.y, ctx.transform.localScale.z);
            ctx.FOV.SetStartingAngle(15f);
        } else if (ctx.StartingDir == -1) {
            ctx.transform.localScale = new Vector3(-1f * Mathf.Abs(ctx.transform.localScale.x), ctx.transform.localScale.y, ctx.transform.localScale.z);
             ctx.FOV.SetStartingAngle(200f);
        }
    }
}
