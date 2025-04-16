using System;
using System.Collections.Generic;
using Inventory;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

public class InventoryManager : MonoBehaviour
{
    [SerializeField]
    private List<ItemSO> _initialItems = new List<ItemSO>();

    [SerializeField] 
    private Transform _itemsContainer;
    [SerializeField]
    private int _usableSlotsNumber = 2;
    [SerializeField]
    private UI_InventoryManager _uiInventoryManager;
    
    private ItemSO[] _usableSlots;
    private Dictionary<ItemSO, ItemBase> _itemInstances = new Dictionary<ItemSO, ItemBase>();
    
    public List<ItemSO> InitialItems => _initialItems;
    

    private void Awake()
    {
        _usableSlots = new ItemSO[_usableSlotsNumber];
    }

    private void Start()
    {
        int i = 0;
        foreach (ItemSO item in _initialItems)
        {
            AddItem(item);

            _usableSlots[i] = item;//TODO hacer bien
            i++;
        }
    }

    public void AddItem(ItemSO item)
    {
        if (_itemInstances.ContainsKey(item))
        {
            _itemInstances[item].IncreaseAmmount();
            //TODO make updateui event
        }
        else
        {
            ItemBase itemBase = Instantiate(item.InstancePrefab, _itemsContainer);
            itemBase.Initialize(item);
            _itemInstances.Add(item, itemBase);
            _uiInventoryManager.AddItem(itemBase); //TODO make with event
        }
    }

    public void RemoveItem(ItemSO item)
    {
        if (_itemInstances.ContainsKey(item))
        {
            ItemBase itemBase = _itemInstances[item];
            itemBase.DecreaseAmmount();
            if (itemBase.CurrentAmmount <= 0)
            {
                _itemInstances.Remove(item);
                
                _uiInventoryManager.RemoveItem(itemBase); //TODO Make with event but careful with destruction
                
                Destroy(itemBase.gameObject);
                
            }
        }
    }

    public void AssignItemToSlot(ItemSO item, int slot)
    {
        _usableSlots[slot] = item;
    }

    public void UseSlot(int slot)
    {
        if (slot >= 0 && slot < _usableSlotsNumber)
        {
            UseItem(_usableSlots[slot]);
        }
    }
    public void UseItem(ItemSO item)
    {
        ItemBase itemInstance = _itemInstances[item];
        if (itemInstance is IUsable usable)
        {
            usable.Use();
        }
    }
}
