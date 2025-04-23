using System;
using UnityEngine;

public class MirrorTileManager : MonoBehaviour
{
    [SerializeField] 
    GridManager gridManager;
    [SerializeField]
    TileInstancer mirroredTileInstancer;
    [SerializeField]
    GameObject[] tilePrefabs;

    void Awake()
    {
        mirroredTileInstancer.InstantiateTileEvent += MirroredTileInstancerOnInstantiateTileEvent;
        mirroredTileInstancer.TileMovedEvent += MirroredTileInstancerOnTileMovedEvent;
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
        GameObject tile = Instantiate(tilePrefabs[tileIndex],gridManager.GridToWorld(generatedtile.GridPosition),Quaternion.identity);
        //ONLY TO TEST
        // tile.transform.localScale *= gridManager.CellSize;
        //
        Tileable tileable = tile.GetComponent<Tileable>();
        tileable.SetInGrid(gridManager,generatedtile.GridPosition);
    }

    private void OnDestroy()
    {
        mirroredTileInstancer.InstantiateTileEvent -= MirroredTileInstancerOnInstantiateTileEvent;
        mirroredTileInstancer.TileMovedEvent -= MirroredTileInstancerOnTileMovedEvent;
    }
}
