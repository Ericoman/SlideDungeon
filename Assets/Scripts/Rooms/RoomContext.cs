using System;

namespace Rooms
{
    [Serializable]
    public class RoomContext
    {
        public int roomId;
        public bool clearedOnce;
        public int enemiesLeft;
        public int totalEnemies;
        public int puzzlesLeft;
        public int totalPuzzles;

        public void Initialize(int roomID, int initialEnemies, int initialPuzzles, bool initialClearedOnce = false)
        { 
            roomId = roomID; 
            totalEnemies = enemiesLeft = initialEnemies;
            totalPuzzles = puzzlesLeft = initialPuzzles;
            clearedOnce = initialClearedOnce;
        }
    }
}
