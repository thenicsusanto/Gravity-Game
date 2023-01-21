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
            StartCoroutine(RangedEnemyFrozen());
        }
    }

    public IEnumerator RangedEnemyFrozen()
    {
        GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
        GetComponentInChildren<Animator>().enabled = false;
        GameObject newIceCrystal = Instantiate(iceCrystal, transform.position, transform.localRotation);
        yield return new WaitForSeconds(6f);
        Destroy(newIceCrystal);
        GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
        GetComponentInChildren<Animator>().enabled = true;
    }

    public IEnumerator MeleeEnemyFrozen()
    {
        GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
        GetComponentInChildren<Animator>().enabled = false;
        GameObject newIceCrystal = Instantiate(iceCrystal, transform.position, transform.localRotation);
        yield return new WaitForSeconds(6f);
        Destroy(newIceCrystal);
        GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
        GetComponentInChildren<Animator>().enabled = true;
    }
}
