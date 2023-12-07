using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Drop : MonoBehaviour
{
    [SerializeField]
    [Tooltip("How much time will this drop remain before being destroyed.")]
    private float destroyInXSeconds;
    [SerializeField]
    [Tooltip("How fast the light beam of the drop will fade in.")]
    private float fadeInSpeed;
    [SerializeField]
    [Tooltip("Identifies this drop")]
    private bool isShuriken;
    [SerializeField]
    [Tooltip("Identifies this drop")]
    private bool isHealth;
    [SerializeField]
    [Tooltip("Identifies this drop")]
    private bool isCoin;

    private float timeExisted;
    private UnityEngine.Rendering.Universal.Light2D light2D;
    private Rigidbody2D rb;
    private Collider2D col;
    private AudioSource pickUpSound;
    private SpriteRenderer sprite;
    private bool canPick;

    private PlayerController playerScript;

    // Start is called before the first frame update
    void Start()
    {
        if (!isCoin) {
            light2D = this.GetComponentInChildren<UnityEngine.Rendering.Universal.Light2D>();
        }
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
        if (isShuriken) {
           for (float t = 0f; t <= 0.7f; t += Time.deltaTime * fadeInSpeed) {
                light2D.color = new Color(0.5f, 0.5f, 0.5f, t);
                yield return new WaitForEndOfFrame();
            } 
        } else if (isHealth) {
            for (float t = 0f; t <= 0.7f; t += Time.deltaTime * fadeInSpeed) {
                light2D.color = new Color(1f, 0.792f, 0.059f, t);
                yield return new WaitForEndOfFrame();
            }
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
            Health healthScript = other.gameObject.GetComponent<Health>();
            if (isShuriken && playerScript.CanPickUpShuriken()) {
                playerScript.IncreaseShurikenNumBy(1);
                pickUpSound.Play();
                canPick = false;
                sprite.enabled = false;
                light2D.enabled = false;
                Invoke("Destroy", 2f);
            } else if (isHealth && healthScript.CanPickUpHealth()) {
                healthScript.IncreasePlayerHealth(1);
                pickUpSound.Play();
                canPick = false;
                sprite.enabled = false;
                light2D.enabled = false;
                Invoke("Destroy", 2f);
            } else if (isCoin) {
                pickUpSound.Play();
                canPick = false;
                sprite.enabled = false;
                GameManager.singleton.IncreaseGoldBy(1);
                Invoke("Destroy", 2f);
            }
        }
    }

    private void Destroy() {
        Destroy(this.gameObject);
    }
}
