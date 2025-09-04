using System;
using Grid.DataObjects;
using UnityEngine;
using UnityEngine.UIElements;

// [ExecuteInEditMode]
public class Tileable : MonoBehaviour
{
    [SerializeField] 
    private int widthTiles = 1;
    [SerializeField] 
    private int heightTiles = 1;

    [SerializeField]
    private GridManager gridManager;
    public int WidthTiles => widthTiles;
    public int HeightTiles => heightTiles;

    private int originalLayer;
    private bool isInGrid = false;
    private Vector2Int lastGridPosition = new Vector2Int(-1,-1);
    public Vector2Int GridPosition
    {
        get => TileContext.tilePosition;
        set => TileContext.tilePosition = value;
    }

    public Vector2Int LastGridPosition => lastGridPosition;
    public GridManager GridManager => gridManager;
    
    private TileContext _tileContext;
    public TileContext TileContext => _tileContext;
    public int TileId
    {
        get => TileContext.tileId;
        set => TileContext.tileId = value;
    }

    public bool CanBeMoved => _canBeMoved;
    private bool _canBeMoved;

    private void Awake()
    {
        _tileContext = new TileContext();
    }

    private void Start()
    {
        originalLayer = gameObject.layer;
    }

    private void Update()
    {

        if (isInGrid)
        {
            SnapToGrid();
        }
        
    }

    public void RemoveFromGrid()
    {
        gridManager.DeleteFromGrid(this);
        lastGridPosition = GridPosition;
        GridPosition = new Vector2Int(-1,-1);
        transform.parent = null;
        
        //Removed since we are not using mouse raycasts anymore
        // transform.gameObject.layer = originalLayer;
        // foreach (Transform child in transform)
        // {
        //     child.gameObject.layer = originalLayer;
        // }
        
        isInGrid = false;
    }

    public void ResetToPreviousGrid()
    {
        if (gridManager != null)
        {
            SetInGrid(gridManager,LastGridPosition);
        }
        else
        {
#if UNITY_EDITOR
            DestroyImmediate(gameObject);
#else
            Destroy(gameObject);
#endif
        }
    }

    public void SetInGrid(GridManager gridManager,int tileIndex)
    {
        TileId = tileIndex;
        SetInGrid(gridManager, new Vector2Int(-1,-1));
    }
    public void SetInGrid(GridManager grid,Vector2Int preferredGridPosition)
    {
        gridManager = grid;
        GridPosition = gridManager.SetInGrid(this,preferredGridPosition);
        lastGridPosition = GridPosition;
        if (GridPosition.x >= 0 && GridPosition.y >= 0)
        {
            transform.parent = gridManager.transform;
            transform.localRotation = Quaternion.identity;
            
            //Removed since we are not using mouse raycasts anymore
            // transform.gameObject.layer = gridManager.gameObject.layer;
            // foreach (Transform child in transform)
            // {
            //     if(child.gameObject.layer != LayerMask.NameToLayer("Interactable")) child.gameObject.layer = gridManager.gameObject.layer;
            // }
            
            isInGrid = true;
        }
        else
        {
#if UNITY_EDITOR
            DestroyImmediate(gameObject);
#else
            Destroy(gameObject);
#endif
        }
    }
    private void SnapToGrid()
    {
        transform.position = gridManager.GridToWorld(GridPosition);  
    }

    private void OnDestroy()
    {
        if (gridManager)
        {
            gridManager.DeleteFromGrid(this);
        }
    }

    public bool TryMove(Vector3 newPosition)
    {
        Vector2Int newPositionGrid = gridManager.GetNextAvailableCoordinates(widthTiles,heightTiles,gridManager.WorldToGrid(newPosition));
        Debug.Log("TRYMOVE COORDINATES: "+newPositionGrid.x + " and " + newPositionGrid.y);
        if (Mathf.Abs(newPositionGrid.x - lastGridPosition.x) == 1 && newPositionGrid.y == lastGridPosition.y ||
            Mathf.Abs(newPositionGrid.y - lastGridPosition.y) == 1 && newPositionGrid.x == lastGridPosition.x )
        {
            transform.position = gridManager.GridToWorld(newPositionGrid);
            lastGridPosition = newPositionGrid;
            return true;
        }
        transform.position = gridManager.GridToWorld(lastGridPosition);
        return false;
    }
    public bool TryMove(Vector2Int newGridPosition)
    {
        Vector2Int newPositionGrid = gridManager.GetNextAvailableCoordinates(widthTiles,heightTiles,newGridPosition);
        if (Mathf.Abs(newPositionGrid.x - lastGridPosition.x) == 1 && newPositionGrid.y == lastGridPosition.y ||
            Mathf.Abs(newPositionGrid.y - lastGridPosition.y) == 1 && newPositionGrid.x == lastGridPosition.x )
        {
            transform.position = gridManager.GridToWorld(newPositionGrid);
            lastGridPosition = newPositionGrid;
            return true;
        }
        transform.position = gridManager.GridToWorld(lastGridPosition);
        return false;
    }

    public void SetContext(TileContext context)
    {
        _tileContext = context;
    }

    public void EnableMovement()
    {
        _canBeMoved = true;
    }
}
