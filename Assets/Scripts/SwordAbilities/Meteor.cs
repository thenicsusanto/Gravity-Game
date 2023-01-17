using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Meteor : MonoBehaviour
{
    public float speed;
    public GameObject impactPrefab;

    [SerializeField]
    private float rotationSpeed = 1000;

    public Transform target;

    private Rigidbody rb;
    public GameObject planet;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        Vector3 targetDirection = target.position - transform.position;

        Vector3 newDirection = Vector3.RotateTowards(transform.forward, targetDirection, rotationSpeed * Time.deltaTime, 0.0F);

        transform.Translate(Vector3.forward * Time.deltaTime * speed, Space.Self);
        transform.rotation = Quaternion.LookRotation(newDirection);  
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Planet"))
        {
            speed = 0;
            ContactPoint contact = collision.GetContact(0);
            Quaternion rot = Quaternion.FromToRotation(Vector3.up, contact.normal);
            Vector3 pos = contact.point;

            if (impactPrefab != null)
            {
                GameObject impactVFX = Instantiate(impactPrefab, pos, rot);
                if(target.GetComponent<MeleeEnemyController>() == true)
                {
                    target.GetComponent<MeleeEnemyController>().MeleeEnemyTakeDamage(150);
                    if (target.GetComponent<MeleeEnemyController>().currentHealthEnemy > 0)
                    {
                        target.GetComponent<MeleeEnemyController>().PlayBurnEnemy(5, 10);
                    }
                }
                else if(target.GetComponent<RangedEnemyController>() == true)
                {
                    target.GetComponent<RangedEnemyController>().RangedEnemyTakeDamage(150);
                    if (target.GetComponent<RangedEnemyController>().currentHealthEnemy > 0)
                    {
                        target.GetComponent<RangedEnemyController>().PlayBurnEnemy(5, 10);
                    }
                }
                
                Destroy(impactVFX, 5);
            }

            Destroy(gameObject);
        }
    }

    IEnumerator CheckMeleeHealth()
    {
        yield return new WaitForSeconds(0.1f);
        
    }

    IEnumerator CheckRangedHealth()
    {
        yield return new WaitForSeconds(0.1f);
        if (target.GetComponent<RangedEnemyController>().currentHealthEnemy > 0)
        {
            target.GetComponent<RangedEnemyController>().PlayBurnEnemy(5, 10);
        }
    }
}
