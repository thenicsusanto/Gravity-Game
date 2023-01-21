using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// An object that will move towards an object marked with the tag 'targetTag'. 
/// </summary>
public class HomingMissile : MonoBehaviour
{
    /// The base movement speed of the missile, in units per second. 
    [SerializeField]
    private float speed = 15;

    /// The base rotation speed of the missile, in radians per second. 
    [SerializeField]
    private float rotationSpeed = 1000;

    /// The distance at which this object stops following its target and continues on its last known trajectory. 
    [SerializeField]
    private float focusDistance = 7;

    /// The transform of the target object.
    private Transform target;

    public GameObject explosion;

    /// Returns true if the object should be looking at the target. 
    private bool isLookingAtObject = true;

    /// The tag of the target object.
    [SerializeField]
    private string targetTag;

    /// Error message.
    private string enterTagPls = "Please enter the tag of the object you'd like to target, in the field 'Target Tag' in the Inspector.";

    public int damage;

    private void Start()
    {
        if (targetTag == "")
        {
            Debug.LogError(enterTagPls);
            return;
        }

        target = GameObject.FindGameObjectWithTag(targetTag).transform;
        StartCoroutine(SelfDestruct());
    }

    void Update()
    {
        if (targetTag == "")
        {
            Debug.LogError(enterTagPls);
            return;
        }

        Vector3 targetDirection = target.position - transform.position;

        Vector3 newDirection = Vector3.RotateTowards(transform.forward, targetDirection, rotationSpeed * Time.deltaTime, 0.0F);

        transform.Translate(Vector3.forward * Time.deltaTime * speed, Space.Self);

        if (Vector3.Distance(transform.position, target.position) < focusDistance)
        {
            isLookingAtObject = false;
        }

        if (isLookingAtObject)
        {
            transform.rotation = Quaternion.LookRotation(newDirection);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<PlayerController>().TakeDamagePlayer(damage);
            GameObject newExplosion = Instantiate(explosion, transform.position, Quaternion.identity);
            Destroy(newExplosion, 2);
            Destroy(gameObject, 0.1f);
        }
    }

    IEnumerator SelfDestruct()
    {
        yield return new WaitForSeconds(4f);
        Destroy(gameObject);
    }
}