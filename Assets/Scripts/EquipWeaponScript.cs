using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipWeaponScript : MonoBehaviour
{
    public PlayerController playerScript;
    public Transform player, swordContainer;
    public ShopManager shopManager;
    // Start is called before the first frame update
    void Start()
    {
        EquipWeapon();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void EquipWeapon()
    {
        transform.SetParent(swordContainer);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.Euler(Vector3.zero);
        transform.localScale = Vector3.one;
    }

    public void UnEquipWeapon()
    {
        transform.SetParent(null);
    }
}
