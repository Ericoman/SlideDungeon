using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public float initialMoveSpeed = 5f; // Movement speed of the character
    private float _moveSpeed; // Movement speed of the character


    private Vector2 move;
    private Vector3 lastMovementDirection;
    public GameObject InteractPuzzle;

    private bool _canMove = true;
    private bool _blockX = false;
    private bool _blockZ = false;

    public Animator anim;

    private PlayerInput _playerInput;

    public void OnMove(InputAction.CallbackContext context)
    {
        move = context.ReadValue<Vector2>();
    }

    private void Awake()
    {
       
        _playerInput = GetComponent<PlayerInput>();
        _moveSpeed = initialMoveSpeed;
    }

    private void Update()
    {
        if (_playerInput.actions["InteractPuzzle"].WasReleasedThisFrame())
        {
            if (!GameManager.Instance.movingCamera)
            {
                if (_canMove)
                {
                    _canMove = false;
                    Debug.Log(GameManager.Instance.maincamera.transform.position + "  "+GameManager.Instance.endCamera.transform.position);
                    GameManager.Instance.MoveCameraToLocation(GameManager.Instance.maincamera.transform, GameManager.Instance.endCamera.transform);
                    GameManager.Instance.ResetHighlight();
                    GameManager.Instance.HighlightTileable();
                }
                else if(GameManager.Instance.selectedTileable==null){

                    _canMove = true;
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
        if (_blockX) move.x = 0f;
        if (_blockZ) move.y = 0f;

        Vector3 movement = new Vector3(move.x, 0f, move.y);

        if (_canMove)
        {
            if (movement != Vector3.zero)
            {
                anim.SetTrigger("Move");
                // Not rotation if axis blocked       
                if (!_blockX && !_blockZ)
                {
                    // Update the last movement direction
                    lastMovementDirection = movement.normalized;

                    // Rotate the player to look in the direction of movement
                    transform.rotation = Quaternion.Slerp(
                        transform.rotation,
                        Quaternion.LookRotation(lastMovementDirection),
                        Time.deltaTime * 10f
                    );
                }
            }
            else
            {
                anim.SetTrigger("Idle");
            }

                // Translate player en la dirección de movement
                transform.Translate(movement * _moveSpeed * Time.deltaTime, Space.World);
        }
    }


    public void SetCanMove(bool move) 
    { 
        _canMove = move;
    }
    public void SetBlockX(bool move) 
    {
        _blockX = move;
    }
    public void SetBlockZ(bool move)
    {
        _blockZ = move;
    }
    public void SetMoveSpeed(float speed) 
    {
        _moveSpeed = speed;
    }
    public void SetFreeMovement() 
    {
        _canMove = true;
        _blockX = false;
        _blockZ = false;
        _moveSpeed = initialMoveSpeed;
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