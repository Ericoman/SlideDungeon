using Assets.Devs.Julia.Scripts;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class Interactor : MonoBehaviour
{
    [SerializeField] private float _interactionRadius = 1.5f;
    [SerializeField] private LayerMask _interactableLayer;
    [SerializeField] private Canvas _canvas;


    private PlayerInput _playerInput;
    private Transform _transform;


    private void Awake()
    {
        _transform = transform;
        _playerInput = GetComponent<PlayerInput>();
    }

    ///////VERSION 2
    ////////////////

    private void Update()
    {
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
