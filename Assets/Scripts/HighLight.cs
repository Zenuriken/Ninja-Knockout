using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HighLight : MonoBehaviour
{
    [SerializeField]
    [Tooltip("The Highlight child object for this GameObject")]
    private GameObject highLight;

    // Sets the status of the highLight child.
    public void SetHighLight(bool state) {
        highLight.SetActive(state);
    }
}
