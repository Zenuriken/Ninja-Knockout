using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Saw : MonoBehaviour
{
    private Vector2 node1;
    private Vector2 node2;

    private Vector2 targetNode;

    private SpriteRenderer sprite;
    public float moveSpeed;
    public Vector2 offSet;


    // Start is called before the first frame update
    void Awake()
    {
        sprite = this.GetComponent<SpriteRenderer>();
        node1 = (Vector2)this.transform.GetChild(0).transform.position + offSet;
        node2 = (Vector2)this.transform.GetChild(1).transform.position - offSet;

        targetNode = node2;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (this.transform.position.x == targetNode.x) {
            if (node1 != targetNode) {
                targetNode = node1;
            } else {
                targetNode = node2;
            }

        }
        
        float xDir = targetNode.x - this.transform.position.x;
        if (xDir >= 0f) {
            sprite.flipX = true;
        } else {
            sprite.flipX = false;
        }
        Vector2 newPos = Vector2.MoveTowards(this.transform.position, targetNode, moveSpeed * Time.deltaTime);
        this.transform.position = newPos;
    }
}
