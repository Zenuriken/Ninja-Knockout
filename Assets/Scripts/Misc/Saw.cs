using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Saw : MonoBehaviour
{
    private Vector2 node1;
    private Vector2 node2;

    private LinkedListNode<Vector2> targetNode;

    private SpriteRenderer sprite;
    public float moveSpeed;
    public Vector2 offSet;

    private LinkedList<Vector2> nodeList; 

    List<Vector2> verts;

    private LayerMask platformLayerMask;


    // Start is called before the first frame update
    void Awake()
    {
        sprite = this.GetComponent<SpriteRenderer>();

        // Initialize list of nodes.
        nodeList = new LinkedList<Vector2>();
        foreach (Transform child in this.transform) {
            nodeList.AddLast((Vector2)child.position);
        }
        //nodeList.Last.Next = nodeList.First;

        // Set the target node to the next node (starts already at node 0)
        targetNode = nodeList.First;
        platformLayerMask = LayerMask.GetMask("Platform");


    }


    private void Start() {
        List<Vector2> verts = new List<Vector2>();

        RaycastHit2D raycastHit2D = Physics2D.Raycast(this.transform.position, Vector2.down, 0.5f, platformLayerMask);

        if (raycastHit2D) {
            CompositeCollider2D compositeCollider = (CompositeCollider2D)raycastHit2D.collider;
            for(int i = 0; i < compositeCollider.pathCount; i++){
                Vector2[] pathVerts = new Vector2[compositeCollider.GetPathPointCount(i)];
                compositeCollider.GetPath(i, pathVerts);
                verts.AddRange(pathVerts);
            }
        }

        foreach (var vert in verts) {
            Debug.Log(vert);
        }

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector2 pos = (Vector2)this.transform.position;
        
        if (pos == targetNode.Value) {
            if (targetNode.Next == null) {
                targetNode = nodeList.First;
            } else {
                 targetNode = targetNode.Next;
            }
        }
        
        //Vector2 dir = (targetNode.Value - pos).normalized;


        // float xDir = targetNode.x - this.transform.position.x;
        // if (xDir > 0f) {
        //     sprite.flipX = true;
        // } else {
        //     sprite.flipX = false;
        // }
        Vector2 newPos = Vector2.MoveTowards(this.transform.position, targetNode.Value, moveSpeed * Time.deltaTime);
        this.transform.position = newPos;
    }
}
