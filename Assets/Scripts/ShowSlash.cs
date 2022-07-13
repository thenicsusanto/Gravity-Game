using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowSlash : MonoBehaviour
{
    public GameObject slash;
    void Start()
    {
        slash.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void EnableSlashVFX()
    {
        slash.SetActive(true);
    }

    void DisableSlashVFX()
    {
        slash.SetActive(false);
    }
}
