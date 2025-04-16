using UnityEngine;

public class TileBehaviour : MonoBehaviour
{
    Tileable tile;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        tile = gameObject.GetComponentInParent<Tileable>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            
            Debug.Log("UWU");
            // Obtener dirección del impacto
            Vector3 direction = collision.GetContact(0).normal;

            // Redondear dirección a una unidad en el eje dominante
            direction = GetDominantDirection(direction);

            //Mover la tile una unidad en esa dirección
            Vector3 newPosition = transform.position + direction;
            Vector2Int oldGridPosition = tile.LastGridPosition;
            if (tile.TryMove(newPosition))
            {
                //TileMovedEvent?.Invoke(this, tile.LastGridPosition, oldGridPosition);
            }

            transform.position += direction;
        }
        
    }
    Vector3 GetDominantDirection(Vector3 dir)
    {
        dir = dir.normalized;

        // Detectar el eje más dominante
        if (Mathf.Abs(dir.x) > Mathf.Abs(dir.z))
        {
            return new Vector3(Mathf.Sign(dir.x), 0, 0);
        }
        else
        {
            return new Vector3(0, 0, Mathf.Sign(dir.z));
        }
    }
}
