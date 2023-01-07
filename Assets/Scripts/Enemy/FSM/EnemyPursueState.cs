using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPursueState : EnemyState
{
    public EnemyPursueState(EnemyStateManager currContext, EnemyStateFactory stateFactory) 
    : base(currContext, stateFactory) {}

    public override void EnterState() {
    }

    public override void UpdateState() {
        CheckSwitchStates();
        if (ctx.CurrentState != this) return;
        ctx.UpdatePursuePath();
        ctx.FollowPath(ctx.PursueSpeed);
    }

    public override void ExitState() {
        
    }

    // Enemy should exit Patrol if alerted.
    public override void CheckSwitchStates() {
        if (!ctx.IsAlerted) SwitchState(factory.Patrol());
    }

    public override void InitializeSubState() {
        
    }
}
