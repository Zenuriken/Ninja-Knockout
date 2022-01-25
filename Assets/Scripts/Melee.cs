using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Melee : MonoBehaviour
{
    private PolygonCollider2D meleeCollider;
    private List<Collider2D> enemyColliders;
    private List<Collider2D> projectileColliders;
    

    // Start is called before the first frame update
    void Start()
    {
        meleeCollider = this.GetComponent<PolygonCollider2D>();
        enemyColliders = new List<Collider2D>();
        projectileColliders = new List<Collider2D>();
    }

    // When the melee attack hits an enemy or projectile
    private void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.tag == "Enemy") {
            if (!enemyColliders.Contains(other)) {
                enemyColliders.Add(other);
            }
        } else if (other.gameObject.tag == "Projectile") {
            if (!projectileColliders.Contains(other)) {
                //Debug.Log("Shuriken entered: " + Time.time);
                projectileColliders.Add(other);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other) {
        if (other.gameObject.tag == "Enemy") {
            enemyColliders.Remove(other);
        } else if (other.gameObject.tag == "Projectile") {
            projectileColliders.Remove(other);
            //Debug.Log("Shuriken exited: " + Time.time);
        }
    }

    // Damages the enemy
    private void DamageEnemy(Collider2D other) {
        EnemyController enemyScript = other.gameObject.GetComponent<EnemyController>();
        if (!enemyScript.IsAlerted()) {
            enemyScript.TakeDmg(5);
        } else {
            enemyScript.TakeDmg(1);
        }
        //Debug.Log(enemyScript.GetHealth());
    }

    // Removes an enemy from the list of enemy colliders upon death
    public void RemoveEnemyFromList(Collider2D enemyCollider) {
        enemyColliders.Remove(enemyCollider);
    }

    // Removes an enemy from the list of projectile colliders upon destruction
    public void RemoveProjFromList(Collider2D projCollider) {
        if (projectileColliders.Contains(projCollider)) {
            projectileColliders.Remove(projCollider);
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
}
