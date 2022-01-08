using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Melee : MonoBehaviour
{
    private PolygonCollider2D meleeCollider;
    private List<Collider2D> enemyColliders;
    

    // Start is called before the first frame update
    void Start()
    {
        meleeCollider = this.GetComponent<PolygonCollider2D>();
        enemyColliders = new List<Collider2D>();
    }

    // When the melee attack hits an enemy
    private void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.tag == "Enemy") {
            if (!enemyColliders.Contains(other)) {
                enemyColliders.Add(other);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other) {
        if (other.gameObject.tag == "Enemy") {
            enemyColliders.Remove(other);
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
    public void RemoveFromList(Collider2D enemyCollider) {
        enemyColliders.Remove(enemyCollider);
    }

    // Returns the list of enemy colliders the melee will contact with.
    public List<Collider2D> GetEnemyColliders() {
        return enemyColliders;
    }
}
