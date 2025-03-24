using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f; // Movement speed of the character
    private Vector2 move;
    private Vector3 lastMovementDirection;
    public void OnMove(InputAction.CallbackContext context)
    {
        move = context.ReadValue<Vector2>();
    }

    private void Update()
    {
        MovePlayer();
    }
    
    public void MovePlayer()
    {
        Vector3 movement = new Vector3(move.x, 0f, move.y);
        
        if (movement != Vector3.zero)
        {
            // Update the last movement direction
            lastMovementDirection = movement.normalized;

            // Rotate the player to look in the direction of movement
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(lastMovementDirection), Time.deltaTime * 10f);
        }

        // Translate player in the direction of movement
        transform.Translate(movement * moveSpeed * Time.deltaTime, Space.World);
    }
    
}