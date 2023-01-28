using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDetectState : EnemyState
{
    public EnemyDetectState(EnemyStateManager currContext, EnemyStateFactory stateFactory)
    : base(currContext, stateFactory) {}

    public override void EnterState() {
        // Debug.Log("DETECT");
        ctx.StartCoroutine(PlayerDetected());
    }

    public override void UpdateState() {
        CheckSwitchStates();
        if (ctx.CurrentState != this) return;
    }

    public override void ExitState() {
        ctx.IsDetectingPlayer = false;
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

    // Sets the direction of the enemy to the player when alerted.
    private void FacePlayer() {
        float xDir = PlayerController.singleton.transform.position.x - ctx.transform.position.x;
        if (xDir >= 0) {
            ctx.transform.localScale = new Vector3(Mathf.Abs(ctx.transform.localScale.x), ctx.transform.localScale.y, ctx.transform.localScale.z);
            ctx.FOV.SetStartingAngle(15f);
        } else {
            ctx.transform.localScale = new Vector3(-1f * Mathf.Abs(ctx.transform.localScale.x), ctx.transform.localScale.y, ctx.transform.localScale.z);
            ctx.FOV.SetStartingAngle(200f);
        }
    }

     // Play's alerted sound and also turns line of sight to red.
    IEnumerator PlayerDetected() {
        FacePlayer();
        ctx.FOV.SetOrigin(ctx.transform.position);
        ctx.QuestionMarks.Clear();
        ctx.ExclamationMark.Play();
        ctx.Sounds.Play("Alerted");
        UIManager.singleton.PlayerDetected();
        yield return new WaitForSeconds(ctx.AlertedDelay);
        ctx.AlertedObj.SetActive(true);
        ctx.IsAlerted = true;
    }
}
