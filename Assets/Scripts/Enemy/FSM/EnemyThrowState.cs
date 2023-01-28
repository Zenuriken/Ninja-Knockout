using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyThrowState : EnemyState
{
    public EnemyThrowState(EnemyStateManager currContext, EnemyStateFactory stateFactory) 
    : base(currContext, stateFactory) {}

    float throwTimer;

    public override void EnterState() {
        // Debug.Log("THROWING");
        ctx.IsThrowing = true;
        ctx.LastAttack = Time.time;
        
        Vector2 dir = (PlayerController.singleton.transform.position - ctx.FirePointTrans.position).normalized;
        RaycastHit2D raycastHit2D = Physics2D.CircleCast(ctx.FirePointTrans.position, 0.335f, dir, 15f, ctx.PlayerAndPlatformLayerMask, 0f, 0f);
        Debug.DrawLine(ctx.FirePointTrans.position, (Vector3)raycastHit2D.point);
        if (raycastHit2D.collider != null && raycastHit2D.collider.tag == "Player") ctx.StartCoroutine(Throw(dir));
    }

    public override void UpdateState() {
        CheckSwitchStates();
        if (ctx.CurrentState != this) return;
        throwTimer += Time.deltaTime;
        
    }

    public override void ExitState() {
        ctx.CancelInvoke();
        ctx.IsThrowing = false;
    }

    // Enemy should exit pursue state if player hides or is in attacking range.
    public override void CheckSwitchStates() {
        if (ctx.HasDied) {
            SwitchState(factory.Death());
        } else if (throwTimer >= 0.5f) {
            SwitchState(factory.Pursue());
        }
    }

    public override void InitializeSubState() {
        
    }

    IEnumerator Throw(Vector2 dir) {
        ctx.transform.localScale = new Vector3(Mathf.Sign(dir.x) * Mathf.Abs(ctx.transform.localScale.x), ctx.transform.localScale.y, ctx.transform.localScale.z);
        yield return new WaitForSeconds(ctx.SpawnDelay);
        GameObject shuriken = GameObject.Instantiate(ctx.ShurikenPrefab, ctx.FirePointTrans.position, Quaternion.identity);
        Shuriken shurikenScript = shuriken.GetComponent<Shuriken>();
        shurikenScript.SetShurikenVelocity(dir);
    }
}
