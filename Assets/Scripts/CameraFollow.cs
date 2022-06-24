using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    // inspector variables
    [SerializeField, Tooltip("Player transform for camera to follow")]
    private Transform playerTransform;
    [SerializeField, Tooltip("Camera offset from player (x not used)")]
    private Vector3 offsetPosition = new Vector3(0, 5, 5);
    [SerializeField]
    private bool lookAt = true;

    // privates
    private Transform viewCamera;

    // Use this for initialization
    private void Start()
    {
        viewCamera = Camera.main.transform;
    }

    // Update is called once per frame
    private void LateUpdate()
    {
        UpdateCamera();
    }

    /// <summary>
    /// Update camera position and rotation
    /// </summary>
    private void UpdateCamera()
    {
        if (playerTransform == null)
        {
            return;
        }
        // camera rig position
        transform.position = playerTransform.position + -(playerTransform.forward * offsetPosition.z) + (playerTransform.up * offsetPosition.y);
        // point camera at player
        if (lookAt)
        {
            // point camera at player using players up direction
            viewCamera.LookAt(playerTransform, playerTransform.up);
        }
    }
}
