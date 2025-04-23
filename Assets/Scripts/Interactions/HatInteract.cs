using Assets.Devs.Julia.Scripts;
using UnityEngine;

public class HatInteract : MonoBehaviour, IInteractable
{
    public void Interact(GameObject interactor)
    {
        if (interactor.tag == "Player")
        {
            GameObject hat = interactor.transform.Find("Mage_Hat_01")?.gameObject;
            if (hat != null)
            {
                hat.SetActive(true); // O false si lo querés ocultar
            }
            Destroy(gameObject);
        }
    }
}
