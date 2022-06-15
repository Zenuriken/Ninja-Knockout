using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parallax : MonoBehaviour
{
    [SerializeField]
    [Tooltip("The camera to apply the parallax effect to.")]
    private GameObject cam;
    [SerializeField]
    [Tooltip("The parallax effect to apply to this layer.")]
    private float parallaxEffect;
    [SerializeField]
    [Tooltip("The parallax offset.")]
    private float offset;
    
    private float length;
    private float startPos;

    private void Start() {
        startPos = transform.position.x;
        length = GetComponent<SpriteRenderer>().bounds.size.x;
    }
    
    // Update is called once per frame
    void Update()
    {
        float temp = (cam.transform.position.x * (1 - parallaxEffect));
        float dist = (cam.transform.position.x * parallaxEffect);

        transform.position = new Vector3(startPos + dist, transform.position.y, transform.position.z);

        if (temp > startPos + length - offset) {
            startPos += length;
        } else if (temp < startPos - length + offset) {
            startPos -= length;
        }
    }
}
