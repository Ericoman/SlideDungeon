using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovementManu : MonoBehaviour
{

    //2h 7mins
    [SerializeField] private float speed = 5f;
    [SerializeField] private Rigidbody rb;
    [SerializeField] private Transform referenceObject; // Objeto de referencia arrastrado desde el editor
    [SerializeField] private bool tracker = false; // Activar/desactivar límite de distancia
    [SerializeField] private float maxDistanceX = 5f; // Distancia máxima en X
    [SerializeField] private float maxDistanceZ = 5f; // Distancia máxima en Z
    public bool canMove = true;
    public InputActionReference move;

    private Vector2 moveDirection;

    private void Update()
    {
        moveDirection = move.action.ReadValue<Vector2>();

            // Normalizar la dirección si se mueve en diagonal
        if (moveDirection.sqrMagnitude > 1f)
        {
            moveDirection = moveDirection.normalized;
        }
    }

    private void FixedUpdate()
    {
        if (!canMove) return;
        Vector3 movement = new Vector3(moveDirection.x, 0f, moveDirection.y) * speed * Time.fixedDeltaTime;
        Vector3 newPosition = rb.position + movement;

        if (tracker && referenceObject != null)
        {
            Vector3 refPos = referenceObject.position; // Posición del objeto de referencia

            // Limitar en el eje X
            float clampedX = Mathf.Clamp(newPosition.x, refPos.x - maxDistanceX, refPos.x + maxDistanceX);
            // Limitar en el eje Z
            float clampedZ = Mathf.Clamp(newPosition.z, refPos.z - maxDistanceZ, refPos.z + maxDistanceZ);

            // Se mantiene la misma altura
            newPosition = new Vector3(clampedX, rb.position.y, clampedZ);
        }

        rb.MovePosition(newPosition);

        if (moveDirection.sqrMagnitude > 0.01f) // Evita rotación innecesaria cuando no se mueve
        {
            Quaternion targetRotation = Quaternion.LookRotation(new Vector3(moveDirection.x, 0f, moveDirection.y));
            rb.MoveRotation(Quaternion.Slerp(rb.rotation, targetRotation, Time.fixedDeltaTime * 10f));
        }
    }
}
