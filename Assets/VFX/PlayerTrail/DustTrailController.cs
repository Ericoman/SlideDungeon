using UnityEngine;
using UnityEngine.VFX;
public class DustTrailController : MonoBehaviour
{
    [Header("References")]
    public VisualEffect dustVFX; // arrastra aquí tu VFX Graph en el inspector
    public float minSpeedToSpawn = 0.1f; // velocidad mínima para que aparezca polvo


    void Start()
    {
        
        if (dustVFX != null)
        {
            dustVFX.Stop(); // empieza apagado
        }
    }

    void Update()
    {
        if (dustVFX == null ) return;

        float speed = GetComponent<Rigidbody>().linearVelocity.magnitude;

        if (speed > minSpeedToSpawn)
        {
            if (!dustVFX.isActiveAndEnabled)
                dustVFX.Play();
            
            dustVFX.SetFloat("SpawnRate", speed * 5f); 
        }
        else
        {
            if (dustVFX.isActiveAndEnabled)
                dustVFX.Stop();
        }
    }
}
