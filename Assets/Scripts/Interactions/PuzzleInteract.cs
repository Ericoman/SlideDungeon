using Assets.Devs.Julia.Scripts;
using UnityEngine;
using UnityEngine.Serialization;

public class PuzzleInteract : MonoBehaviour, IInteractable
{
    [SerializeField] 
    private MeshRenderer outlineMeshRenderer;
    public void Interact(GameObject interactor)
    {
        if (interactor.tag == "Player")
        {
            PlayerMovement playerMovement = interactor.GetComponent<PlayerMovement>();
            if (playerMovement && !GameManager.Instance.movingCamera)
            {
                playerMovement.PuzzleInteract();
                outlineMeshRenderer.enabled = !playerMovement.GetPuzzleMode();
            }
        }
    }
}
