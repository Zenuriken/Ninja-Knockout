using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyThrowState : EnemyState
{
    public EnemyThrowState(EnemyStateManager currContext, EnemyStateFactory stateFactory) 
    : base(currContext, stateFactory) {}

    public override void EnterState() {
        
    }

    public override void UpdateState() {
        CheckSwitchStates();
        if (ctx.CurrentState != this) return;
        
    }

    public override void ExitState() {
        ctx.CancelInvoke();
    }

    // Enemy should exit pursue state if player hides or is in attacking range.
    public override void CheckSwitchStates() {
        
    }

    public override void InitializeSubState() {
        
    }


    #region Attack Functions
    // private void Attack() {
    //     playerIsInMeleeRange = meleeEnemyScript.IsTouchingMeleeTrigger();
    //     playerIsInThrowingRange = alertedSightScript.IsTouchingAlertedTrigger() && IsWithinVectorBounds();
    //     if (playerIsInMeleeRange && isGrounded && CanAttack()) {
    //         isMeleeing = true;
    //         lastAttack = Time.time;
    //         sounds.Play("Meleeing");
    //         Invoke("SetIsMeleeingFalse", 0.5f);
    //         playerHealthScript.TakeDmg(dmg, this.transform.position);
    //     } else if (playerIsInThrowingRange && isGrounded && CanAttack() && unreachable) {
    //         Vector2 dir = (playerScript.transform.position - firePointTrans.position).normalized;
    //         RaycastHit2D raycastHit2D = Physics2D.CircleCast(firePointTrans.position, shurikenRadius, dir, 15f, playerAndPlatformLayerMask, 0f, 0f);
    //         Debug.DrawLine(firePointTrans.position, (Vector3)raycastHit2D.point);
    //         if (raycastHit2D.collider != null && raycastHit2D.collider.tag == "Player") StartCoroutine(Throw(dir));
    //     }
    // }


    // IEnumerator Throw(Vector2 dir) {
    //     isThrowing = true;
    //     if (dir.x >= 0) {
    //         transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
    //     } else {
    //         transform.localScale = new Vector3(-1f * Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
    //     }
    //     lastAttack = Time.time;
    //     Invoke("SetIsThrowingFalse", 0.5f);
    //     yield return new WaitForSeconds(spawnDelay);
    //     GameObject shuriken = Instantiate(shurikenPrefab, firePointTrans.position, Quaternion.identity);
    //     Shuriken shurikenScript = shuriken.GetComponent<Shuriken>();
    //     shurikenScript.SetShurikenVelocity(dir);
    // }
    #endregion
}
