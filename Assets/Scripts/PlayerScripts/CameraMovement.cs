using System;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public Transform player; // Reference to the player's transform
    public float smoothSpeed = 5f; // Speed at which the camera smooths its movement
    public Vector3 offset = new Vector3(0f, 10f, -10f); // Offset between the player and the camera

    private Vector3 velocity = Vector3.zero;
    private Quaternion originalLocalRotation;

    private void Awake()
    {
        originalLocalRotation = transform.localRotation;
    }

    void LateUpdate()
    {
        if (player == null)
        {
            Debug.LogError("Player reference is missing in CameraMovement. Assign the player in the inspector.");
            return;
        }
        if (!GameManager.Instance.movingCamera)
        {
            Vector3 desiredPosition = player.position + offset;

            // Smoothly move the camera to the desired position
            transform.position = Vector3.SmoothDamp(transform.position, desiredPosition, ref velocity, 1f / smoothSpeed);
            transform.localRotation = originalLocalRotation;
        }
        // Calculate the desired camera position based on the player's position and the offset
       
    }
}
