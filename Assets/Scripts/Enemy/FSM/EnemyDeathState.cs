using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDeathState : EnemyState
{
    public EnemyDeathState(EnemyStateManager currContext, EnemyStateFactory stateFactory) 
    : base(currContext, stateFactory) {}

    public override void EnterState() {
        Debug.Log("DEAD");
        
    }

    public override void UpdateState() {
        CheckSwitchStates();
        if (ctx.CurrentState != this) return;
        
    }

    public override void ExitState() {
        
    }

    // Enemy should exit pursue state if player hides or is in attacking range.
    public override void CheckSwitchStates() {

    }

    public override void InitializeSubState() {
        
    }

    // // Kills the enemy
    // IEnumerator Death() {
    //     hasDied = true;
    //     // if (!isAlerted) {
    //     //     UIManager.singleton.IncreaseScoreBy(enemyPoints * 2);
    //     // } else {
    //     //     UIManager.singleton.IncreaseScoreBy(enemyPoints);
    //     // }
    //     alertedObj.SetActive(false);
    //     SetHighLight(false);
    //     if (playerIsInThrowingRange && isAlerted) {
    //         playerScript.IncreaseAlertedNumBy(-1);
    //     }
    //     Destroy(fov.gameObject);
    //     this.gameObject.layer = 12; // Set the gameObject to layer 12
    //     Invoke("BodySplat", bodySplatDelay);
    //     StartCoroutine("FadeAway");

    //     float value = Random.Range(0, 100);
    //     if (value < dropChance) {
    //         GameObject shurikenDrop = Instantiate(shurikenDropPrefab, this.transform.position, Quaternion.identity);
    //     }

    //     yield return new WaitForSeconds(destroyDelay);
    //     meleeScript.RemoveEnemyFromList(enemyCollider);
    //     Destroy(this.gameObject);
    // }

    // private void BodySplat() {
    //     if (isGrounded && Mathf.Abs(enemyRB.velocity.x) < 0.05f && Mathf.Abs(enemyRB.velocity.y) < 0.05f) {
    //         sounds.Play("BodySplat");
    //         playedBodySplat = true;
    //     } else {
    //         bodySplatDelayPast = true;
    //         playedBodySplat = false;
    //     }
    // }

}
