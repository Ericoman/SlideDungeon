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
            GameObject tInstancer = GameObject.FindGameObjectWithTag("TileInstancer");
            //Debug.Log("UWU");
            // Obtener direcci�n del impacto
            Vector3 direction = collision.GetContact(0).normal;

            // Redondear direcci�n a una unidad en el eje dominante
            direction = GetDominantDirection(direction);

            //Mover la tile una unidad en esa direcci�n
            Vector3 newPosition = transform.position + direction;
            Vector2Int oldGridPosition = tile.LastGridPosition;
            

            tInstancer.GetComponent<TileInstancer>().NewMoveTile(tile, new Vector2Int(Mathf.FloorToInt(direction.x),Mathf.FloorToInt(direction.z)));
            //TileMovedEvent?.Invoke(this, tile.LastGridPosition, oldGridPosition);
            

            //transform.position += direction;
        }
        
    }
    Vector3 GetDominantDirection(Vector3 dir)
    {
        dir = dir.normalized;

        // Detectar el eje m�s dominante
        if (Mathf.Abs(dir.x) > Mathf.Abs(dir.z))
        {
            return new Vector3(Mathf.Sign(dir.x), 0f, 0f);
        }
        else
        {
            return new Vector3(0f, 0f, Mathf.Sign(dir.z));
        }
        
    }
}
