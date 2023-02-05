using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;


public class Lever : MonoBehaviour
{
    [SerializeField][Tooltip("The trap doors the lever is connected to.")]
    private TrapDoor trapDoorScript;
    
    // Private Variables.
    private GameObject highLight;
    private Animator anim;
    private AudioSource switchSound;
    private int meleeCounter;
    private bool isOn;

    private void Awake() {
        highLight = this.transform.GetChild(0).gameObject;
        anim = this.GetComponent<Animator>();
        switchSound = this.GetComponent<AudioSource>();
    }

    public void Switch() {
        isOn = !isOn;
        anim.SetBool("isOn", isOn);
        switchSound.Play();
        trapDoorScript.Switch();
    }

    public void SetHighLight(bool status) {
        highLight.SetActive(status);
    }

    public void SetMeleeCounter(int num) {
        meleeCounter = num;
    }

    // Checks to see if enemy has already been damaged by player's current meleeCounter.
    public bool HasBeenDamaged(int counter) {
        return this.meleeCounter == counter;
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.tag == "Projectile") {
            Switch();
        }
    }
}
