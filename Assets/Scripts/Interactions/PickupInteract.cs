using Assets.Devs.Julia.Scripts;
using Inventory;
using NUnit.Framework.Interfaces;
using UnityEngine;

public class PickupInteract : MonoBehaviour, IInteractable
{
    [SerializeField]
    private ItemSO itemData;
    public void Interact(GameObject interactor)
    {
        if (interactor.tag == "Player")
        {
            InventoryManager inventory = interactor.gameObject.GetComponentInChildren<InventoryManager>();
            inventory.AddItem(itemData);
            Destroy(gameObject);
        }
    }


}
