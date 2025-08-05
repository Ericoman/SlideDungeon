using Assets.Devs.Julia.Scripts;
using Assets.Scripts.Interactions;
using Unity.Cinemachine;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class Interactor : MonoBehaviour
{
    [SerializeField] private float _interactionRadius = 1.5f;
    [SerializeField] private LayerMask _interactableLayer;
    [SerializeField] private Canvas _canvas;
    [SerializeField] private float _maxDistanceToFloor = 2;

    private PlayerInput _playerInput;
    private InputAction _interactAction;
    private Transform _transform;

    private GameObject _grabbedObject = null;

    private float _interactHeldTime = 0f;
    public float holdThreshold = 0.5f;

    private void Awake()
    {
        _transform = transform;
        _playerInput = GetComponent<PlayerInput>();
        _interactAction = _playerInput.actions["Interact"];
    }


    public void SetGrabbedObject(GameObject grabbedObject) 
    {
        _grabbedObject = grabbedObject;
        if (_canvas && grabbedObject != null) _canvas.gameObject.SetActive(false);
    }

    ///////VERSION 2
    ////////////////

    private void Update()
    {
        if (_grabbedObject == null) //not grabbing
        {
            if (Physics.Raycast(_transform.position, transform.forward, out var hit, _interactionRadius, _interactableLayer))
            {
                IInteractable interactableObject = null;
                hit.transform.TryGetComponent(out interactableObject);
                IInteractableHeld interactableHeldObject = null;
                hit.transform.TryGetComponent(out interactableHeldObject);

                if (interactableObject != null || interactableHeldObject != null)
                {
                    if (_canvas) _canvas.gameObject.SetActive(true);

                    //Diferenciate if holding or just press considerring witch acction is available

                    if (interactableHeldObject != null && _interactAction.IsPressed()) //HOLDING
                    {
                        _interactHeldTime += Time.deltaTime;

                        if (_interactHeldTime >= holdThreshold) //---> holding action 
                        {
                            Debug.Log("Holding Interact");
                            if (_playerInput.actions["RotateRight"].WasReleasedThisFrame()) //right
                            {
                                interactableHeldObject.InteractHeldRight(gameObject);
                            }
                            else if (_playerInput.actions["RotateLeft"].WasReleasedThisFrame()) //left
                            {
                                interactableHeldObject.InteractHeldLeft(gameObject);
                            }

                        }
                    }
                    else if (_interactAction.WasPressedThisFrame()) //INITIAL PRESS
                    {
                        _interactHeldTime = 0f;
                    }
                    else if (_interactAction.WasReleasedThisFrame()) //RELEASE PRESS
                    {
                        if (interactableObject != null && _interactHeldTime < holdThreshold) //---> press action 
                        {
                            interactableObject.Interact(gameObject);
                            Debug.Log("interactableObject reached");
                        }
                        _interactHeldTime = 0f; 
                    }
                }
            }
            else
            {
                if (_canvas) _canvas.gameObject.SetActive(false);
            }
        }
        else //grabing -> DROP
        {
            if (_interactAction.WasReleasedThisFrame()) //try drop 
            {
                bool isHitDrop = Physics.Raycast(_transform.position, transform.forward, out var hitDrop, _grabbedObject.transform.localScale.x + 1);
                Debug.DrawRay(_transform.position, transform.forward * (_grabbedObject.transform.localScale.x + 1), Color.red, 2f, false);

                //floor under
                bool isFloor = Physics.Raycast(_grabbedObject.transform.position, Vector3.down, _maxDistanceToFloor);
                Debug.DrawRay(_grabbedObject.transform.position, Vector3.down * _maxDistanceToFloor, Color.green, 2f, false);


                if ((!isHitDrop || hitDrop.collider.gameObject == _grabbedObject) && isFloor) //can drop
                {
                    // Detach from parent
                    _grabbedObject.transform.SetParent(null);

                    // Snap rotation to nearest 45 degrees on Y axis
                    Quaternion currentRotation = _grabbedObject.transform.rotation;
                    Vector3 euler = currentRotation.eulerAngles;
                    // Round Y angle to nearest multiple of 45
                    float snappedY = Mathf.Round(euler.y / 45f) * 45f;
                    // Apply snapped rotation (keep X and Z unchanged, or set to 0 if you want to limit to Y only)
                    _grabbedObject.transform.rotation = Quaternion.Euler(0f, snappedY, 0f);

                    _grabbedObject.transform.position = hitDrop.transform.position;
                    _grabbedObject = null;

                }
            }
        }

        /*
        if (Physics.Raycast(_transform.position, transform.forward, out var hit, _interactionRadius, _interactableLayer))
        {
            if (hit.transform.TryGetComponent(out IInteractable interactableObject))
            {

                Debug.Log("interaction in reach");
                if (_canvas) _canvas.gameObject.SetActive(true);


                if (_playerInput.actions["Interact"].WasReleasedThisFrame())
                {
                    interactableObject.Interact(gameObject);
                    Debug.Log("interactableObject reached");
                }

            }
        }
        else
        {
            if(_canvas) _canvas.gameObject.SetActive(false);
        }*/
    }

    ///////VERSION 1
    ////////////////

    /* 
    private void OnEnable()
    {
        _playerInput.actions["Interact"].performed += OnInteract;
    }

    private void OnDisable()
    {
        _playerInput.actions["Interact"].performed -= OnInteract;
    }

    private void OnInteract(InputAction.CallbackContext callbackContext) 
    {

        Debug.Log("E - interact");

        if(Physics.Raycast(_transform.position, transform.forward, out var hit, _interactionRadius, _interactableLayer)) 
        { 
            if(hit.transform.TryGetComponent(out IInteractable interactableObject)) 
            {
                interactableObject.Interact(gameObject);
                Debug.Log("interactableObject reached");
            }
        }

    }*/


    ///////VERSION 3
    ////////////////

    /*
   [SerializeField] private Transform _interactionPoint;
   private readonly Collider[] _colliders = new Collider[1];

   private void Update()
   {
       int numFound = Physics.OverlapCapsuleNonAlloc(_interactionPoint.position, _interactionPoint.position, _interactionRadius, _colliders, _interactableLayer);
       bool canIntercat = false;

       if (numFound > 0)
       {
           IInteractable interactableObject = _colliders[0].GetComponent<IInteractable>();
           if (interactableObject != null)
           {

               Debug.Log("interaction in reach");
               canIntercat = true;


               if (_playerInput.actions["Interact"].WasReleasedThisFrame())
               {
                   interactableObject.Interact(gameObject);
                   Debug.Log("interactableObject reached");
               }

           }

       }
       _canvas.gameObject.SetActive(canIntercat);

   }*/
}
