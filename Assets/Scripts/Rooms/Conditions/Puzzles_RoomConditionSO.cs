using UnityEngine;

namespace Rooms.Conditions
{
    [CreateAssetMenu(fileName = "Puzzles_RoomConditionSO", menuName = "Scriptable Objects/Puzzles_RoomConditionSO")]
    public class Puzzles_RoomConditionSO : RoomConditionSO
    {
        [SerializeField] 
        private int puzzlesToSolve = -1;
        public int PuzzlesToSolve => puzzlesToSolve;
        public override bool IsCompleted(RoomContext context)
        {
            if (PuzzlesToSolve < 0)
            {
                if (context.puzzlesLeft <= 0)
                {
                    return true;
                }
                return false;
            }

            if (context.totalPuzzles - context.puzzlesLeft >= PuzzlesToSolve)
            {
                return true;
            }
            return false;
        }
    }
}
