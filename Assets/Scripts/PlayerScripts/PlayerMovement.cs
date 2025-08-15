using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f; // Movement speed of the character
    private Vector2 move;
    private Vector3 lastMovementDirection;
    public GameObject InteractPuzzle;

    public bool canMove = true;

    private PlayerInput _playerInput;

    public void OnMove(InputAction.CallbackContext context)
    {
        move = context.ReadValue<Vector2>();
    }

    private void Awake()
    {
       
        _playerInput = GetComponent<PlayerInput>();
    }

    private void Update()
    {
        if (_playerInput.actions["InteractPuzzle"].WasReleasedThisFrame())
        {
            if (!GameManager.Instance.movingCamera)
            {
                if (canMove)
                {
                    canMove = false;
                    Debug.Log(GameManager.Instance.maincamera.transform.position + "  "+GameManager.Instance.endCamera.transform.position);
                    GameManager.Instance.MoveCameraToLocation(GameManager.Instance.maincamera.transform, GameManager.Instance.endCamera.transform);
                    GameManager.Instance.ResetHighlight();
                    GameManager.Instance.HighlightTileable();
                }
                else if(GameManager.Instance.selectedTileable==null){
                    
                    canMove = true;
                    GameManager.Instance.ResetHighlight();
                    GameManager.Instance.ResetMainCamera(GameManager.Instance.maincamera.transform, GameManager.Instance.savedCamPosition);
                }
            }

            
            
        }
        if (!GameManager.Instance.ShowingFirstmovement)
        {
            if (GameManager.Instance.puzzleMode)
            {
                    if (_playerInput.actions["SelectTileable"].WasReleasedThisFrame())
                    {
                        GameManager.Instance.SetSelectedTileable();
                    }


                    MovePuzzle();
                    
                
            }
            else
            {
                MovePlayer();
            }
        }
        
            
    }

    public void MovePuzzle()
    {
        if (move.sqrMagnitude != 0)
        {
            GameManager.Instance.SetNextTileable(move.x, move.y);
        }
    }
    
    public void MovePlayer()
    {
        Vector3 movement = new Vector3(move.x, 0f, move.y);

        if (canMove)
        {
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

    

    
    
    private void OnTriggerEnter(Collider other)
    {
        InteractPuzzle.SetActive(true);
        //if (other.gameObject.tag == "EndTriggerVolume")
        //{
        //    GameManager.Instance.EnableCameraEnd();
        //}
    }
    
    

    private void OnTriggerExit(Collider other)
    {
        InteractPuzzle.SetActive(false);
        //if (other.gameObject.tag == "EndTriggerVolume")
        //{
        //    GameManager.Instance.DisableCameraEnd();
        //}
    }

}