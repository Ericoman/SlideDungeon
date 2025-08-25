using UnityEngine;

public class RotateItem : MonoBehaviour
{
    public float velocidadY = 50f;

    void Update()
    {
        transform.Rotate(0, velocidadY * Time.deltaTime, 0);
    }
}
