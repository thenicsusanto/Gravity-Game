using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordContainerAssign : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GameManager.Instance.swordContainer = gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
