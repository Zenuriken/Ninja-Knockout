using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class ShurikenDrop : MonoBehaviour
{
    [SerializeField]
    [Tooltip("How much time will this drop remain before being destroyed.")]
    private float destroyInXSeconds;
    [SerializeField]
    [Tooltip("How fast the light beam of the drop will fade in.")]
    private float fadeInSpeed;

    private float timeExisted;
    private Light2D light;
    private Rigidbody2D rb;
    private Collider2D col;
    private AudioSource pickUpSound;
    private SpriteRenderer sprite;
    private bool canPick;

    private PlayerController playerScript;

    // Start is called before the first frame update
    void Start()
    {
        light = this.GetComponentInChildren<Light2D>();
        rb = this.GetComponent<Rigidbody2D>();
        col = this.GetComponent<CircleCollider2D>();
        sprite = this.GetComponent<SpriteRenderer>();
        pickUpSound = this.GetComponent<AudioSource>();
        playerScript = GameObject.Find("Player").GetComponent<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        timeExisted += Time.deltaTime;
        if (timeExisted >= destroyInXSeconds) {
            Destroy(this.gameObject);
        }
    }

    IEnumerator FadeIn() {
        for (float t = 0f; t <= 0.7f; t += Time.deltaTime * fadeInSpeed) {
            light.color = new Color(0.5f, 0.5f, 0.5f, t);
            yield return new WaitForEndOfFrame();
        }
        canPick = true;
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.tag == "Platform") {
            rb.constraints = RigidbodyConstraints2D.FreezeAll;
            StartCoroutine("FadeIn");
        }
    }

    private void OnTriggerStay2D(Collider2D other) {
        if (other.gameObject.tag == "Player" && canPick) {
            playerScript.IncreaseShurikenNumBy(1);
            pickUpSound.Play();
            canPick = false;
            sprite.enabled = false;
            light.enabled = false;
            Invoke("Destroy", 2f);
        }
    }

    private void Destroy() {
        Destroy(this.gameObject);
    }
}
