using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Blink : MonoBehaviour
{
    Image target;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // IEnumerator Blink() {
    //     for (float alpha = 1f; alpha >= 0.75f; alpha -= 0.05f) {
    //         target.color = new Color(1f, 1f, 1f, alpha);
    //         yield return new WaitForSeconds(0.01f);
    //     }

    //     for (int i = 0; i < bufferDur; i++) {
    //         for (float alpha = 0.75f; alpha >= 0.25f; alpha -= 0.05f) {
    //             target.color = new Color(1f, 1f, 1f, alpha);
    //             yield return new WaitForSeconds(0.01f);
    //         }
    //         for (float alpha = 0.25f; alpha <= 0.75f; alpha += 0.05f) {
    //             target.color = new Color(1f, 1f, 1f, alpha);
    //             yield return new WaitForSeconds(0.01f);
    //         }
    //     }

    //     for (float alpha = 0.75f; alpha <= 1f; alpha += 0.05f) {
    //         target.color = new Color(1f, 1f, 1f, alpha);
    //         yield return new WaitForSeconds(0.01f);
    //     }
    // }
}
