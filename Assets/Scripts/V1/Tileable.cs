using System;
using UnityEngine;

[ExecuteInEditMode]
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
    
    private bool isInGrid = false;
    private Vector2Int gridPosition = new Vector2Int(-1,-1);
    private Vector2Int lastGridPosition = new Vector2Int(-1,-1);
    public Vector2Int GridPosition => gridPosition;
    public Vector2Int LastGridPosition => lastGridPosition;
    public GridManager GridManager => gridManager;

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
        lastGridPosition = gridPosition;
        gridPosition = new Vector2Int(-1,-1);
        transform.parent = null;
        isInGrid = false;
    }

    public void ResetToPreviousGrid()
    {
        if (gridManager != null)
        {
            SetInGrid(gridManager);
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
    public void SetInGrid(GridManager grid)
    {
        gridManager = grid;
        gridPosition = gridManager.SetInGrid(this);
        lastGridPosition = gridPosition;
        if (gridPosition.x >= 0 && gridPosition.y >= 0)
        {
            transform.parent = gridManager.transform;
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
        transform.localPosition = gridManager.GridToWorld(gridPosition);  
    }

    private void OnDestroy()
    {
        gridManager.DeleteFromGrid(this);
    }

    public bool TryMove(Vector3 newPosition)
    {
        Vector2Int newPositionGrid = gridManager.GetNextAvailableCoordinates(widthTiles,heightTiles,gridManager.WorldToGrid(newPosition));
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
}
