using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyConfusedState : EnemyState
{
    private float confusedTime;
    
    public EnemyConfusedState(EnemyStateManager currContext, EnemyStateFactory stateFactory) 
    : base(currContext, stateFactory) {}

    public override void EnterState() {
        // Debug.Log("CONFUSED");
        ctx.InvokeRepeating("CreateQuestionMark", 0f, 1f);
    }

    public override void UpdateState() {
        CheckSwitchStates();
        if (ctx.CurrentState != this) return;
        ctx.SetDirection();
        confusedTime += Time.deltaTime;
    }

    public override void ExitState() {
        ctx.CancelInvoke();
        confusedTime = 0f;
    }

    // Enemy should exit pursue state if player hides or is in attacking range.
    public override void CheckSwitchStates() {
        if (ctx.HasDied) {
            SwitchState(factory.Death());
        } else if (ctx.LostPlayer() && confusedTime >= 5f) {
            SwitchState(factory.Return());
        } else if (!ctx.LostPlayer()) {
            SwitchState(factory.Pursue());
        }
    }

    public override void InitializeSubState() {
        
    }
}
