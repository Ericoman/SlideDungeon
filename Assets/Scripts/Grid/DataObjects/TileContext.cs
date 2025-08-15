using System;
using UnityEngine;

namespace Grid.DataObjects
{
    [Serializable]
    public class TileContext
    {
        public int tileId = -1;
        public Vector2Int tilePosition = new Vector2Int(-1,-1);
        
        public void Initialize(int tileID, Vector2Int preferredTilePosition)
        { 
            tileId = tileID;
            tilePosition = preferredTilePosition;
        }
    }
}
