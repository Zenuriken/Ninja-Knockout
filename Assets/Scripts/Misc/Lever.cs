using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lever : MonoBehaviour
{
    private GameObject highLight;
    public GameObject trapDoors;
    private Transform leftDoor;
    private Transform rightDoor;
    private Animator anim;
    private AudioSource switchSound;
    private AudioSource trapDoorSound;
    private bool isOn;
    private int meleeCounter;

    private void Awake() {
        highLight = this.transform.GetChild(0).gameObject;
        anim = this.GetComponent<Animator>();
        switchSound = this.GetComponent<AudioSource>();
        trapDoorSound = trapDoors.GetComponent<AudioSource>();

        rightDoor = trapDoors.transform.GetChild(0).transform;
        leftDoor = trapDoors.transform.GetChild(1).transform;
    }

    public void Switch() {
        if (isOn) {
            anim.SetBool("isOn", false);
            isOn = false;
            leftDoor.Rotate(0f, 0f, 90f, Space.Self);
            rightDoor.Rotate(0f, 0f, -90f, Space.Self);
            switchSound.Play();
            trapDoorSound.Play();

        } else {
            anim.SetBool("isOn", true);
            isOn = true;
            leftDoor.Rotate(0f, 0f, -90f, Space.Self);
            rightDoor.Rotate(0f, 0f, 90f, Space.Self);
            switchSound.Play();
            trapDoorSound.Play();
        }
        //Debug.Log("IsOn: " + isOn);
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
}
