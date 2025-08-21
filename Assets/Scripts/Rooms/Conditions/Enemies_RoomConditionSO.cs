using UnityEngine;

namespace Rooms.Conditions
{
    [CreateAssetMenu(fileName = "Enemies_RoomConditionSO", menuName = "Scriptable Objects/Enemies_RoomConditionSO")]
    public class Enemies_RoomConditionSO : RoomConditionSO
    {
        [SerializeField] 
        private int enemiesToKill = -1; 
        public int EnemiesToKill => enemiesToKill; 
        public override bool IsCompleted(RoomContext context)
        {
            if (EnemiesToKill < 0)
            {
                if (context.enemiesLeft <= 0)
                {
                    return true;
                }

                return false;
            }
            if (context.totalEnemies - context.enemiesLeft >= EnemiesToKill)
            {
                return true;
            }
            return false;
        }
    }
}
