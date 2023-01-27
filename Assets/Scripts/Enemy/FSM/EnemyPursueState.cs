using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPursueState : EnemyState
{
    public EnemyPursueState(EnemyStateManager currContext, EnemyStateFactory stateFactory) 
    : base(currContext, stateFactory) {}

    public override void EnterState() {
        Debug.Log("PURSUING");
        ctx.IsAlerted = true;
        ctx.IsDetectingPlayer = false;
        ctx.InvokeRepeating("UpdatePath", 0f, 0.5f);
    }

    public override void UpdateState() {
        CheckSwitchStates();
        if (ctx.CurrentState != this) return;
        ctx.FollowPath(ctx.PursueSpeed);
    }

    public override void ExitState() {
        ctx.CancelInvoke();
    }

    // Enemy should exit pursue state if player hides or is in attacking range.
    public override void CheckSwitchStates() {
        if (ctx.PlayerIsHiding() && ctx.Unreachable) {
            SwitchState(factory.Confused());
        } else if (ctx.CanAttack()) {
            SwitchState(factory.Attack());
        }
    }

    public override void InitializeSubState() {
        
    }
}
