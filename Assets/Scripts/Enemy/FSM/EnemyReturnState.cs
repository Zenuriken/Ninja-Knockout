using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyReturnState : EnemyState
{
    public EnemyReturnState(EnemyStateManager currContext, EnemyStateFactory stateFactory) 
    : base(currContext, stateFactory) {}

    public override void EnterState() {
        Debug.Log("RETURNING");
        ctx.IsAlerted = false;
        ctx.AlertedObj.SetActive(false);
        ctx.InvokeRepeating("UpdatePath", 0f, 0.5f);
    }

    public override void UpdateState() {
        CheckSwitchStates();
        if (ctx.CurrentState != this) return;
        ctx.FOV.SetOrigin(ctx.transform.position);
        ctx.FollowPath(ctx.PatrolSpeed);
    }

    public override void ExitState() {
        ctx.CancelInvoke();
    }

    // Enemy should exit pursue state if player hides or is in attacking range.
    public override void CheckSwitchStates() {
        if (ctx.IsDetectingPlayer) {
            SwitchState(factory.Detect());
        } else if (ctx.AstarScript.HasReturned(ctx.SpawnPos)) {
            SwitchState(factory.Patrol());
        }
    }

    public override void InitializeSubState() {
        
    }

    // IEnumerator ReturnToPatrols() {
    //     ctx.InvokeRepeating("CreateQuestionMark", 0f, 1f);
    //     yield return new WaitForSeconds(5f);
    //     if (ctx.PlayerIsHiding()) {
    //         ctx.IsAlerted = false;
    //         ctx.AlertedObj.SetActive(false);
    //         ctx.CancelInvoke();
    //         ctx.InvokeRepeating("UpdatePath", 0f, 0.5f);
    //     }
    // }

}
