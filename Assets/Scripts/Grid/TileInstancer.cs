using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Grid.DataObjects;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.UI.Image;

public class TileInstancer : MonoBehaviour
{
    [Serializable]
    struct DefaultTile
    {
        public Transform tile;
        public int tileIndex;
        public bool keepOriginal;
    }
    
    [SerializeField]
    GameObject[] tilePrefabs;

    [SerializeField]
    private DefaultTile[] _defaultTiles;

    [SerializeField]
    private GridManager gridManager;
    
    protected LayerMask raycastMask = ~(1<<2);

    public delegate void InstantiateTileEventHandler(object sender, int tileIndex,Tileable generatedTile);
    public event InstantiateTileEventHandler InstantiateTileEvent;
    public delegate void TileMovedEventHandler(object sender,Vector2Int newGridPosition, Vector2Int oldGridPosition);
    public event TileMovedEventHandler TileMovedEvent;
    
    public event Action<TileContext[]> TilesLoadedEvent;

    // private void Start()
    // {
    //     raycastMask = ~LayerMask.GetMask("Ignore Raycast");
    // }

    private void Start()
    {
        if (!gridManager)
        {
            Debug.LogError("Grid Manager not set");
            return;
        }
        foreach (DefaultTile tile in _defaultTiles)
        {
            if (tile.keepOriginal)
            {
                SetTileOnGrid(gridManager, tile.tileIndex, tile.tile.gameObject);
            }
            else
            {
                Tileable newTile = InstantiateTileOnGrid(gridManager, tile.tile.position, tile.tileIndex).GetComponentInChildren<Tileable>();
                Destroy(tile.tile.gameObject);
            }
        }
    }
    private void Update()
    {
        // if (Input.GetMouseButtonDown(0))
        // {
        //     Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        //     Debug.DrawRay(ray.origin, ray.direction*10000, Color.red, 10f);
        //     if (Physics.Raycast(ray, out RaycastHit hit,1000,raycastMask))
        //     {
        //         Tileable tileable = hit.collider.GetComponentInParent<Tileable>();
        //         if (tileable != null)
        //         {
        //             MoveTile(tileable);
        //         }
        //         else
        //         {
        //             int tileIndex = 0;
        //             if (Input.GetKey(KeyCode.Alpha1))
        //             {
        //                 tileIndex = 1;
        //             }    
        //             else if (Input.GetKey(KeyCode.Alpha2))
        //             {
        //                 tileIndex = 2;
        //             }
        //             else if (Input.GetKey(KeyCode.Alpha3))
        //             {
        //                 tileIndex = 3;
        //             }
        //             
        //             GridManager gridManager = hit.collider.gameObject.GetComponentInParent<GridManager>();
        //             //Debug.Log(hit.point);
        //             GameObject tile = InstantiateTileOnGrid(gridManager,hit.point,tileIndex);
        //             if (tile == null)
        //             {
        //                 Debug.LogWarning("Tile not instantiated, no valid grid found");
        //             }
        //         }
        //     }
        //
        //     
        // }
    }

    [Obsolete("Use NewMoveTile(Tileable tile, Vector2Int direction) instead")]
    void MoveTile(Tileable tile)
    {
        tile.RemoveFromGrid();
        StartCoroutine(MoveTile_CO(tile));
    }
    [Obsolete("Use NewMoveTile(Tileable tile, Vector2Int direction) instead")]
    public void NewMoveTile(Tileable tile, Vector3 newPosition)
    {
        tile.RemoveFromGrid();
        Vector2Int oldGridPosition = tile.LastGridPosition;

        if (tile.TryMove(newPosition))
        {
            
            TileMovedEvent?.Invoke(this, tile.LastGridPosition, oldGridPosition);
        }
        tile.ResetToPreviousGrid();
    }
    public void NewMoveTile(Tileable tile, Vector2Int direction)
    {
        tile.RemoveFromGrid();
        Vector2Int oldGridPosition = tile.LastGridPosition;

        if (tile.TryMove(oldGridPosition + direction))
        {
            
            TileMovedEvent?.Invoke(this, tile.LastGridPosition, oldGridPosition);
        }
        tile.ResetToPreviousGrid();
    }

    IEnumerator MoveTile_CO(Tileable tile)
    {
        bool moving = true;
        Plane groundPlane = new Plane(Vector3.up, Vector3.zero); // Ground at Y=0
        while (moving)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (groundPlane.Raycast(ray, out float enter))
            {
                Vector3 newPosition = ray.GetPoint(enter);

                Vector2Int oldGridPosition = tile.LastGridPosition;
                if (tile.TryMove(newPosition))
                {
                    TileMovedEvent?.Invoke(this, tile.LastGridPosition, oldGridPosition);
                }
            }

            if (Input.GetMouseButtonUp(0))
            {
                tile.ResetToPreviousGrid();
                moving = false;
            }

            yield return null;
        }
    }
    public GameObject InstantiateTileOnGrid(GridManager gridManager, Vector3 position, int tileIndex)
    {
        if (gridManager)
        {
            //Debug.Log(position);
            GameObject tile = Instantiate(tilePrefabs[tileIndex],position,Quaternion.identity);
            Tileable tileable = tile.GetComponentInChildren<Tileable>();
            tileable.SetInGrid(gridManager,tileIndex);
            
            InstantiateTileEvent?.Invoke(this, tileIndex, tileable);
            return tile;
        }
    
        return null;
    }

    public void SetTileOnGrid(GridManager gridManager, int tileIndex, GameObject tile)
    {
        if (gridManager)
        {   
            Tileable tileable = tile.GetComponentInChildren<Tileable>();
            tileable.SetInGrid(gridManager,tileIndex);
            
            InstantiateTileEvent?.Invoke(this, tileIndex, tileable);
        }
    }

    public TileContext[] GetTileContexts()
    {
        return gridManager.GetIndividualTileContexts();
    }

    public void SetTilecontexts(TileContext[] tileContexts)
    {
        Tileable[] tiles = gridManager.GetIndividualTilesInGrid();
        
        foreach (Tileable tileable in tiles)
        {
            Vector2Int newGridPosition = tileable.GridPosition;
            tileable.RemoveFromGrid();
            TileContext tileContext = tileContexts.FirstOrDefault(x=>x.tileId == tileable.TileId);
            if (tileContext != null && tileable.TileId == tileContext.tileId)
            {
                tileable.SetContext(tileContext);
                newGridPosition = tileContext.tilePosition;
            }
            tileable.SetInGrid(gridManager,newGridPosition);
        }
        TilesLoadedEvent?.Invoke(tileContexts);
    }
}
