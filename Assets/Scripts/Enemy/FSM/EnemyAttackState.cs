using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttackState : EnemyState
{
    public EnemyAttackState(EnemyStateManager currContext, EnemyStateFactory stateFactory) : base(currContext, stateFactory)
    {
    }

    public override void EnterState() {
        
    }

    public override void UpdateState() {

    }

    public override void ExitState() {

    }

    // Enemy should exit Patrol if alerted.
    public override void CheckSwitchStates() {
        
    }

    public override void InitializeSubState() {
        
    }
}
