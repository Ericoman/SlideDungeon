using System.Collections.Generic;
using Inventory;
using UnityEngine;

public class UI_InventoryManager : MonoBehaviour
{
    [SerializeField] 
    private Transform itemsContent;
    [SerializeField]
    private UI_Showable inventoryUI;
    [SerializeField]
    private UI_Item inventoryItemPrefab;
    
    Dictionary<ItemBase,UI_Item> _items = new Dictionary<ItemBase,UI_Item>();
    public void AddItem(ItemBase item)
    {
        UI_Item newItem = Instantiate<UI_Item>(inventoryItemPrefab, itemsContent);
        newItem.Initialize(item);
        _items.Add(item,newItem);
    }

    public void RemoveItem(ItemBase item)
    {
        UI_Item itemToRemove = _items[item];
        Destroy(itemToRemove.gameObject);
    }
}
