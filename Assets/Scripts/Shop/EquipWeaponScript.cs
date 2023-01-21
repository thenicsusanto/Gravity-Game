using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipWeaponScript : MonoBehaviour
{
    public PlayerController playerScript;
    // Start is called before the first frame update
    void Start()
    {
        Invoke("EquipWeapon", 0.2f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void EquipWeapon()
    {
        transform.SetParent(GameManager.Instance.swordContainer.transform);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.Euler(Vector3.zero);
        transform.localScale = Vector3.one;
    }

    public void UnEquipWeapon()
    {
        Destroy(gameObject);
    }
}
