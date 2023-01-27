using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyConfusedState : EnemyState
{
    private float confusedTime;
    
    public EnemyConfusedState(EnemyStateManager currContext, EnemyStateFactory stateFactory) 
    : base(currContext, stateFactory) {}

    public override void EnterState() {
        Debug.Log("CONFUSED");
        ctx.InvokeRepeating("CreateQuestionMark", 0f, 1f);
    }

    public override void UpdateState() {
        CheckSwitchStates();
        if (ctx.CurrentState != this) return;
        confusedTime += Time.deltaTime;
    }

    public override void ExitState() {
        ctx.CancelInvoke();
        confusedTime = 0f;
    }

    // Enemy should exit pursue state if player hides or is in attacking range.
    public override void CheckSwitchStates() {
        // if (ctx.PlayerIsHiding() && confusedTime >= 5f) {
        //     SwitchState(factory.Return());
        // } else if (!ctx.PlayerIsHiding()) {
        //     SwitchState(factory.Pursue());
        // }
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
