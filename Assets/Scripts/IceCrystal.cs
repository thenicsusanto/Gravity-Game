using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceCrystal : MonoBehaviour
{
    public AudioSource iceCrystalBreakSound;
    // Start is called before the first frame update
    void Start()
    {
        Invoke("DestroyObject", 4);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void DestroyObject()
    {
        FindObjectOfType<AudioManager>().Play("IceBreakingSound");
        gameObject.GetComponent<MeshRenderer>().enabled = false;
        Destroy(gameObject, 2);
    }
}
