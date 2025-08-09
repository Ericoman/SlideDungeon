using Assets.Devs.Julia.Scripts;
using UnityEngine;

public class DamageInteract : MonoBehaviour, IInteractable
{
    [SerializeField]
    private int damageProduced = 1;
    public void Interact(GameObject interactor)
    {
        HealthComponent health = gameObject.GetComponentInChildren<HealthComponent>();
        health.ReceiveDamage(damageProduced);        
    }
}
