using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovementManu : MonoBehaviour
{

    //2h 7mins
   public float speed = 5f;
    private Vector2 moveDirection;
    public InputActionReference move;

    private void Update()
    {
        moveDirection = move.action.ReadValue<Vector2>();
        
        Vector3 movement = new Vector3(moveDirection.x, 0f, moveDirection.y);
        transform.position += movement * speed * Time.deltaTime;
        
        if (movement != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(movement);
        }
    }
}
