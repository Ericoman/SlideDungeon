using System;
using UnityEngine;

namespace Tools
{
    [RequireComponent(typeof(Tileable))]
    [ExecuteInEditMode]
    public class RoomBuilder : MonoBehaviour
    {
        public GameObject wallUnitPrefab;
        public bool bUseCollidersForWallBounds = true;
        public GameObject floorUnitPrefab;
        public bool bUseCollidersForFloorBounds = true;

        
        public float wallHeight = 2f;
        public float floorHeight = 1f;
        public int cellSize = 30;
        
        private const int WALL_NUMBER = 4;
        private Tileable _tileable;
        private GameObject _wallsContainer;
        private GameObject _floorContainer;
        private GameObject _customElementsContainer;
        
        private const string WALLS_PARENT_NAME = "Walls";
        private const string WALL_PARENT_NAME = "Wall";
        private const string FLOOR_PARENT_NAME = "Floor";
        private const string CUSTOM_ELEMENTS_PARENT_NAME = "Custom Elements";
        
        Bounds wallTileBounds;
        Bounds floorTileBounds;
        
        private void Awake()
        {
            _tileable = GetComponent<Tileable>();
            _wallsContainer = transform.Find(WALLS_PARENT_NAME)?.gameObject;
            _floorContainer = transform.Find(FLOOR_PARENT_NAME)?.gameObject;
            _customElementsContainer = transform.Find(CUSTOM_ELEMENTS_PARENT_NAME)?.gameObject;
            
        }

        private Bounds GetBounds(Transform objectTransform, bool bUseColliders)
        {
            Bounds combinedBounds = new Bounds(objectTransform.position, Vector3.zero);
            bool hasBounds = false;

            if (bUseColliders)
            {
                GameObject temp = Instantiate(objectTransform.gameObject);
                Physics.SyncTransforms();
                foreach (Collider col in temp.GetComponentsInChildren<Collider>())
                {
                    if (!hasBounds)
                    {
                        combinedBounds = col.bounds;
                        hasBounds = true;
                    }
                    else
                    {
                        combinedBounds.Encapsulate(col.bounds);
                    }
                }
                DestroyImmediate(temp);
            }
            else
            {
                foreach (Renderer rend in objectTransform.GetComponentsInChildren<Renderer>())
                {
                    if (!hasBounds)
                    {
                        combinedBounds = rend.bounds;
                        hasBounds = true;
                    }
                    else
                    {
                        combinedBounds.Encapsulate(rend.bounds);
                    }
                }
            }
    
            if (hasBounds)
            {
                Vector3 totalSize = combinedBounds.size;
                Debug.Log("Total object size (world space): " + totalSize);
            }
            else
            {
                Debug.LogWarning("No renderers found on this object or its children.");
            }
            
            return combinedBounds;
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
            wallTileBounds = GetBounds(wallUnitPrefab.transform,bUseCollidersForWallBounds);

            if (wallUnitPrefab == null) return;
            
            if (_wallsContainer == null)
            {
                _wallsContainer = new GameObject();
                _wallsContainer.transform.SetParent(transform);
                _wallsContainer.transform.localPosition = Vector3.zero;
                _wallsContainer.name = WALLS_PARENT_NAME;
            }
            
            for (int i = 0; i < WALL_NUMBER; i++)
            {
                GameObject currentWall = new GameObject();
                currentWall.transform.SetParent(_wallsContainer.transform);
                currentWall.transform.localPosition = Vector3.zero;
                currentWall.name = WALL_PARENT_NAME + i;

                if (i % 2 != 0)
                {
                    int numTiles = Mathf.CeilToInt(_tileable.HeightTiles * cellSize / wallTileBounds.size.x);
                    for (int j = 0; j < numTiles; j++)
                    {
                        GameObject wallTile = Instantiate(wallUnitPrefab, currentWall.transform);
                        wallTile.transform.localScale = new Vector3(1f,wallHeight,1f);
                        RotateWall(wallTile.transform,-currentWall.transform.right,currentWall.transform.up,i/2 == 0);
                        wallTile.transform.localPosition += new Vector3(0,wallTileBounds.size.x * j,0);
                    }
                }
                else
                {
                    int numTiles = Mathf.CeilToInt(_tileable.WidthTiles * cellSize / wallTileBounds.size.x);
                    for (int j = 0; j < numTiles; j++)
                    {
                        GameObject wallTile = Instantiate(wallUnitPrefab, currentWall.transform);
                        wallTile.transform.localScale = new Vector3(1f,wallHeight,1f);
                        RotateWall(wallTile.transform,currentWall.transform.up,currentWall.transform.right,i/2 == 0);
                        wallTile.transform.localPosition += new Vector3(wallTileBounds.size.x * j,0,0);
                    }
                }
                
                float posX = i % 2 == 0 ? 0 : i/2 * _tileable.WidthTiles * cellSize;
                float posY = i % 2 != 0 ? 0 : (WALL_NUMBER-1-i)/2 * _tileable.HeightTiles * cellSize;
                float posZ = currentWall.transform.localPosition.z;
                currentWall.transform.localPosition = new Vector3(posX,posY,posZ);
            }
            

        }
        public static Quaternion GetPrefabToParentRotation()
        {
            Vector3 right = Vector3.right;       // Parent +X
            Vector3 up = Vector3.back;            // Parent -Z
            Vector3 forward = Vector3.up;         // Parent +Y

            Matrix4x4 m = new Matrix4x4();
            m.SetColumn(0, right);
            m.SetColumn(1, up);
            m.SetColumn(2, forward);
            m.SetColumn(3, new Vector4(0, 0, 0, 1));

            return m.rotation;
        }
        private void RotateWall(Transform wallTransform,Vector3 wallForward ,Vector3 wallDirection, bool interiorOnRight)
        {
            wallTransform.localRotation = GetPrefabToParentRotation();
            
            wallDirection.Normalize();
            wallForward.Normalize();
            
            if (!interiorOnRight)
            {
                wallForward *= -1;
                wallDirection *= -1;
            }
            wallTransform.localPosition -= wallForward * wallTileBounds.size.z;
            
            wallTransform.localRotation = Quaternion.LookRotation(wallForward, wallTransform.up);
            if (!interiorOnRight)
            {
                wallTransform.localPosition -= wallDirection * wallTileBounds.size.x;
            }
        }

