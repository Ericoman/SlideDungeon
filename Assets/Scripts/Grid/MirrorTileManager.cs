using System;
using System.Collections.Generic;
using System.Linq;
using Grid.DataObjects;
using Rooms;
using UnityEngine;

public class MirrorTileManager : MonoBehaviour
{
    [SerializeField] 
    GridManager gridManager;
    [SerializeField]
    TileInstancer mirroredTileInstancer;
    [SerializeField]
    GameObject[] tilePrefabs;
    [SerializeField]
    OriginalTile[] originalTiles;
    
    [SerializeField]
    RoomDataSO[] roomDatas;
    
    [Serializable]
    struct OriginalTile
    {
        public Tileable tile;
        public int tileIndex;
    }
    
    private Dictionary<int,Tileable> originalTilesDictionary = new Dictionary<int, Tileable>();

    void Awake()
    {
        mirroredTileInstancer.InstantiateTileEvent += MirroredTileInstancerOnInstantiateTileEvent;
        mirroredTileInstancer.TileMovedEvent += MirroredTileInstancerOnTileMovedEvent;
        mirroredTileInstancer.TilesLoadedEvent += MirroredTileInstancerOnTilesLoadedEvent;
        
        foreach (OriginalTile originalTile in originalTiles)
        {
            originalTilesDictionary.Add(originalTile.tileIndex,originalTile.tile);
        }
    }

    private void MirroredTileInstancerOnTilesLoadedEvent(TileContext[] tileContexts)
    {
        Tileable[] tiles = gridManager.GetIndividualTilesInGrid();
        
        foreach (Tileable tileable in tiles)
        {
            tileable.RemoveFromGrid();
        }
        foreach (Tileable tileable in tiles)
        {
            Vector2Int newGridPosition = tileable.LastGridPosition;
            TileContext tileContext = tileContexts.FirstOrDefault(x=>x.tileId == tileable.TileId);
            if (tileContext != null && tileable.TileId == tileContext.tileId)
            {
                newGridPosition = tileContext.tilePosition;
            }
            tileable.SetInGrid(gridManager,newGridPosition);
        }
    }

    private void MirroredTileInstancerOnTileMovedEvent(object sender, Vector2Int newGridPosition, Vector2Int oldGridPosition)
    {
        Tileable mirrorTile = gridManager.GetTile(oldGridPosition);
        mirrorTile.RemoveFromGrid();
        mirrorTile.TryMove(newGridPosition);
        mirrorTile.ResetToPreviousGrid();
    }

    private void MirroredTileInstancerOnInstantiateTileEvent(object sender, int tileIndex, Tileable generatedtile)
    {
        Tileable tileable;
        if (originalTilesDictionary.TryGetValue(tileIndex, out tileable))
        {
            tileable.SetInGrid(gridManager,generatedtile.GridPosition);
            Rooms.RoomManager roomManager = tileable.GetComponentInChildren<Rooms.RoomManager>();
            if (roomManager != null)
            {
                roomManager.RoomCompleted += generatedtile.EnableMovement;
                if (roomManager.IsCompleted())
                {
                    generatedtile.EnableMovement();
                }
            }
        }
        else
        {
            GameObject tile = Instantiate(tilePrefabs[tileIndex],gridManager.GridToWorld(generatedtile.GridPosition),Quaternion.identity);
            //ONLY TO TEST
            // tile.transform.localScale *= gridManager.CellSize;
            //
            tileable = tile.GetComponentInChildren<Tileable>();
            tileable.SetInGrid(gridManager,generatedtile.GridPosition);
            
            Rooms.RoomManager roomManager = tile.GetComponentInChildren<Rooms.RoomManager>();
            if (roomManager != null && roomDatas.Length > tileIndex)
            {
                roomManager.RoomCompleted += generatedtile.EnableMovement;
                roomManager.SetRoomDataSO(roomDatas[tileIndex]);
            }
        }
        tileable.TileId = tileIndex;
    }

    private void OnDestroy()
    {
        mirroredTileInstancer.InstantiateTileEvent -= MirroredTileInstancerOnInstantiateTileEvent;
        mirroredTileInstancer.TileMovedEvent -= MirroredTileInstancerOnTileMovedEvent;
        mirroredTileInstancer.TilesLoadedEvent -= MirroredTileInstancerOnTilesLoadedEvent;
    }
}
