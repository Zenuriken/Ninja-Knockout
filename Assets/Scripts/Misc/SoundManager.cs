using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager singleton;
    public List<AudioSource> audioSources;

    private void Awake() {
        if (singleton != null && singleton != this) { 
            Destroy(this); 
        } 
        else { 
            singleton = this;
            
        }
    }

    private void Start() {
        
    }
    
    
}
