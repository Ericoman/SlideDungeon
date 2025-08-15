using UnityEngine;

namespace Rooms.Conditions
{
    [CreateAssetMenu(fileName = "RoomConditionSO", menuName = "Scriptable Objects/RoomConditionSO")]
    public abstract class RoomConditionSO : ScriptableObject
    {
        [SerializeField] private string description;
        public string Description => description;
        
        public abstract bool IsCompleted(RoomContext context);
    }
}
