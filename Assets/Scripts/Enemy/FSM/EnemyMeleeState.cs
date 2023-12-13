using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMeleeState : EnemyState
{
    public EnemyMeleeState(EnemyStateManager currContext, EnemyStateFactory stateFactory) 
    : base(currContext, stateFactory) {}

    float meleeTimer;

    public override void EnterState() {
        ctx.IsMeleeing = true;
        ctx.LastAttack = Time.time;
        ctx.Sounds.Play("Meleeing");
        PlayerController.singleton.GetComponent<Health>().TakeDmg(ctx.Dmg, ctx.transform.position);
    }

    public override void UpdateState() {
        CheckSwitchStates();
        if (ctx.CurrentState != this) return;
        meleeTimer += Time.deltaTime;
        
    }

    public override void ExitState() {
        ctx.CancelInvoke();
        ctx.IsMeleeing = false;
    }

    // Enemy should exit pursue state if player hides or is in attacking range.
    public override void CheckSwitchStates() {
        if (ctx.HasDied) {
            SwitchState(factory.Death());
        } else if (meleeTimer >= 0.5f) {
            SwitchState(factory.Pursue());
        }
    }

    public override void InitializeSubState() {
        
    }


    #region Attack Functions
    #endregion
}
