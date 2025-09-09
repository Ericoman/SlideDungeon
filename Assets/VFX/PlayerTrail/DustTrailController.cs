using System;
using UnityEngine;
using UnityEngine.VFX;
public class DustTrailController : MonoBehaviour
{
    [Header("References")]
    public PlayerMovement player;   // referencia a tu script PlayerMovement
    public VisualEffect dustVFX;    // arrastra aquí tu VFX Graph
    public float minSpeedToSpawn = 0.1f;
    public float spawnMultiplier = 10f;


    void Start()
    {
        
        if (dustVFX != null)
        {
            dustVFX.Play();
        }
    }

    void Update()
    {
        if (dustVFX == null || player == null) return;

        // obtenemos la magnitud de movimiento (velocidad "real")
        Vector3 move = new Vector3(player.MoveInput.x, 0f, player.MoveInput.y);
        float speed = move.magnitude * player.CurrentMoveSpeed;

        if (speed > minSpeedToSpawn)
        {
            
            
            dustVFX.SetFloat("SpawnRate",  spawnMultiplier);
        }
        else
        {

            dustVFX.SetFloat("SpawnRate", 0f);
        }
    }
}
