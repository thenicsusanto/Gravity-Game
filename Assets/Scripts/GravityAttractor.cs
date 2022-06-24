using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityAttractor : MonoBehaviour
{
    public float gravity = -10f;

    public void Attract(Transform body)
    {
        // set planet gravity direction for the object body
        Vector3 gravityDir = (body.position - transform.position).normalized;
        Vector3 bodyUp = body.up;
        // apply gravity to objects rigidbody
        body.GetComponent<Rigidbody>().AddForce(gravityDir * gravity);
        // update the objects rotation in relation to the planet
        Quaternion targetRotation = Quaternion.FromToRotation(bodyUp, gravityDir) * body.rotation;
        // smooth rotation
        body.rotation = Quaternion.Slerp(body.rotation, targetRotation, 50 * Time.deltaTime);
    }
}
