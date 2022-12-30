using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;


public class Lever : MonoBehaviour
{
    // Public Variables.
    public GameObject trapDoors;
    public Tile tile;
    public bool isHorizontal;

    // Private Variables.
    private GameObject highLight;
    private Vector3Int[] tileList;
    private Tilemap platformTilemap;
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

        platformTilemap = GameObject.Find("Tilemap_Platform").GetComponent<Tilemap>();

        SetPlatformTiles();
    }

    public void Switch() {
        // Closing doors.
        if (isOn) {
            anim.SetBool("isOn", false);
            isOn = false;
            leftDoor.Rotate(0f, 0f, 90f, Space.Self);
            rightDoor.Rotate(0f, 0f, -90f, Space.Self);
            switchSound.Play();
            trapDoorSound.Play();
            TogglePlatformTiles(false);
        // Opening doors.
        } else {
            anim.SetBool("isOn", true);
            isOn = true;
            leftDoor.Rotate(0f, 0f, -90f, Space.Self);
            rightDoor.Rotate(0f, 0f, 90f, Space.Self);
            switchSound.Play();
            trapDoorSound.Play();
            TogglePlatformTiles(true);
        }
    }

    public void SetHighLight(bool status) {
        highLight.SetActive(status);
    }

    public void SetMeleeCounter(int num) {
        meleeCounter = num;
    }

    // Initializes a list of platform tiles that the trapdoor covers.
    private void SetPlatformTiles() {
        tileList = new Vector3Int[4];
        Vector3Int leftTile = platformTilemap.WorldToCell(leftDoor.position);
        if (isHorizontal) {
            for (int i = 0; i < 4; i++) {
                Vector3Int t = new Vector3Int(leftTile.x + i + 1, leftTile.y, leftTile.z);
                tileList[i] = t;
            }
        } else {
            for (int i = 0; i < 4; i++) {
                Vector3Int t = new Vector3Int(leftTile.x, leftTile.y + i + 1, leftTile.z);
                tileList[i] = t;
            }
        }
    }

    // Toggles whethre the platform tiles are present or not.
    private void TogglePlatformTiles(bool isOn) {
        if (isOn) {
            foreach (Vector3Int t in tileList) {
                platformTilemap.SetTile(t, null);
            }
        } else {
            foreach (Vector3Int t in tileList) {
                platformTilemap.SetTile(t, tile);
            }
        }
    }

    // Checks to see if enemy has already been damaged by player's current meleeCounter.
    public bool HasBeenDamaged(int counter) {
        return this.meleeCounter == counter;
    }
}
