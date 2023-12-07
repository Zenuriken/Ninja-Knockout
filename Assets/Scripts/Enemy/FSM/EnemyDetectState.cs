using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDetectState : EnemyState
{
    public EnemyDetectState(EnemyStateManager currContext, EnemyStateFactory stateFactory)
    : base(currContext, stateFactory) {}

    public override void EnterState() {
        // Debug.Log("DETECT: " + Time.time);
        ctx.StartCoroutine(PlayerDetected());
    }

    public override void UpdateState() {
        CheckSwitchStates();
        if (ctx.CurrentState != this) return;
        ctx.FOV.SetOrigin(ctx.transform.position);
    }

    public override void ExitState() {
        ctx.IsDetectingPlayer = false;
        ctx.StopAllCoroutines();
    }

    public override void CheckSwitchStates() {
        if (ctx.HasDied) {
            SwitchState(factory.Death());
        } else if (ctx.IsAlerted) {
            SwitchState(factory.Pursue());
        }
    }

    public override void InitializeSubState() {
        
    }

     // Play's alerted sound and also turns line of sight to red.
    IEnumerator PlayerDetected() {
        ctx.FacePlayer();
        ctx.FOV.SetOrigin(ctx.transform.position);
        ctx.QuestionMarks.Clear();
        ctx.ExclamationMark.Play();
        ctx.Sounds.Play("Alerted");
        GameManager.singleton.PlayerDetected();
        yield return new WaitForSeconds(ctx.AlertedDelay);
        ctx.AlertedObj.SetActive(true);
        ctx.IsAlerted = true;
    }
}
