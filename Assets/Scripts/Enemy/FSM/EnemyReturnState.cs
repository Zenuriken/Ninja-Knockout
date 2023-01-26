using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyReturnState : EnemyState
{
    public EnemyReturnState(EnemyStateManager currContext, EnemyStateFactory stateFactory) 
    : base(currContext, stateFactory) {}

    public override void EnterState() {
        ctx.InvokeRepeating("UpdatePath", 0f, 0.5f);
    }

    public override void UpdateState() {
        CheckSwitchStates();
        if (ctx.CurrentState != this) return;
        ctx.FollowPath(ctx.PatrolSpeed);
    }

    public override void ExitState() {
        
    }

    // Enemy should exit pursue state if player hides or is in attacking range.
    public override void CheckSwitchStates() {
        // if (!ctx.IsAlerted) {
        //     SwitchState(factory.Patrol());
        // }
    }

    public override void InitializeSubState() {
        
    }


    // if (!hasDied && playerScript.IsHiding() && Mathf.Abs(enemyRB.velocity.x) < 0.05f && !isJumping && isAlerted) {
    //         CreateQuestionMark();
    //         if (!isReturningToPatrolPos) {
    //             StartCoroutine("ReturnToPatrols");
    //         }
    //     }

    // IEnumerator ReturnToPatrols() {
    //     yield return new WaitForSeconds(5f);
    //     if (playerScript.IsHiding() && Mathf.Abs(enemyRB.velocity.x) < 0.05f && isAlerted) {
    //         SetAlertStatus(false);
    //         astarScript.SetReturnToPatrolPos(true);
    //     } else {
    //         isReturningToPatrolPos = false;
    //     }
    // }

}
