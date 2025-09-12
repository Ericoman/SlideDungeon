using Assets.Devs.Julia.Scripts;
using Assets.Scripts.Interactions;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class Interactor : MonoBehaviour
{
    [SerializeField] private float _interactionRadius = 1.5f;
    [SerializeField] private LayerMask _interactableLayer;
    [SerializeField] private LayerMask _obstacleLayer;

    [SerializeField] private float _maxDistanceToFloor = 2;
    public float holdThreshold = 0.5f;

    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private DeviceDetector deviceDetector;
    public Animator anim;


    [Header("Interaction info")]
    [SerializeField] private Canvas _canvas;
    [SerializeField] private Sprite _keboardImg;
    [SerializeField] private Sprite _gamepadImg; 
    private Image _image;

    private PlayerInput _playerInput;
    private InputAction _interactAction;
    private Transform _transform;

    private Outline _otlineLastSeen; 
    private GameObject _grabbedObject = null;
    private IInteractableHeld _heldObject = null;

    private float _interactHeldTime = 0f;


    private void Awake()
    {
        _transform = transform;
        _playerInput = GetComponent<PlayerInput>();
        _interactAction = _playerInput.actions["Interact"];
        _image = _canvas.GetComponentInChildren<Image>();
    }


    public void SetGrabbedObject(GameObject grabbedObject) 
    {
        _grabbedObject = grabbedObject;
        if (_canvas && grabbedObject != null) _canvas.gameObject.SetActive(false);
    }

    public void SetOtline(RaycastHit hit, bool visible) 
    {
        if (visible) 
        {
            //set UI 
            if (_canvas != null && _image != null) 
            {
                if (deviceDetector.IsUsingKeyboard())
                {
                    _image.sprite = _keboardImg;
                }
                else 
                {
                    _image.sprite = _gamepadImg;
                }
                
                _canvas.gameObject.SetActive(true);
            }


            //set outline
            Outline _aux = _otlineLastSeen;
            _otlineLastSeen = hit.transform.GetComponent<Outline>() ??
                                hit.transform.GetComponentInParent<Outline>() ??
                                hit.transform.GetComponentInChildren<Outline>();
            if (_otlineLastSeen)
            {
                _otlineLastSeen.enabled = true;
                if (_aux && _aux.transform.root != _otlineLastSeen.transform.root)
                {
                    _aux.enabled = false;
                }
            }
        }
        else 
        {
            if (_canvas) _canvas.gameObject.SetActive(false);
            if (_otlineLastSeen)
            {
                _otlineLastSeen.enabled = false;
                _otlineLastSeen = null;
            }
        }
    }

    ///////VERSION 2
    ////////////////

    private void Update()
    {
        //EXCEPTIONS - exit not normal game modes
        if (_interactAction.WasReleasedThisFrame()) 
        {
            //Close menu if oppen
            GameObject messageUI = GameObject.FindGameObjectWithTag("MessageUI");
            if (messageUI != null) 
            {
                Destroy(messageUI);
                Time.timeScale = 1f;
                return;
            }
        }
        
        

        if (_grabbedObject == null) //not grabbing
        {
            if (Physics.Raycast(_transform.position, transform.forward, out var hit, _interactionRadius, _interactableLayer | _obstacleLayer))
            {
                int objectLayerMask = 1 << hit.transform.gameObject.layer;
                if ((_obstacleLayer.value & objectLayerMask) != 0)
                {
                    SetOtline(hit, false);

                    if (_heldObject != null)
                    {
                        playerMovement.SetCanMove(true);
                        _heldObject = null;
                    }

                    return;
                }

                IInteractable interactableObject = null;
                hit.transform.TryGetComponent(out interactableObject);
                IInteractableHeld interactableHeldObject = null;
                hit.transform.TryGetComponent(out interactableHeldObject);

                if (interactableObject != null || interactableHeldObject != null)
                {
                    SetOtline(hit, true);

                    //Diferenciate if holding or just press considerring witch acction is available
                    if (interactableHeldObject != null && _interactAction.IsPressed()) //HOLDING
                    {
                        _interactHeldTime += Time.deltaTime;

                        if (_interactHeldTime >= holdThreshold) //---> holding action 
                        {
                            _heldObject = interactableHeldObject;
                            interactableHeldObject.InteractHeld(gameObject);
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
                            if(anim) anim.SetTrigger("Interact");
                        }
                        _interactHeldTime = 0f;

                        if (interactableHeldObject != null) 
                        {
                            interactableHeldObject.InteractHeldRelease(gameObject);
                            _heldObject = null;
                        }
                    }
                }
            }
            else
            {
                SetOtline(hit, false);
                if (_heldObject != null) 
                {
                    playerMovement.SetCanMove(true);
                    _heldObject = null;
                }
            }
        }
        else //grabing -> DROP
        {
            if (_interactAction.WasReleasedThisFrame()) //try drop 
            {
                bool isHitDrop = Physics.Raycast(_transform.position, transform.forward, out var hitDrop, _grabbedObject.transform.localScale.x + 1);
                //floor under
                bool isFloor = Physics.Raycast(_grabbedObject.transform.position, Vector3.down, _maxDistanceToFloor);


                if ((!isHitDrop || hitDrop.collider.gameObject == _grabbedObject) && (isFloor || (!isFloor && _heldObject != null))) //can drop
                {
                    if (!isFloor && _heldObject != null)
                    {
                        Rigidbody rigidbody = _grabbedObject.GetComponent<Rigidbody>();
                        if (rigidbody)
                        {
                            rigidbody.isKinematic = false;
                        }
                    }


                    GameObject boxArea = transform.Find("BoxArea").gameObject;
                    if (boxArea) boxArea.SetActive(false);

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
                    _heldObject = null;

                    playerMovement.SetFreeMovement();

                }
            }

            if (!Physics.Raycast(_transform.position, transform.forward, out var hit, _interactionRadius, _interactableLayer))
            {
                if (_heldObject != null)
                {
                    _grabbedObject.transform.SetParent(null);
                    _grabbedObject = null;
                    _heldObject = null;

                    playerMovement.SetFreeMovement();

                }
            } 
        }
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
