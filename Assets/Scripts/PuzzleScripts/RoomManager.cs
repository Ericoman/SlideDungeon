using UnityEngine;

public class RoomManager : MonoBehaviour
{
    [Header("Requirements for room completion")]
    public PatrolController[] patrolEnemies;
    public TeslaCoil[] teslaCoils;

    [Space(5)]
    [Header("Doors")]
    public GameObject[] roomDoors;
    public float targetYPositionDoors = 10f; // desired y position
    public float moveSpeedDoors = 2f; // Speed

    private bool isMoving = false;

    void Start()
    {
        
    }

    void Update()
    {
        if (isMoving)
        {
            foreach (GameObject puzzleDoor in roomDoors) 
            {
                // Get the door's current position
                Vector3 currentPosition = puzzleDoor.transform.position;

                // Lerp to smoothly move toward the target Y position
                float newY = Mathf.Lerp(currentPosition.y, targetYPositionDoors, moveSpeedDoors * Time.deltaTime);

                // Update the door's position with the new Y value
                puzzleDoor.transform.position = new Vector3(currentPosition.x, newY, currentPosition.z);

                // Stop moving if the door has reached the target position
                if (Mathf.Abs(currentPosition.y - targetYPositionDoors) < 0.01f)
                {
                    puzzleDoor.transform.position = new Vector3(currentPosition.x, targetYPositionDoors, currentPosition.z);
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
            if (!coil.isActive)
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
