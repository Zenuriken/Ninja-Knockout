using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelSelectManager : MonoBehaviour
{
    public List<GameObject> levels;
    private AudioSource sound;
    private int lvl;
    private int lastLvl;
    private bool axisInUse;
    
    // Start is called before the first frame update
    void Start()
    {
        sound = this.GetComponent<AudioSource>();
        levels[lvl].transform.GetChild(0).gameObject.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        int xInput = (int) Input.GetAxisRaw("Horizontal");
        if (xInput != 0 && !axisInUse) {
            lvl += xInput;
            lvl = (int) Mathf.Clamp(lvl, 0, levels.Count - 1);
            axisInUse = true;
        } else if (xInput == 0) {
            axisInUse = false;
        }
        if (lvl != lastLvl) HighLightLvl();

        if (Input.GetKeyDown(KeyCode.Return)) {
            // GameManager.singleton.LoadLevel(lvl);
            sound.Play();
        }
    }

    // Sets the highlight of the current level
    private void HighLightLvl() {
        levels[lastLvl].transform.GetChild(0).gameObject.SetActive(false);
        levels[lvl].transform.GetChild(0).gameObject.SetActive(true);
        lastLvl = lvl;
        sound.Play();
    }
}
