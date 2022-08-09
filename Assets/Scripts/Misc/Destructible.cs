using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destructible : MonoBehaviour
{
    [SerializeField]
    [Tooltip("The item drop prefab.")]
    private GameObject shurikenDropPrefab;
    [SerializeField]
    [Tooltip("The number of items this item will drop when broke.")]
    private int numDrop;
    [SerializeField]
    [Tooltip("The chance each item will drop.")]
    private int chancePerDrop;
    [SerializeField]
    [Tooltip("The left bound for spawning drops.")]
    private float xOffset;

    private Melee meleeScript;
    private bool canDestroy;
    private GameObject highLight;
    private SpriteRenderer sprite;
    private AudioSource sound;
    private int meleeCounter;

    private void Awake() {
        highLight = this.transform.GetChild(0).gameObject;
        sprite = this.GetComponent<SpriteRenderer>();
        sound = this.GetComponent<AudioSource>();
        meleeScript = GameObject.Find("Player").gameObject.transform.GetChild(1).GetComponent<Melee>();
        canDestroy = true;
    }

    public void Break() {
        if (canDestroy) {
            for (int i = 0; i < numDrop; i++) {
                int roll = Random.Range(0, 100);
                if (roll < chancePerDrop) {
                    GameObject shurikenDrop = GameObject.Instantiate(shurikenDropPrefab, GetRandomPos(), Quaternion.identity);
                }
            }
            StartCoroutine("Destroy");
        }
    }

    IEnumerator Destroy() {
        canDestroy = false;
        sprite.enabled = false;
        SetHighLight(false);
        sound.Play();
        this.gameObject.layer = 12; // Set it to a layer where the player can't interact with it.
        yield return new WaitForSeconds(1f);
        meleeScript.RemoveDestructibleFromList(this.GetComponent<Collider2D>());
        Destroy(this.gameObject);
    }

    private Vector2 GetRandomPos() {
        float xPos = Random.Range(-xOffset, xOffset);
        return new Vector2(this.transform.position.x + xPos, this.transform.position.y);
    }

    public void SetHighLight(bool state) {
        if ((state && canDestroy) || !state) {
            highLight.SetActive(state);
        }
    }

    public bool HasBeenDamaged(int counter) {
        return this.meleeCounter == counter;
    }

    public void SetMeleeCounter(int counter) {
        this.meleeCounter = counter;
    }

    public bool CanBreak() {
        return canDestroy;
    }
}
