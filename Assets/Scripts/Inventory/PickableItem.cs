using System;
using Inventory;
using UnityEngine;

public class PickableItem : MonoBehaviour
{
    [SerializeField] 
    private ItemSO itemData;
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player") //Podrian añadirse mas tags como enemigos
        {
            InventoryManager inventory = other.gameObject.GetComponentInChildren<InventoryManager>();
            inventory.AddItem(itemData);
            Destroy(gameObject);
        }
    }
}
