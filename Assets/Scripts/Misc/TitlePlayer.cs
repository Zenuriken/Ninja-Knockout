using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitlePlayer : MonoBehaviour
{
    private Animator playerAnim;
    private Rigidbody2D playerRB;
    
    private void Start() {
        playerAnim = GetComponent<Animator>();
        playerRB = GetComponent<Rigidbody2D>();
        playerAnim.SetBool("isMoving", true);
    }
    
    private void Update() {
        playerRB.velocity = new Vector2(10f, 0f);
    }
}
