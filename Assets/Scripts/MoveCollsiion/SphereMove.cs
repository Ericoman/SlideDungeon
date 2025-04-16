using UnityEngine;

public class SphereMove : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public float moveForce = 10f;
    public float torqueForce = 5f;

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        float h = Input.GetAxis("Horizontal"); // A/D o Flechas izquierda/derecha
        float v = Input.GetAxis("Vertical");   // W/S o Flechas arriba/abajo

        Vector3 moveDir = new Vector3(h, 0, v).normalized;

        // Aplicar fuerza para mover
        rb.AddForce(moveDir * moveForce);

        // Aplicar torque para que rote como una esfera real
        Vector3 torqueDir = new Vector3(v, 0, -h); // Invertimos para que coincida con la dirección visual
        rb.AddTorque(torqueDir * torqueForce);
    }
}
