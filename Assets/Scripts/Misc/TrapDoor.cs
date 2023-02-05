using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TrapDoor : MonoBehaviour
{
    [SerializeField][Tooltip("Where tiles will be spawned.")]
    private List<Transform> nodeList;
    [SerializeField][Tooltip("The tile to spawn in place of door.")]
    private Tile doorTile;

    private Tilemap platformTilemap;
    private Transform leftDoor;
    private Transform rightDoor;
    private AudioSource trapDoorSound;
    private float dir;

    private void Awake() {
        trapDoorSound = this.GetComponent<AudioSource>();
        rightDoor = this.transform.GetChild(0).transform;
        leftDoor = this.transform.GetChild(1).transform;
        platformTilemap = GameObject.Find("Tilemap_Platform").GetComponent<Tilemap>();

        dir = 1f;
        SetPlatformTiles();
    }

    public void Switch() {
        DeletePlatformTiles();
        leftDoor.Rotate(0f, 0f,  dir * -90f, Space.Self);
        rightDoor.Rotate(0f, 0f, dir * 90f, Space.Self);
        SetPlatformTiles();
        trapDoorSound.Play();
        dir = -dir;
    }

    // Deletes the tiles at node position if they are temporary tiles.
    private void DeletePlatformTiles() {
        foreach (Transform node in nodeList) {
            Vector3Int tilePos = platformTilemap.WorldToCell(node.position);
            if (node.gameObject.activeInHierarchy && platformTilemap.HasTile(tilePos) && platformTilemap.GetTile(tilePos) == doorTile) platformTilemap.SetTile(tilePos, null);
        }
    }

    // Initializes a list of platform tiles that the trapdoor covers.
    private void SetPlatformTiles() {
        foreach (Transform node in nodeList) {
            Vector3Int tilePos = platformTilemap.WorldToCell(node.position);
            if (node.gameObject.activeInHierarchy && !platformTilemap.HasTile(tilePos)) platformTilemap.SetTile(tilePos, doorTile);
        }
    }
}
