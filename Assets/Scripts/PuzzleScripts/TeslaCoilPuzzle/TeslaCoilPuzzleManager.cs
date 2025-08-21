using System;
using UnityEngine;

public class TeslaCoilPuzzleManager : MonoBehaviour
{
    public TeslaCoil[] teslaCoils;

    public GameObject puzzleDoor;
    
    public float targetYPosition = 10f; // The desired Y position for the door
    public float moveSpeed = 2f; // Speed at which the door moves toward the target position

    private bool isMoving = false; // Track if the door is in the process of moving

    private bool _isCompleted = false;
    public bool IsCompleted => _isCompleted;
    
    public event Action PuzzleCompleted;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        foreach (TeslaCoil teslaCoil in teslaCoils)
        {
            teslaCoil.Activated += TeslaCoilOnActivated;
        }
    }

    private void TeslaCoilOnActivated(bool activated)
    {
        if (AreAllTeslasActive())
        {
            _isCompleted = true;
            PuzzleCompleted?.Invoke();
        }
    }

    // Update is called once per frame
    void Update()
    {
        // if (isMoving)
        // {
        //     // Get the door's current position
        //     Vector3 currentPosition = puzzleDoor.transform.position;
        //
        //     // Lerp to smoothly move toward the target Y position
        //     float newY = Mathf.Lerp(currentPosition.y, targetYPosition, moveSpeed * Time.deltaTime);
        //
        //     // Update the door's position with the new Y value
        //     puzzleDoor.transform.position = new Vector3(currentPosition.x, newY, currentPosition.z);
        //
        //     // Stop moving if the door has reached the target position
        //     if (Mathf.Abs(currentPosition.y - targetYPosition) < 0.01f)
        //     {
        //         puzzleDoor.transform.position = new Vector3(currentPosition.x, targetYPosition, currentPosition.z);
        //         isMoving = false;
        //     }
        // }
        // AreAllTeslasActive();

        // if (AreAllTeslasActive() == true)
        // {
        //     MoveDoorToTargetY();
        // }
    }
    
    public bool AreAllTeslasActive()
    {
        if (teslaCoils == null || teslaCoils.Length == 0)
        {
            return false; // No Tesla Coils to check
        }

        // Loop through all Tesla Coils and check their active state
        foreach (TeslaCoil coil in teslaCoils)
        {
            if (!coil.IsActive)
            {
                return false; // If any coil is not active, return false
            }
        }

        return true; // All Tesla Coils are active
    }
    
    // Call this function to start moving the door
    public void MoveDoorToTargetY()
    {
        isMoving = true;
    }
}
