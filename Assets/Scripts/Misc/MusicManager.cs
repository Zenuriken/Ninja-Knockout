using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public static MusicManager singleton;
    private Dictionary<string, AudioSource> audioSources;

    private AudioSource currAudio;

    private void Awake() {
        if (singleton != null && singleton != this) { 
            Destroy(this.gameObject); 
        } else { 
            singleton = this;
            audioSources = new Dictionary<string, AudioSource>();
            foreach (Transform child in this.transform) {
                audioSources[child.name] = child.GetComponent<AudioSource>();
            }
            DontDestroyOnLoad(this.gameObject);
        }
    }

    public void Play(string name) {
        AudioSource source = audioSources[name];
        source.Play();
    }

    public void Stop() {
        currAudio.Stop();
    }

    public void FadeInAudio(string name) {
        AudioSource audioSource = audioSources[name];
        audioSource.volume = 0f;
        audioSource.Play();
        currAudio = audioSource;
        StartCoroutine(StartFade(audioSources[name], 3f, 0.25f));
    }

    public void FadeOutAudio() {
        StartCoroutine(StartFade(currAudio, 3f, 0f));
    }

    IEnumerator StartFade(AudioSource audioSource, float duration, float targetVolume) {
        float currentTime = 0;
        float start = audioSource.volume;
        while (currentTime < duration)
        {
            currentTime += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(start, targetVolume, currentTime / duration);
            yield return null;
        }
        yield break;
    }
}
