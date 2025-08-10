using Assets.Devs.Julia.Scripts;
using UnityEngine;

public class SmokeInteract : MonoBehaviour, IInteractable
{
    [SerializeField]
    private int healthRecived = 1;

    public void Interact(GameObject interactor)
    {
        if (interactor.tag == "Player")
        {
            PlayersHealthComponent playersHealth = interactor.gameObject.GetComponentInChildren<PlayersHealthComponent>();
            playersHealth.ReceiveExtraHealth(healthRecived);
        }
    }
}
