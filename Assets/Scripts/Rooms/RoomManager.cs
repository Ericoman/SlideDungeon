using System;
using System.Collections;
using UnityEngine;

namespace Rooms
{
    public class RoomManager : MonoBehaviour
    {
        [SerializeField]
        private RoomDataSO roomDataSO;
        
        [Header("Requirements for room completion")]
        public PatrolController[] patrolEnemies;
        public TeslaCoilPuzzleManager[] teslaCoils;
        
        [Header("Room Entities")]
        [SerializeField]
        private GameObject[] roomDoors;
        [SerializeField]
        private float targetYPositionDoors = 10f; // desired y position
        [SerializeField]
        private float moveSpeedDoors = 2f; // Speed
        
        private RoomContext _currentContext;

        private Coroutine _moveDoorCoroutine;
        private void Awake()
        {
            _currentContext = new RoomContext();
            _currentContext.Initialize(roomDataSO.id,patrolEnemies.Length,teslaCoils.Length,false);
        }

        private void Start()
        {
            GameManager.Instance.RegisterRoom(this);
            foreach (PatrolController enemy in patrolEnemies)
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
            foreach (PatrolController enemy in patrolEnemies)
            {
                HealthComponent healthComponent = enemy.GetComponent<HealthComponent>();
                healthComponent.OnDeath -= OnEnemyDied;
            }

            foreach (TeslaCoilPuzzleManager teslaCoil in teslaCoils)
            {
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
            if (IsCompleted() && _moveDoorCoroutine == null)
            {
                _moveDoorCoroutine = StartCoroutine(MoveDoor_CO());
            }
        }
        
        private IEnumerator MoveDoor_CO()
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
                        puzzleDoor.transform.position = new Vector3(currentPosition.x, targetYPositionDoors, currentPosition.z);
                        isMoving = false;

                        //TODO - AQUI SE ENCENDERIAN LAS ANTORCHAS; LUZ Y ESAS COSAS 

                    }
                }

                yield return null;
            }
            _moveDoorCoroutine = null;
        }
        public bool IsCompleted()
        {
            for (int i = 0; i < roomDataSO.conditions.Length; ++i)
            {
                if (!roomDataSO.conditions[i].IsCompleted(_currentContext))
                {
                    return false;
                }
            }
            _currentContext.clearedOnce = true;
            return true;
        }

        public RoomContext GetCurrentContext()
        {
            return _currentContext;
        }

        public void SetContext(RoomContext roomContext)
        {
            _currentContext = roomContext;
        }

        public RoomDataSO GetRoomDataSO()
        {
            return roomDataSO;
        }

        public void SetRoomDataSO(RoomDataSO newRoomData)
        {
            roomDataSO = newRoomData;
        }
    }
}
