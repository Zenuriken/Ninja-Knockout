using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MusicManager : MonoBehaviour
{
    public static MusicManager singleton;

    [SerializeField][Tooltip("Controls the master volume")]
    private int maxVolume;

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

    // called first
    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    // called second
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (currAudio != null) return;

        if (scene.name == "Level0") {
            PlayAudio("Traveler");
        } 
    }

    // called third
    void Start()
    {
    }

    // called when the game is terminated
    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    public void PlayAudio(string name) {
        AudioSource audioSource = audioSources[name];
        audioSource.Play();
        currAudio = audioSource;
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
