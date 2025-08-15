using System;
using System.Collections.Generic;
using System.Linq;
using Grid.DataObjects;
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

    private Transform _backgroundContainer = null; 
    private const string BACKGROUND_TILE_PARENT_NAME = "BackgroundTiles";
    public void Awake()
    {
        grid = new int[width,height];
        tiles = new Tileable[width, height];
        _backgroundContainer = transform.Find(BACKGROUND_TILE_PARENT_NAME);
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
        if (_backgroundContainer == null)
        {
            _backgroundContainer = new GameObject(BACKGROUND_TILE_PARENT_NAME).transform;
            _backgroundContainer.SetParent(transform);
            _backgroundContainer.localPosition = Vector3.zero;
            _backgroundContainer.localRotation = Quaternion.identity;
            _backgroundContainer.localScale = Vector3.one;
        }

        if (_backgroundContainer.childCount != Width * Height)
        {
            foreach (Transform child in _backgroundContainer)
            {
                DestroyImmediate(child.gameObject);
            }
            
            for (int z = 0; z < height; z++)
            {
                for (int x = 0; x < width; x++)
                {
                    GameObject backgroundTile = Instantiate(backgroundTilePrefab, _backgroundContainer);
                    backgroundTile.transform.localPosition = new Vector3(x*cellSize, z*cellSize, cellSize);
                    //backgroundTile.transform.localPosition = new Vector3(x * cellSize, 0, y * cellSize);
                    backgroundTile.transform.localScale *= cellSize;
                    //backgroundTile.transform.rotation = Quaternion.Euler(90, 0, 0);
                    backgroundTile.layer = gameObject.layer;
                    foreach (Transform child in backgroundTile.transform)
                    {
                        child.gameObject.layer = gameObject.layer;
                    }
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
            Vector3 end = new Vector3(x * cellSize,0, height * cellSize);
            Gizmos.DrawLine(start + transform.position, end + transform.position);
        }

        for (int z = 0; z <= height; z++)
        {
            Vector3 start = new Vector3(0, 0, z * cellSize);
            Vector3 end = new Vector3(width * cellSize,0, z * cellSize);
            Gizmos.DrawLine(start + transform.position, end + transform.position);
        }
    }

    public Vector2Int SetInGrid(Tileable tileable)
    {
        return SetInGrid(tileable,new Vector2Int(-1, -1));
    }
    public Vector2Int SetInGrid(Tileable tileable, Vector2Int preferredGridPosition)
    {
        //Debug.Log(tileable.transform.position);
        Vector2Int finalPreferredGridPosition = preferredGridPosition;
        if(preferredGridPosition.x < 0 || preferredGridPosition.y < 0)
        {
            finalPreferredGridPosition = WorldToGrid(tileable.transform.position);
        }
        Vector2Int assignedCoordinates = GetNextAvailableCoordinates(tileable.WidthTiles, tileable.HeightTiles,finalPreferredGridPosition);
        //Debug.Log("NEXTAVAILABLE COORDINATES: "+assignedCoordinates.x + " "+ assignedCoordinates.y);
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
        int x = Mathf.FloorToInt((position.x - transform.position.x)  / cellSize);
        int z = Mathf.FloorToInt((position.z - transform.position.z)/ cellSize);
        //Debug.Log("WorldToGrid TO GRID: " + x + " and " + z);
        x = Mathf.FloorToInt(Mathf.Clamp(x, 0, width));
        z = Mathf.FloorToInt(Mathf.Clamp(z, 0, height));

        return new Vector2Int(x,z);
    }

    public Vector3 GridToWorld(Vector2Int gridPosition)
    {
        return new Vector3(gridPosition.x * cellSize+transform.position.x, transform.position.y, gridPosition.y * CellSize+transform.position.z);
    }

    public Tileable GetTile(Vector2Int gridPosition)
    {
        if (gridPosition.x >= 0 && gridPosition.x < tiles.Length && gridPosition.y >= 0 && gridPosition.y < tiles.Length)
        {
            return tiles[gridPosition.x, gridPosition.y];
        }
        return null;
    }

    public int GetTilesLength()
    {
        return tiles.Length;
    }

    public Tileable[] GetIndividualTilesInGrid()
    {
        HashSet<Tileable> individualTiles = new HashSet<Tileable>();
        for (int i = 0; i < width; ++i)
        {
            for (int j = 0; j < height; ++j)
            {
                if (tiles[i, j] != null)
                {
                    individualTiles.Add(tiles[i, j]);
                }
            }
        }
        return individualTiles.ToArray();
    }

    public TileContext[] GetIndividualTileContexts()
    {
        HashSet<TileContext> individualTileContexts = new HashSet<TileContext>();
        for (int i = 0; i < width; ++i)
        {
            for (int j = 0; j < height; ++j)
            {
                if (tiles[i, j] != null)
                {
                    individualTileContexts.Add(tiles[i, j].TileContext);
                }
            }
        }
        return individualTileContexts.ToArray();
    }
}
