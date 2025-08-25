using UnityEngine;

public class RoomManager : MonoBehaviour
{
    [Header("Requirements for room completion")]
    public PatrolController[] patrolEnemies;
    public TeslaCoil[] teslaCoils;

    [Space(5)]
    [Header("Doors")]
    public GameObject[] roomDoors;
    public float targetYRotationDoors = 90f; // desired Y rotation in degrees
    public float moveSpeedDoors = 2f; // Speed

    private bool isMoving = false;

    void Update()
    {
        if (isMoving)
        {
            foreach (GameObject puzzleDoor in roomDoors) 
            {
                // Get current rotation
                Quaternion currentRotation = puzzleDoor.transform.rotation;

                // Create target rotation
                Quaternion targetRotation = Quaternion.Euler(currentRotation.eulerAngles.x, targetYRotationDoors, currentRotation.eulerAngles.z);

                // Lerp to rotate toward target rotation
                puzzleDoor.transform.rotation = Quaternion.Lerp(currentRotation, targetRotation, moveSpeedDoors * Time.deltaTime);

                // Stop rotating if the door has reached the target rotation
                if (Quaternion.Angle(currentRotation, targetRotation) < 0.1f)
                {
                    puzzleDoor.transform.rotation = targetRotation;
                    isMoving = false;

                    //TODO - AQUI SE ENCENDERIAN LAS ANTORCHAS; LUZ Y ESAS COSAS
                }
            }            
        }

        if (IsRoomCmpleted() == true)
        {
            MoveDoorToTarget();
        }
    }

    public bool IsRoomCmpleted()
    {
        if ((teslaCoils == null && patrolEnemies == null) || (teslaCoils.Length == 0 && patrolEnemies.Length == 0))
        {
            return false; // No Tesla Coils or enemies 
        }

        foreach (TeslaCoil coil in teslaCoils)
        {
            if (!coil.IsActive)
            {
                return false; // If any coil is not active, return false
            }
        } 
        
        foreach (PatrolController enemy in patrolEnemies)
        {
            if (enemy != null)
            {
                return false; // enemy still alive
            }
        }

        return true; // All Tesla Coils are active
    }

    public void MoveDoorToTarget()
    {
        isMoving = true;
    }
}
