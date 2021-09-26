using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shuriken : MonoBehaviour
{
    
    public float spinSpeed;
    private GameObject player;
    private PlayerController playerScript;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        playerScript = player.GetComponent<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        float lastDir = playerScript.getPlayerDir();
        if (lastDir == -1) {
            this.transform.Rotate(0, 0, spinSpeed); 
        } else {
           this.transform.Rotate(0, 0, -spinSpeed);   
        }  
    }
}
