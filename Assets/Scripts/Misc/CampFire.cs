using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CampFire : MonoBehaviour
{
    [SerializeField]
    [Tooltip("Controls how fast the highlight will light up")]
    private float lightUpSpeed;
    [SerializeField]
    [Tooltip("How long the player must remain near the fire to regenerate health")]
    private float regenTime;
    [SerializeField]
    [Tooltip("Sets whether the campfire should already be activated.")]
    private bool hasActivated;

    private bool isActive;
    private bool isFullyLit;
    private float currRegenTime;
    private GameObject fireParticles;
    private SoundManager sounds;
    private AudioSource fireSound;
    private UnityEngine.Rendering.Universal.Light2D highLight;

    private Health healthScript;
    private Vector2 pos;

    private void Start() {
        fireParticles = this.transform.GetChild(0).gameObject;
        highLight = this.transform.GetChild(1).GetComponent<UnityEngine.Rendering.Universal.Light2D>();
        sounds = this.transform.GetChild(2).GetComponent<SoundManager>();
        healthScript = PlayerController.singleton.GetComponent<Health>();
        pos = this.transform.position;

        if (hasActivated) {
            fireParticles.SetActive(true);
            highLight.gameObject.SetActive(true);
            highLight.intensity = 5f;
            isActive = true;
            isFullyLit = true;
            sounds.Play("CampFire");
        } else {
            fireParticles.SetActive(false);
        }
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.tag == "Player" && !isActive) {
            StartCoroutine("LightUp");

            GameManager.singleton.SetSpawnLocation(new Vector2(pos.x - 2f, pos.y + 1f));

            // PlayerController.singleton.SetCampFirePos(new Vector2(pos.x - 2f, pos.y + 1f));
            // PlayerController.singleton.SetSpawnLocation(new Vector2(pos.x - 2f, pos.y + 1f));
        }
    }

    private void OnTriggerStay2D(Collider2D other) {
        if (other.gameObject.tag == "Player" && isFullyLit) {
            currRegenTime += Time.deltaTime;
            if (healthScript.CanPickUpHealth()) {
                PlayerController.singleton.SetHealthParticles(true);
                if (currRegenTime >= regenTime) {
                    PlayerController.singleton.IncreaseHealthBy(1);
                    currRegenTime = 0f;
                }
            } else {
                PlayerController.singleton.SetHealthParticles(false);
                currRegenTime = 0f;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other) {
        if (other.gameObject.tag == "Player" && isFullyLit) {
            PlayerController.singleton.SetHealthParticles(false);
            currRegenTime = 0f;
        }
    }

    IEnumerator LightUp() {
        isActive = true;
        sounds.Play("Lighter");
        sounds.Play("CampFire");
        fireParticles.SetActive(true);
        highLight.gameObject.SetActive(true);
        isFullyLit = true;
        for (float i = 1f; i <= 5f; i += Time.deltaTime * lightUpSpeed) {
            highLight.intensity = i;
            yield return new WaitForEndOfFrame();
        }
    }


    public bool HasActivated() {
        return isActive;
    }

    public void SetHasActivated(bool state) {
        hasActivated = state;
    }
}
