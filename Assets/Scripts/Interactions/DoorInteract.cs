using UnityEngine;
using Assets.Devs.Julia.Scripts;

public class DoorInteract : MonoBehaviour, IInteractable
{
    [SerializeField] private float _movementRange = 2f;

    private bool _isClosed = true;
    public void Interact(GameObject interactor)
    {
        Debug.Log("DOOR OPEN");
        if (_isClosed) 
        {
            transform.Translate(Vector3.right * _movementRange); // Mueve en X
            _isClosed = false;
        }
        else 
        {
            transform.Translate(Vector3.right * -_movementRange); // Mueve en X
            _isClosed = true;
        }
        
    }
}
