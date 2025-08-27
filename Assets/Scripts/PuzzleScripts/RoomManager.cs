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

    [Space(5)]
    [Header("Torches")]
    public GameObject[] roomTorches;


    private bool _roomCompleted = false;
    private bool _onceCompleted = false;

    void Update()
    {
        if (!_roomCompleted && CheckedRequirements())
        {
            if (!_onceCompleted) 
            {
                _onceCompleted = true;
                TurnOnTorches();
            }

            MoveDoorToTarget();
        }
    }

    public bool CheckedRequirements()
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
        if (!_roomCompleted)
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
                    _roomCompleted = true;
                }
            }
        }
    }

    private void TurnOnTorches() 
    { 
        foreach(GameObject torch in roomTorches) 
        {
            GameObject fire = torch.transform.Find("Fire").gameObject;
            if (fire) fire.SetActive(true);
        }
    }
}
