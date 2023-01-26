using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPursueState : EnemyState
{
    public EnemyPursueState(EnemyStateManager currContext, EnemyStateFactory stateFactory) 
    : base(currContext, stateFactory) {}

    public override void EnterState() {
        ctx.InvokeRepeating("UpdatePath", 0f, 0.5f);
    }

    public override void UpdateState() {
        CheckSwitchStates();
        if (ctx.CurrentState != this) return;
        ctx.FollowPath(ctx.PursueSpeed);
    }

    public override void ExitState() {
        Debug.Log("CANCELLED");
        ctx.CancelInvoke();
    }

    // Enemy should exit pursue state if player hides or is in attacking range.
    public override void CheckSwitchStates() {
        if (PlayerIsHiding()) {
            SwitchState(factory.Return());
        } else if (CanAttack()) {
            SwitchState(factory.Attack());
        }
    }

    public override void InitializeSubState() {
        
    }


    private bool PlayerIsHiding() {
        return ctx.Unreachable && PlayerController.singleton.IsHiding() && Mathf.Abs(ctx.EnemyRB.velocity.x) < 0.05f;
    }

    private bool CanAttack() {
        return false;
    }

    // if (!hasDied && playerScript.IsHiding() && Mathf.Abs(enemyRB.velocity.x) < 0.05f && !isJumping && isAlerted) {
    //         CreateQuestionMark();
    //         if (!isReturningToPatrolPos) {
    //             StartCoroutine("ReturnToPatrols");
    //         }
    //     }

}
