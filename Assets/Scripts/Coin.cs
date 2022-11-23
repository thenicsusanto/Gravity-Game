using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour
{
    public float expForce;
    public float radius;
    private Rigidbody rb;
    public ShopManager shopManager;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.AddExplosionForce(expForce, transform.position, radius);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            GameManager.Instance.coins++;
            shopManager.CheckPurchasable();
        }
    }
}
