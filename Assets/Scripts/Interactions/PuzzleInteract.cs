using Assets.Devs.Julia.Scripts;
using UnityEngine;

public class PuzzleInteract : MonoBehaviour, IInteractable
{
    public void Interact(GameObject interactor)
    {
        if (interactor.tag == "Player")
        {
            PlayerMovement playerMovement = interactor.GetComponent<PlayerMovement>();
            if (playerMovement)
            {
                playerMovement.PuzzleInteract();
            }
        }
    }
}
