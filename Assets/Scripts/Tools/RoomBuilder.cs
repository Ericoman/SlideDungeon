using System;
using UnityEngine;

namespace Tools
{
    [RequireComponent(typeof(Tileable))]
    [ExecuteInEditMode]
    public class RoomBuilder : MonoBehaviour
    {
        public GameObject wallUnitPrefab;
        public bool expandWallPrefab;
        public GameObject floorUnitPrefab;
        public bool expandFloorPrefab;

        public float wallHeight = 2f;
        public float floorHeight = 1f;
        public int cellSize = 30;
        
        private const int WALL_NUMBER = 4;
        private Tileable _tileable;
        private GameObject _wallsContainer;
        private GameObject _floorContainer;
        private GameObject _customElementsContainer;

        private void Awake()
        {
            _tileable = GetComponent<Tileable>();
        }

        public void GenerateRoom()
        {
            GenerateCustomElementsContainer();
            GenerateWalls();
            GenerateFloors();
        }

        public void GenerateWalls()
        {
            ClearWalls();

            if (wallUnitPrefab == null) return;
            
            if (_wallsContainer == null)
            {
                _wallsContainer = new GameObject();
                _wallsContainer.transform.SetParent(transform);
                _wallsContainer.transform.localPosition = Vector3.zero;
                _wallsContainer.name = "Walls";
            }

            if (expandWallPrefab)
            {
                for (int i = 0; i < WALL_NUMBER; i++)
                {
                    GameObject wall = Instantiate(wallUnitPrefab,_wallsContainer.transform);
                    wall.name = "Wall" + i;

                    int width = i % 2  != 0 ? 1 : _tileable.WidthTiles * cellSize;
                    int height = i % 2 == 0 ? 1 : _tileable.HeightTiles * cellSize;
                    wall.transform.localScale = new Vector3(width,height, wallHeight);
                    
                    int posX = i % 2 == 0 ? 0 : i/2 * (_tileable.WidthTiles * cellSize - 1);
                    int posY = i % 2 != 0 ? 0 : (WALL_NUMBER-1-i)/2 * (_tileable.HeightTiles * cellSize - 1);
                    float posZ = wall.transform.localPosition.z - wallHeight;
                    wall.transform.localPosition = new Vector3(posX ,posY,posZ);
                }
            }
            else
            {
                for (int i = 0; i < WALL_NUMBER; i++)
                {
                    GameObject currentWall = new GameObject();
                    currentWall.transform.SetParent(_wallsContainer.transform);
                    currentWall.transform.localPosition = Vector3.zero;
                    currentWall.name = "Wall" + i;

                    if (i % 2 != 0)
                    {
                        int numTiles = _tileable.HeightTiles * cellSize;
                        for (int j = 0; j < numTiles; j++)
                        {
                            GameObject wallTile = Instantiate(wallUnitPrefab, currentWall.transform);
                            wallTile.transform.localScale = new Vector3(1f,1f, wallHeight);
                            wallTile.transform.localPosition = new Vector3(wallTile.transform.localPosition.x,j,wallTile.transform.localPosition.z);
                        }
                    }
                    else
                    {
                        int numTiles = _tileable.WidthTiles * cellSize;
                        for (int j = 0; j < numTiles; j++)
                        {
                            GameObject wallTile = Instantiate(wallUnitPrefab, currentWall.transform);
                            wallTile.transform.localScale = new Vector3(1f,1f, wallHeight);
                            wallTile.transform.localPosition = new Vector3(j,wallTile.transform.localPosition.y,wallTile.transform.localPosition.z);
                        }
                    }
                    
                    int posX = i % 2 == 0 ? 0 : i/2 * (_tileable.WidthTiles * cellSize - 1);
                    int posY = i % 2 != 0 ? 0 : (WALL_NUMBER-1-i)/2 * (_tileable.HeightTiles * cellSize - 1);
                    float posZ = currentWall.transform.localPosition.z - wallHeight;
                    currentWall.transform.localPosition = new Vector3(posX ,posY,posZ);
                }
            }

        }

        public void GenerateFloors()
        {
            ClearFloors();

            if (floorUnitPrefab == null) return;
            
            if (_floorContainer == null)
            {
                _floorContainer = new GameObject();
                _floorContainer.transform.SetParent(transform);
                _floorContainer.transform.localPosition = Vector3.zero;
                _floorContainer.name = "Floor";
            }

            if (expandFloorPrefab)
            {
                GameObject floor = Instantiate(floorUnitPrefab,_floorContainer.transform);
                floor.name = "Floor";

                int width = _tileable.WidthTiles * cellSize - 2;
                int height = _tileable.HeightTiles * cellSize - 2;
                floor.transform.localScale = new Vector3(width,height, floorHeight);
                
                float posZ = floor.transform.localPosition.z - floorHeight;
                floor.transform.localPosition = new Vector3(1 ,1,posZ);
                
            }
            else
            {
                GameObject floor = new GameObject();
                floor.transform.SetParent(_floorContainer.transform);
                floor.transform.localPosition = Vector3.zero;
                floor.name = "Floor";
                
                int numTilesWidth = _tileable.WidthTiles * cellSize - 2;
                int numTilesHeight = _tileable.HeightTiles * cellSize - 2;
                for (int i = 0; i < numTilesWidth; i++)
                {
                    for (int j = 0; j < numTilesHeight; j++)
                    {
                        GameObject floorTile = Instantiate(floorUnitPrefab, floor.transform);
                        floorTile.transform.localScale = new Vector3(1f,1f, floorHeight);
                        floorTile.transform.localPosition = new Vector3(i,j,floorTile.transform.localPosition.z);
                    }
                }
                
                float posZ = floor.transform.localPosition.z - floorHeight;
                floor.transform.localPosition = new Vector3(1 ,1,posZ);
            }
        }

        public void ClearRoomExceptCustom()
        {
            ClearWalls();
            ClearFloors();
        }
        public void ClearRoom()
        {
            ClearChildren(transform);
        }
        public void ClearWalls()
        {
            if (_wallsContainer != null)
            {
                ClearChildren(_wallsContainer.transform);
                DestroyImmediate(_wallsContainer);
                _wallsContainer = null;
            }
        }

        public void ClearFloors()
        {
            if (_floorContainer != null)
            {
                ClearChildren(_floorContainer.transform);
            }
            DestroyImmediate(_floorContainer);
            _floorContainer = null;
        }

        public void ClearChildren(Transform parent)
        {
            for (int i = parent.childCount - 1; i >= 0; i--)
            {
                DestroyImmediate(parent.GetChild(i).gameObject);
            } 
        }

        private void GenerateCustomElementsContainer()
        {
            if (_customElementsContainer == null)
            {
                _customElementsContainer = new GameObject();
                _customElementsContainer.transform.SetParent(transform);
                _customElementsContainer.transform.localPosition = Vector3.zero;
                _customElementsContainer.name = "Custom Elements";
            }
        }
    }
}
