using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

[ExecuteInEditMode]
public class GridManager : MonoBehaviour
{
    
    [SerializeField] 
    private int width = 10;
    [SerializeField] 
    private int height = 10;
    [SerializeField]
    private float cellSize = 1f;
    
    [SerializeField]
    private GameObject backgroundTilePrefab;

    private int[,] grid;
    private Tileable[,] tiles;

    public int Width => width;
    public int Height => height;
    public float CellSize => cellSize;
    public void Awake()
    {
        grid = new int[width,height];
        tiles = new Tileable[width, height];
        GenerateBackgroundTiles();
    }
    
#if UNITY_EDITOR
    [SerializeField]
    Color gridColor = Color.magenta;
    private void OnDrawGizmosSelected()
    {
        DrawGrid();
    }
#endif
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Delete))
        {
            ClearGrid();
        }
    }

    private void GenerateBackgroundTiles()
    {
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                GameObject backgroundTile = Instantiate(backgroundTilePrefab, transform);
                backgroundTile.transform.localPosition = new Vector3(x*cellSize, y*cellSize, 1);
                backgroundTile.transform.localScale *= cellSize;
                backgroundTile.layer = gameObject.layer;
                foreach (Transform child in backgroundTile.transform)
                {
                    child.gameObject.layer = gameObject.layer;
                }
            }
        }
    }
    private void DrawGrid()
    {
        Gizmos.color = gridColor;

        for (int x = 0; x <= width; x++)
        {
            Vector3 start = new Vector3(x * cellSize, 0, 0);
            Vector3 end = new Vector3(x * cellSize, height * cellSize, 0);
            Gizmos.DrawLine(start + transform.position, end + transform.position);
        }

        for (int y = 0; y <= height; y++)
        {
            Vector3 start = new Vector3(0, y * cellSize, 0);
            Vector3 end = new Vector3(width * cellSize, y * cellSize, 0);
            Gizmos.DrawLine(start + transform.position, end + transform.position);
        }
    }
    public Vector2Int SetInGrid(Tileable tileable)
    {
        Vector2Int assignedCoordinates = GetNextAvailableCoordinates(tileable.WidthTiles, tileable.HeightTiles,WorldToGrid(tileable.transform.position));
        if (assignedCoordinates.x >= 0 && assignedCoordinates.y >= 0)
        {
            for (int x = 0; x < tileable.WidthTiles; ++x)
            {
                for (int y = 0; y < tileable.HeightTiles; ++y)
                {
                    grid[assignedCoordinates.x + x, assignedCoordinates.y + y] = 1;
                    tiles[assignedCoordinates.x + x, assignedCoordinates.y + y] = tileable;
                }
            }
        }
        return assignedCoordinates;
    }
    public void DeleteFromGrid(Tileable tileable)
    {
        Vector2Int assignedCoordinates = tileable.GridPosition;
        if (assignedCoordinates.x >= 0 && assignedCoordinates.y >= 0)
        {
            for (int x = 0; x < tileable.WidthTiles; ++x)
            {
                for (int y = 0; y < tileable.HeightTiles; ++y)
                {
                    grid[assignedCoordinates.x + x, assignedCoordinates.y + y] = 0;
                    tiles[assignedCoordinates.x + x, assignedCoordinates.y + y] = null;
                }
            }
        }
    }

    public void ClearGrid()
    {
        List<Transform> toDestroy = new List<Transform>();
        foreach (Transform child in transform)
        {
            toDestroy.Add(child);
        }

        foreach (Transform t in toDestroy)
        {
#if UNITY_EDITOR
            DestroyImmediate(t.gameObject);
#else
            Destroy(t.gameObject);
#endif
        }
        GenerateBackgroundTiles();
    }

    public Vector2Int GetNextAvailableCoordinates(int _width, int _height)
    {
        return GetNextAvailableCoordinates(_width,_height,new Vector2Int(-1,-1));
    }
    public Vector2Int GetNextAvailableCoordinates(int _width, int _height, Vector2Int _preferredCoordinates)
    {
        if (grid == null || grid.GetLength(0) != width || grid.GetLength(1) != height)
        {
            ClearGrid();
            grid = new int[width, height];
            tiles = new Tileable[width, height];
        }

        List<Vector2Int> candidates = new List<Vector2Int>();
        
        for (int y = 0; y <= height - _height; y++) // Ensure room for height
        {
            for (int x = 0; x <= width - _width; x++) // Ensure room for width
            {
                bool spaceAvailable = true;

                // Check if a tile of _width x _height can fit here
                for (int i = 0; i < _width && spaceAvailable; i++)
                {
                    for (int j = 0; j < _height; j++)
                    {
                        if (grid[x + i, y + j] != 0)
                        {
                            spaceAvailable = false;
                            break; // No need to check further
                        }
                    }
                }

                if (spaceAvailable)
                {
                    if (_preferredCoordinates.x < 0 || _preferredCoordinates.y < 0)
                    {
                        return new Vector2Int(x, y);
                    }
                    candidates.Add(new Vector2Int(x,y));
                }
            }
        }
        candidates.Sort((a, b) =>
        {
            int distA = Mathf.Abs(a.x - _preferredCoordinates.x) + Mathf.Abs(a.y - _preferredCoordinates.y);
            int distB = Mathf.Abs(b.x - _preferredCoordinates.x) + Mathf.Abs(b.y - _preferredCoordinates.y);
            return distA.CompareTo(distB);
        });
        return candidates.Count > 0 ? candidates[0] : new Vector2Int(-1, -1);
    }

    public Vector2Int WorldToGrid(Vector3 position)
    {
        int x = Mathf.FloorToInt((position.x -transform.position.x) / cellSize);
        int y = Mathf.FloorToInt((position.y - transform.position.y)/ cellSize);
        x = Mathf.FloorToInt(Mathf.Clamp(x, 0, width));
        y = Mathf.FloorToInt(Mathf.Clamp(y, 0, height));
        return new Vector2Int(x,y);
    }

    public Vector3 GridToWorld(Vector2Int gridPosition)
    {
        return new Vector3(gridPosition.x * cellSize+transform.position.x, gridPosition.y * cellSize+transform.position.y, transform.position.z);
    }

    public Tileable GetTile(Vector2Int gridPosition)
    {
        if (gridPosition.x > 0 && gridPosition.x < tiles.Length && gridPosition.y > 0 && gridPosition.y < tiles.Length)
        {
            return tiles[gridPosition.x, gridPosition.y];
        }
        return null;
    }
}
