using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDeathState : EnemyState
{
    public EnemyDeathState(EnemyStateManager currContext, EnemyStateFactory stateFactory) 
    : base(currContext, stateFactory) {}

    public override void EnterState() {
        // Debug.Log("DEAD:"  + Time.time);
        ctx.StartCoroutine(Death());
    }

    public override void UpdateState() {
        //CheckSwitchStates();
        if (ctx.CurrentState != this) return;
        
    }

    public override void ExitState() {
        
    }

    // Enemy should exit pursue state if player hides or is in attacking range.
    public override void CheckSwitchStates() {

    }

    public override void InitializeSubState() {
        
    }

    // Kills the enemy
    IEnumerator Death() {
        ctx.AlertedObj.SetActive(false);
        ctx.SetHighLight(false);
        if (ctx.PlayerIsInThrowingRange && ctx.IsAlerted) PlayerController.singleton.IncreaseAlertedNumBy(-1);
        GameObject.Destroy(ctx.FOV.gameObject);
        ctx.gameObject.layer = 12; // Set the gameObject to layer 12

        float value = Random.Range(0, 100);
        if (value < ctx.DropChance) GameObject.Instantiate(ctx.ShurikenDropPrefab, ctx.transform.position, Quaternion.identity);

        yield return new WaitForSeconds(ctx.BodySplatDelay);
        BodySplat();
        yield return ctx.StartCoroutine(FadeAway());

        //yield return new WaitForSeconds(ctx.DestroyDelay);
        PlayerController.singleton.RemoveEnemyFromList(ctx.EnemyCollider);
        GameObject.Destroy(ctx.gameObject);
    }

    private void BodySplat() {
        if (ctx.IsGrounded && Mathf.Abs(ctx.EnemyRB.velocity.x) < 0.05f && Mathf.Abs(ctx.EnemyRB.velocity.y) < 0.05f) {
            ctx.Sounds.Play("BodySplat");
        }
    }

    // Fades away the body of the enemy upon death.
    IEnumerator FadeAway() {
        yield return new WaitForSeconds(ctx.FadeAwayDelay);
        for (float alpha = 1f; alpha > 0f; alpha -= Time.deltaTime * ctx.FadeAwaySpeed) {
            ctx.EnemySprite.color = new Color(1f, 1f, 1f, alpha);
            yield return new WaitForEndOfFrame();
        }
    }
}
