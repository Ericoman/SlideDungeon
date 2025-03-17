using System.Collections;
using UnityEngine;

public class TileInstancer : MonoBehaviour
{
    [SerializeField]
    GameObject[] tilePrefabs;
    
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                Tileable tileable = hit.collider.GetComponentInParent<Tileable>();
                if (tileable != null)
                {
                    MoveTile(tileable);
                }
                else
                {
                    int tileIndex = 0;
                    if (Input.GetKey(KeyCode.Alpha1))
                    {
                        tileIndex = 1;
                    }    
                    else if (Input.GetKey(KeyCode.Alpha2))
                    {
                        tileIndex = 2;
                    }
                    else if (Input.GetKey(KeyCode.Alpha3))
                    {
                        tileIndex = 3;
                    }
                    
                    GridManager gridManager = hit.collider.gameObject.GetComponentInParent<GridManager>();
                    GameObject tile = InstantiateTileOnGrid(gridManager,hit.point,tileIndex);
                    if (tile == null)
                    {
                        Debug.LogWarning("Tile not instantiated, no valid grid found");
                    }
                }
            }

            
        }
    }

    void MoveTile(Tileable tile)
    {
        tile.RemoveFromGrid();
        StartCoroutine(MoveTile_CO(tile));
    }

    IEnumerator MoveTile_CO(Tileable tile)
    {
        bool moving = true;
        while (moving)
        {
            Vector3 mousePos = Input.mousePosition;
            mousePos.z = -Camera.main.transform.position.z;
            Vector3 newPosition = Camera.main.ScreenToWorldPoint(mousePos);
            newPosition.z = tile.transform.position.z;

            tile.TryMove(newPosition);

            if (Input.GetMouseButtonUp(0))
            {
                tile.ResetToPreviousGrid();
                moving = false;
            }

            yield return null;
        }
    }
    GameObject InstantiateTileOnGrid(GridManager gridManager, Vector3 position, int tileIndex)
    {
        if (gridManager)
        {
            GameObject tile = Instantiate(tilePrefabs[tileIndex],position,Quaternion.identity);
            Tileable tileable = tile.GetComponent<Tileable>();
            tileable.SetInGrid(gridManager);
            return tile;
        }
    
        return null;
    }
}
