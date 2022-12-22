using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreezeEnemy : MonoBehaviour
{
    public GameObject iceCrystal;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlayFreeze()
    {
        if(GetComponent<MeleeEnemyController>() != null)
        {
            StartCoroutine(MeleeEnemyFrozen());
        } 
        else if(GetComponent<RangedEnemyController>() != null)
        {
            StartCoroutine(MeleeEnemyFrozen());
        }
        
    }

    public IEnumerator RangedEnemyFrozen()
    {
        GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
        GetComponentInChildren<Animator>().speed = 0;
        GameObject newIceCrystal = Instantiate(iceCrystal, transform.localPosition, transform.localRotation);
        yield return new WaitForSeconds(2f);
        Destroy(newIceCrystal);
        GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
        GetComponentInChildren<Animator>().speed = 1;
    }

    public IEnumerator MeleeEnemyFrozen()
    {
        GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
        GetComponentInChildren<Animator>().speed = 0;
        GameObject newIceCrystal = Instantiate(iceCrystal, transform.localPosition, transform.localRotation);
        yield return new WaitForSeconds(2f);
        Destroy(newIceCrystal);
        GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
        GetComponentInChildren<Animator>().speed = 1;
    }
}