        public void GenerateFloors()
        {
            ClearFloors();
            wallTileBounds = GetBounds(wallUnitPrefab.transform,bUseCollidersForWallBounds);
            floorTileBounds = GetBounds(floorUnitPrefab.transform,bUseCollidersForFloorBounds);

            if (floorUnitPrefab == null) return;
            
            if (_floorContainer == null)
            {
                _floorContainer = new GameObject();
                _floorContainer.transform.SetParent(transform);
                _floorContainer.transform.localPosition = Vector3.zero;
                _floorContainer.name = FLOOR_PARENT_NAME;
            }
            
            GameObject floor = new GameObject();
            floor.transform.SetParent(_floorContainer.transform);
            floor.transform.localPosition = Vector3.zero;
            floor.name = FLOOR_PARENT_NAME;
            
            int numTilesWidth = Mathf.CeilToInt(_tileable.WidthTiles * cellSize / floorTileBounds.size.x - 2*wallTileBounds.size.z);
            int numTilesHeight = Mathf.CeilToInt(_tileable.HeightTiles * cellSize / floorTileBounds.size.z - 2*wallTileBounds.size.z);
            for (int i = 0; i < numTilesWidth; i++)
            {
                for (int j = 0; j < numTilesHeight; j++)
                {
                    GameObject floorTile = Instantiate(floorUnitPrefab, floor.transform);
                    floorTile.transform.localScale = new Vector3(1f,floorHeight,1f);
                    floorTile.transform.localRotation = GetPrefabToParentRotation();
                    floorTile.transform.localPosition += new Vector3(i,j,0);
                }
            }
            
            float posZ = floor.transform.localPosition.z - floorTileBounds.size.y*floorHeight;
            floor.transform.localPosition = new Vector3( wallTileBounds.size.z,wallTileBounds.size.z,posZ);
            
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
                _customElementsContainer.name = CUSTOM_ELEMENTS_PARENT_NAME;
            }
        }
    }
}
