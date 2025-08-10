using System;
using System.Collections.Generic;
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
        
        foreach (OriginalTile originalTile in originalTiles)
        {
            originalTilesDictionary.Add(originalTile.tileIndex,originalTile.tile);
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
        }
        else
        {
            GameObject tile = Instantiate(tilePrefabs[tileIndex],gridManager.GridToWorld(generatedtile.GridPosition),Quaternion.identity);
            //ONLY TO TEST
            // tile.transform.localScale *= gridManager.CellSize;
            //
            tileable = tile.GetComponent<Tileable>();
            tileable.SetInGrid(gridManager,generatedtile.GridPosition);
        }
    }

    private void OnDestroy()
    {
        mirroredTileInstancer.InstantiateTileEvent -= MirroredTileInstancerOnInstantiateTileEvent;
        mirroredTileInstancer.TileMovedEvent -= MirroredTileInstancerOnTileMovedEvent;
    }
}
