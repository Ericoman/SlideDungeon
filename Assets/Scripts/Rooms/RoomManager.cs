using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace Rooms
{
    public class RoomManager : MonoBehaviour
    {
        [SerializeField]
        private RoomDataSO roomDataSO;
        
        [Header("Requirements for room completion")]
        public GameObject[] enemies;
        public TeslaCoilPuzzleManager[] teslaCoils;
        
        [Space(5)]
        [Header("Doors")]
        [SerializeField]
        private GameObject[] roomDoors;
        [SerializeField]
        private float targetYPositionDoors = 10f; // desired y position
        [SerializeField]
        private float targetYRotationDoors = 90f; // desired Y rotation in degrees
        [SerializeField]
        private float moveSpeedDoors = 2f; // Speed
        
        [Space(5)]
        [Header("Torches")]
        public TorchComponent[] roomTorches;
        
        private RoomContext _currentContext;

        private Coroutine _moveDoorCoroutine;

        public event Action RoomCompleted;
        private void Awake()
        {
            _currentContext = new RoomContext();
            _currentContext.Initialize(roomDataSO.id,enemies.Length,teslaCoils.Length,false);
        }

        private void Start()
        {
            GameManager.Instance.RegisterRoom(this);
            foreach (GameObject enemy in enemies)
            {
                HealthComponent healthComponent = enemy.GetComponent<HealthComponent>();
                healthComponent.OnDeath += OnEnemyDied;
            }

            foreach (TeslaCoilPuzzleManager teslaCoil in teslaCoils)
            {
                teslaCoil.PuzzleCompleted += TeslaCoilOnPuzzleCompleted;
            }
        }

        private void OnDestroy()
        {
            foreach (GameObject enemy in enemies)
            {
                if(enemy == null) continue;
                HealthComponent healthComponent = enemy.GetComponent<HealthComponent>();
                healthComponent.OnDeath -= OnEnemyDied;
            }

            foreach (TeslaCoilPuzzleManager teslaCoil in teslaCoils)
            {
                if(teslaCoil == null) continue;
                teslaCoil.PuzzleCompleted -= TeslaCoilOnPuzzleCompleted;
            }
            
            GameManager.Instance.UnregisterRoom(this);
        }

        private void TeslaCoilOnPuzzleCompleted()
        {
            _currentContext.puzzlesLeft--;
        }

        private void OnEnemyDied()
        {
            _currentContext.enemiesLeft--;
        }

        private void Update()
        {
            if (!_currentContext.clearedOnce && IsCompleted())
            {
                CompleteRoom();
            }
        }
        
        private IEnumerator MoveDoor_CO(bool bUsePosition = false, bool bUseRotation = true)
        {
            if (bUsePosition)
            {
                yield return StartCoroutine(MoveDoorPosition_CO());
            }

            if (bUseRotation)
            {
                yield return StartCoroutine(MoveDoorRotation_CO());
            }
            _moveDoorCoroutine = null;
        }

        private IEnumerator MoveDoorPosition_CO()
        {
            bool isMoving = true;
            
            while (isMoving)
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
                        puzzleDoor.transform.position =
                            new Vector3(currentPosition.x, targetYPositionDoors, currentPosition.z);
                        isMoving = false;
                    }
                }

                yield return null;
            }
        }
        private IEnumerator MoveDoorRotation_CO()
        {
            bool isMoving = true;
            if (GetComponent<AudioComponent>())
            {
                GetComponent<AudioComponent>().Play();
            }
            while (isMoving)
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
                    }
                }

                yield return null;
            }
        }
        private void TurnOnTorches() 
        { 
            foreach(TorchComponent torch in roomTorches) 
            {
                torch.LightFire(true);
            }
        }
        public bool IsCompleted()
        {
            if (_currentContext.clearedOnce || roomDataSO.conditions == null)
            {
                return true;
            }
            
            for (int i = 0; i < roomDataSO.conditions.Length; ++i)
            {
                if (!roomDataSO.conditions[i].IsCompleted(_currentContext))
                {
                    return false;
                }
            }
            return true;
        }

        private void CompleteRoom()
        {
            _currentContext.clearedOnce = true;
            if (_moveDoorCoroutine == null)
            {
                _moveDoorCoroutine = StartCoroutine(MoveDoor_CO());
            }
            
            TurnOnTorches();
            RoomCompleted?.Invoke();
        }

        public RoomContext GetCurrentContext()
        {
            return _currentContext;
        }

        public void SetContext(RoomContext roomContext)
        {
            _currentContext = roomContext;
            if (IsCompleted())
            {
                CompleteRoom();
            }
        }

        public RoomDataSO GetRoomDataSO()
        {
            return roomDataSO;
        }

        public void SetRoomDataSO(RoomDataSO newRoomData)
        {
            roomDataSO = newRoomData;
            if (_currentContext != null)
            {
                _currentContext.roomId = roomDataSO.id;
            }
        }

        public void SetValuesFromOldRoomManager(global::RoomManager oldRoomManager)
        {
            targetYRotationDoors = oldRoomManager.targetYRotationDoors;
            moveSpeedDoors = oldRoomManager.moveSpeedDoors;
            
            roomDoors = oldRoomManager.roomDoors;

            if (oldRoomManager.teslaCoils.Length > 0)
            {
                TeslaCoilPuzzleManager teslaCoilPuzzleManager = new GameObject("TeslaCoilPuzzleManager").AddComponent<TeslaCoilPuzzleManager>();
                
                teslaCoilPuzzleManager.transform.SetParent(oldRoomManager.teslaCoils[0].transform.parent);
                teslaCoilPuzzleManager.teslaCoils = oldRoomManager.teslaCoils;
                teslaCoils = new []{teslaCoilPuzzleManager};
            }
            
            List<GameObject> oldRoomManagerEnemies = new List<GameObject>();
            foreach (PatrolController patrol in oldRoomManager.patrolEnemies)
            {
                oldRoomManagerEnemies.Add(patrol.gameObject);
            }

            foreach (ShooterController shooter in oldRoomManager.shooterEnemies)
            {
                oldRoomManagerEnemies.Add(shooter.gameObject);
            }
            enemies = oldRoomManagerEnemies.ToArray();
            
            List<TorchComponent> torchComponents = new List<TorchComponent>();
            foreach (GameObject torch in oldRoomManager.roomTorches)
            {
                TorchComponent torchComponent = torch.GetComponent<TorchComponent>();
                if (torchComponent != null)
                {
                    torchComponents.Add(torchComponent);
                }
            }
            roomTorches = torchComponents.ToArray();
        }
    }
}
