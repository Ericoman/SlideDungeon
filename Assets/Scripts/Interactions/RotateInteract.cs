using Assets.Devs.Julia.Scripts;
using Assets.Scripts.Interactions;
using UnityEngine;

public class RotateInteract : MonoBehaviour, IInteractableHeld
{
    public void InteractHeldRight(GameObject interactor)
    {
        Vector3 localEuler = gameObject.transform.localEulerAngles;
        localEuler.y -= 45f;
        gameObject.transform.localRotation = Quaternion.Euler(localEuler);
    }
    public void InteractHeldLeft(GameObject interactor)
    {
        Vector3 localEuler = gameObject.transform.localEulerAngles;
        localEuler.y += 45f;
        gameObject.transform.localRotation = Quaternion.Euler(localEuler);
    }
}
