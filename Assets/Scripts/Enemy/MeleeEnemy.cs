using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeEnemy : MonoBehaviour
{
    #region Private Variables
    private bool playerContact;
    private List<Collider2D> projectileColliders;
    #endregion

    #region Initialization Functions
    // Start is called before the first frame update
    void Start()
    {
        projectileColliders = new List<Collider2D>();
    }
    #endregion

    #region Collision Functions
    // When the melee attack hits an enemy or projectile
    private void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.tag == "Player") {
            playerContact = true;
        } else if (other.gameObject.tag == "Projectile") {
            if (!projectileColliders.Contains(other)) {
                projectileColliders.Add(other);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other) {
        if (other.gameObject.tag == "Player") {
            playerContact = false;
        } else if (other.gameObject.tag == "Projectile") {
            projectileColliders.Remove(other);
        }
    }
    #endregion

    #region Public Functions
    // Removes an enemy from the list of projectile colliders upon destruction
    public void RemoveProjFromList(Collider2D projCollider) {
        if (projectileColliders.Contains(projCollider)) {
            projectileColliders.Remove(projCollider);
        }
    }

    // Returns the list of projectile colliders the melee will contact with.
    public List<Collider2D> GetProjectileColliders() {
        return projectileColliders;
    }

    // Returns whether the player is in contact with the enemy's melee collider.
    public bool IsTouchingMeleeTrigger() {
        return playerContact;
    }
    #endregion
}
