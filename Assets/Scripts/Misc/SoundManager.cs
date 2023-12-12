using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    private Dictionary<string, AudioSource> audioSources;
    private Dictionary<string, float> audioVolumes;

    private void Awake() {
        audioSources = new Dictionary<string, AudioSource>();
        audioVolumes = new Dictionary<string, float>();
        foreach (Transform child in this.transform) {
            audioSources[child.name] = child.GetComponent<AudioSource>();
            audioVolumes[child.name] = child.GetComponent<AudioSource>().volume;
        }
        AdjustSoundVolume();
    }

    public void Play(string name) {
        AudioSource source = audioSources[name];
        source.Play();
    }

    public void Stop(string name) {
        AudioSource source = audioSources[name];
        source.Stop();
    }

    public void AdjustSoundVolume() {
        foreach (AudioSource audio in audioSources.Values) {
            audio.volume = audioVolumes[audio.name] * PlayerPrefs.GetFloat("volume");
        }
    }
    // private void Awake() {
    //     if (singleton != null && singleton != this) { 
    //         Destroy(this); 
    //     } 
    //     else { 
    //         singleton = this;

    //     }
    // }

    // public void PlayShuriken() {
    //     AudioSource shurikenAudio = audioSources[0];
    //     shurikenAudio.Play();
    // }

    // public void StopShuriken(bool hitEnemy) {
    //     AudioSource shurikenAudio = audioSources[0];
    //     shurikenAudio.Stop();
    //     AudioSource contact;
    //     if (hitEnemy) {
    //         contact = audioSources[1];
    //     } else {
    //         contact = audioSources[2];
    //     }
    //     contact.Play();
    // }

    // public void PlayWallClimb() {

    // }

    // public void StopWallClimb() {

    // }
    
    
}
