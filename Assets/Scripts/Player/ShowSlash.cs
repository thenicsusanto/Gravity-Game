using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowSlash : MonoBehaviour
{
    public GameObject slash;
    public ShopManager shopManager;
    public SwordCollider swordCollider;

    void Start()
    {
        swordCollider = GetComponent<SwordCollider>();
    }

    void Update()
    {
        //if(shopManager.purchasedItem == true)
        //{
        //    slash = GameObject.FindGameObjectWithTag("Slash");
        //    shopManager.purchasedItem = false;
        //    slash.SetActive(false);
        //}
    }

    void EnableSlashVFX()
    {
        slash.SetActive(true);
    }

    void DisableSlashVFX()
    {
        slash.SetActive(false);
    }

    void ShowExplosion()
    {
        swordCollider.PlayExplosion();
    }
}
