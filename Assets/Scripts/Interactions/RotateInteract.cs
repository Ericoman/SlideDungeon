using Assets.Devs.Julia.Scripts;
using Assets.Scripts.Interactions;
using UnityEngine;

public class RotateInteract : MonoBehaviour, IInteractableHeld
{
    public enum RotationAxis { X, Y, Z }
    [SerializeField] private RotationAxis axis = RotationAxis.Y;
    [SerializeField] private float step = 45f;

    public void InteractHeldRight(GameObject interactor)
    {
        Rotate(-step);
    }

    public void InteractHeldLeft(GameObject interactor)
    {
        Rotate(step);
    }

    private void Rotate(float angle)
    {
        Vector3 rotationVector = Vector3.zero;

        switch (axis)
        {
            case RotationAxis.X: 
                rotationVector = Vector3.right * angle; 
                break;
            case RotationAxis.Y: 
                rotationVector = Vector3.up * angle; 
                break;
            case RotationAxis.Z: 
                rotationVector = Vector3.forward * angle; 
                break;
        }

        transform.Rotate(rotationVector, Space.Self);
    }
}
