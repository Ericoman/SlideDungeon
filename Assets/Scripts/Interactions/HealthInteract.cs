using Assets.Devs.Julia.Scripts;
using UnityEngine;

public class HealthInteract : MonoBehaviour, IInteractable
{
    [SerializeField] 
    private int healthRecived = 1;

    

    public void Interact(GameObject interactor)
    {
        if (interactor.tag == "Player")
        {
            HealthComponent health = interactor.gameObject.GetComponentInChildren<HealthComponent>();
            health.ReceiveHealth(healthRecived);

            
            Destroy(gameObject);
        }
    }
}
