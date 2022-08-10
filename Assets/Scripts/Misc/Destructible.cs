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
    [SerializeField]
    [Tooltip("The delay before object begins to fade.")]
    private float fadeAwayDelay;
    [SerializeField]
    [Tooltip("The speed in which object fades away.")]
    private float fadeAwaySpeed;

    private Melee meleeScript;
    private bool canDestroy;
    private GameObject highLight;
    private SpriteRenderer sprite;
    private AudioSource sound;
    private Animator anim;
    private int meleeCounter;

    private void Awake() {
        highLight = this.transform.GetChild(0).gameObject;
        sprite = this.GetComponent<SpriteRenderer>();
        sound = this.GetComponent<AudioSource>();
        anim = this.GetComponent<Animator>();
        meleeScript = GameObject.Find("Player").gameObject.transform.GetChild(1).GetComponent<Melee>();
        canDestroy = true;
        anim.enabled = false;
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
        SetHighLight(false);
        sound.Play();
        this.gameObject.layer = 12; // Set it to a layer where the player can't interact with it.
        anim.enabled = true;
        yield return new WaitForSeconds(fadeAwayDelay);
        meleeScript.RemoveDestructibleFromList(this.GetComponent<Collider2D>());
        for (float alpha = 1f; alpha > 0f; alpha -= Time.deltaTime * fadeAwaySpeed) {
            sprite.color = new Color(1f, 1f, 1f, alpha);
            yield return new WaitForEndOfFrame();
        }
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
