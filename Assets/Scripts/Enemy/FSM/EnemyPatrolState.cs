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
        adjustedPos = ctx.AstarScript.GetAdjustedPosition();
        patrolPath = ctx.AstarScript.CalculatePatrolPath(ctx.MaxNodeDist);
        ctx.AlertedObj.SetActive(false);
    }

    public override void UpdateState() {
        CheckSwitchStates();
        if (ctx.CurrentState != this) return;
        Patrol();
    }
    
    public override void ExitState() {
        ctx.IsDetectingPlayer = false;
    }

    // Enemy should exit Patrol if alerted.
    public override void CheckSwitchStates() {
        if (ctx.IsDetectingPlayer) SwitchState(factory.Detect());
    }

    public override void InitializeSubState() {
        
    }


    // The enemy's patrolling state
    private void Patrol() {
        adjustedPos = ctx.AstarScript.GetAdjustedPosition();
        ctx.FOV.SetOrigin(ctx.transform.position);
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
}
