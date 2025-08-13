using Assets.Devs.Julia.Scripts;
using UnityEngine;

public class HatInteract : MonoBehaviour, IInteractable
{
    public void Interact(GameObject interactor)
    {
        if (interactor.tag == "Player")
        {
            GameObject hat = interactor.transform.Find("Hat 02 Brown")?.gameObject;
            if (hat != null)
            {
                hat.SetActive(true);
            }
            Destroy(gameObject);
        }
    }
}
