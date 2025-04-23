using Assets.Devs.Julia.Scripts;
using UnityEngine;

public class HatGoldInteract : MonoBehaviour, IInteractable
{
    [SerializeField] Material materialGold;
    public void Interact(GameObject interactor)
    {
        if (interactor.tag == "Player")
        {
            GameObject hat = interactor.transform.Find("Mage_Hat_01")?.gameObject;
            if (hat != null)
            {
                SkinnedMeshRenderer smr = hat.GetComponentInChildren<SkinnedMeshRenderer>(true);

                if (smr != null)
                {
                    smr.materials = new Material[] { materialGold };
                }

                hat.SetActive(true);
            }
            Destroy(gameObject);
        }
    }
}