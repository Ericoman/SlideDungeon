using Assets.Devs.Julia.Scripts;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class GrabInteract : MonoBehaviour, IInteractable
{
    public void Interact(GameObject interactor)
    {
        if (interactor.tag == "Player")
        {
            Interactor interactorComponent = interactor.gameObject.GetComponentInChildren<Interactor>();
            if (interactorComponent) 
            { 
                interactorComponent.SetGrabbedObject(gameObject);

                gameObject.transform.SetParent(interactor.transform);
                //gameObject.transform.localRotation = Quaternion.identity;
                gameObject.transform.localRotation = Quaternion.Euler(0f, -90f, 0f);
                gameObject.transform.position = interactor.transform.Find("GrabPoint").position;

            }
        }
    }
}