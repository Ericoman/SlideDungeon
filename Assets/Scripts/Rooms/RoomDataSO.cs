using Rooms.Conditions;
using UnityEngine;

namespace Rooms
{
    [CreateAssetMenu(fileName = "RoomData", menuName = "Scriptable Objects/RoomData")]
    public class RoomDataSO : ScriptableObject
    {
        public int id;
        public RoomConditionSO[] conditions;
    }
}
