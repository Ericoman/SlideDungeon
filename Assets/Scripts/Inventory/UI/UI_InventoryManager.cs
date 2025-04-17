using System;
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
    
    [SerializeField] 
    private Transform slotsContent;
    [SerializeField] 
    private UI_Showable slotsUI;

    private UI_Item[] _slotItems;
    
    Dictionary<ItemBase,UI_Item> _items = new Dictionary<ItemBase,UI_Item>();

    public delegate void ItemSelectedEventHandler(ItemBase sender);
    public event ItemSelectedEventHandler ItemSelectedEvent;

    private void Start()
    {
        inventoryUI.Show(false);
    }

    public void Initialize(int slotCount)
    {
        _slotItems = new UI_Item[slotCount];
        for (int i = 0; i < slotCount; i++)
        {
            _slotItems[i] = Instantiate<UI_Item>(inventoryItemPrefab, slotsContent);
            _slotItems[i].Initialize(null);
        }
    }
    public void AddItem(ItemBase item)
    {
        UI_Item newItem = Instantiate<UI_Item>(inventoryItemPrefab, itemsContent);
        newItem.Initialize(item);
        _items.Add(item,newItem);
        newItem.ItemSelectedEvent += OnItemSelected;
    }

    private void OnItemSelected(UI_Item sender)
    {
        ItemSelectedEvent?.Invoke(sender.ItemData);
    }

    public void RemoveItem(ItemBase item)
    {
        UI_Item itemToRemove = _items[item];
        itemToRemove.ItemSelectedEvent -= OnItemSelected;
        Destroy(itemToRemove.gameObject);
    }

    public void AssignItemToSlot(ItemBase item, int slot)
    {
        _slotItems[slot].Initialize(item);
    }

    public void OpenCloseInventory()
    {
        inventoryUI.Show(!inventoryUI.IsShown);
    }
}
