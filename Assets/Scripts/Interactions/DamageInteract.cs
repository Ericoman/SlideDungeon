using Assets.Devs.Julia.Scripts;
using UnityEngine;

public class DamageInteract : MonoBehaviour, IInteractable
{
    [SerializeField]
    private int damageRecived = 1;
    public void Interact(GameObject interactor)
    {
        HealthComponent health = gameObject.GetComponentInChildren<HealthComponent>();
        health.ReceiveDamage(damageRecived);        
    }
}
