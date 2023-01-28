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
        } else if (CanMelee()) {
            SwitchState(factory.Melee());
        } else if (CanThrow()) {
             SwitchState(factory.Throw());
        }
    }

    public override void InitializeSubState() {
        
    }

    private bool IsWithinVectorBounds() {
        Vector2 dir = (PlayerController.singleton.transform.position - ctx.FirePointTrans.position).normalized;
        float dot = (dir.x >= 0) ? Vector2.Dot(dir, Vector2.right) : Vector2.Dot(dir, Vector2.left);
        return Mathf.Abs(dot) >= 0.25f; // 0.5 is the dot product value for 60 degrees
    }

    private bool CanMelee() {
        bool playerIsInMeleeRange = ctx.MeleeEnemy.IsTouchingMeleeTrigger();
        return (ctx.LastAttack + ctx.AttackRate <= Time.time) && !ctx.IsStunned && playerIsInMeleeRange && ctx.IsGrounded;
    }

    private bool CanThrow() {
        bool playerIsInThrowingRange = ctx.AlertedSight.IsTouchingAlertedTrigger() && IsWithinVectorBounds();
        return (ctx.LastAttack + ctx.AttackRate <= Time.time) && !ctx.IsStunned && playerIsInThrowingRange && ctx.IsGrounded && ctx.Unreachable;
    }
}
