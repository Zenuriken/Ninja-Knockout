using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPursueState : EnemyState
{
    public EnemyPursueState(EnemyStateManager currContext, EnemyStateFactory stateFactory) 
    : base(currContext, stateFactory) {}

    public override void EnterState() {
        Debug.Log("PURSUE");
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
