using System;
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
        
        private RoomContext _currentContext;

        private void Awake()
        {
            _currentContext = new RoomContext();
            _currentContext.Initialize(roomDataSO.id,patrolEnemies.Length,teslaCoils.Length,false);
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
    }
}
