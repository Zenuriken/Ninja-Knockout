using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Melee : MonoBehaviour
{
    #region Private Variables
    private List<Collider2D> enemyColliders;
    private List<Collider2D> projectileColliders;
    private List<Collider2D> platformColliders;
    private List<Collider2D> leverColliders;
    private List<Collider2D> destructibleColliders;

    //private PEnemyStateManager playerScript;
    #endregion

    #region Initialization Functions
    // Start is called before the first frame update
    void Start()
    {
        enemyColliders = new List<Collider2D>();
        projectileColliders = new List<Collider2D>();
        platformColliders = new List<Collider2D>();
        leverColliders = new List<Collider2D>();
        destructibleColliders = new List<Collider2D>();
    }
    #endregion

    #region Collision Functions
    // When the melee attack hits an enemy or projectile
    private void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.tag == "Enemy") {
            if (!enemyColliders.Contains(other) /*&& playerScript.IsFreeOfAction()*/) {
                enemyColliders.Add(other);
                EnemyStateManager enemyScript = other.GetComponent<EnemyStateManager>();
                //enemyScript.SetIsInPlayerMeleeRange(true);
                enemyScript.SetHighLight(true);
            }
        } else if (other.gameObject.tag == "Projectile") {
            if (!projectileColliders.Contains(other)) {
                projectileColliders.Add(other);
            }
        } else if (other.gameObject.tag == "Platform") {
            if (!platformColliders.Contains(other)) {
                platformColliders.Add(other);
            }
        } else if (other.gameObject.tag == "Lever") {
            if (!leverColliders.Contains(other)) {
                leverColliders.Add(other);
                Lever leverScript = other.GetComponent<Lever>();
                leverScript.SetHighLight(true);
            }
        } else if (other.gameObject.tag == "Destructible") {
            if (!destructibleColliders.Contains(other)) {
                destructibleColliders.Add(other);
                Destructible destructibleScript = other.GetComponent<Destructible>();
                destructibleScript.SetHighLight(true);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other) {
        if (other.gameObject.tag == "Enemy") {
            enemyColliders.Remove(other);
            EnemyStateManager enemyScript = other.GetComponent<EnemyStateManager>();
            //enemyScript.SetIsInPlayerMeleeRange(false);
            enemyScript.SetHighLight(false);
        } else if (other.gameObject.tag == "Projectile") {
            projectileColliders.Remove(other);
        } else if (other.gameObject.tag == "Platform") {
            platformColliders.Remove(other);
        } else if (other.gameObject.tag == "Lever") {
            leverColliders.Remove(other);
            Lever leverScript = other.GetComponent<Lever>();
            leverScript.SetHighLight(false);
        } else if (other.gameObject.tag == "Destructible") {
            destructibleColliders.Remove(other);
            Destructible destructibleScript = other.GetComponent<Destructible>();
            destructibleScript.SetHighLight(false);
        }
    }
    #endregion

    #region Public Functions
    // Removes an enemy from the list of enemy colliders upon death
    public void RemoveEnemyFromList(Collider2D enemyCollider) {
        enemyColliders.Remove(enemyCollider);
        EnemyStateManager enemyScript = enemyCollider.GetComponent<EnemyStateManager>();
        //enemyScript.SetIsInPlayerMeleeRange(false);
        enemyScript.SetHighLight(false);
    }

    // Removes an projectile from the list of projectile colliders upon destruction
    public void RemoveProjFromList(Collider2D projCollider) {
        if (projectileColliders.Contains(projCollider)) {
            projectileColliders.Remove(projCollider);
        }
    }

    // Removes a destructible from the list of destructible colliders upon destruction
    public void RemoveDestructibleFromList(Collider2D col) {
        if (destructibleColliders.Contains(col)) {
            destructibleColliders.Remove(col);
        }
    }

    // Returns the list of enemy colliders the melee will contact with.
    public List<Collider2D> GetEnemyColliders() {
        return enemyColliders;
    }

    // Returns the list of projectile colliders the melee will contact with.
    public List<Collider2D> GetProjectileColliders() {
        return projectileColliders;
    }

    // Returns the list of platform colliders the melee will contact with.
    public List<Collider2D> GetPlatformColliders() {
        return platformColliders;
    }

    // Returns the list of levers the melee will contact with.
    public List<Collider2D> GetLeverColliders() {
        return leverColliders;
    }

    // Returns the list of destructibles the melee will contact with.
    public List<Collider2D> GetDestructibleColliders() {
        return destructibleColliders;
    }
    #endregion
}
