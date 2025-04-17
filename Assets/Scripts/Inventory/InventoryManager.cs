using System;
using System.Collections.Generic;
using System.Linq;
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

    private int _lastAssignedSlot = -1;

    private void Awake()
    {
        _usableSlots = new ItemSO[_usableSlotsNumber];
    }

    private void Start()
    {
        _uiInventoryManager.Initialize(_usableSlotsNumber);
        _uiInventoryManager.ItemSelectedEvent += OnItemSelectedEvent;
        
        int i = 0;
        foreach (ItemSO item in _initialItems)
        {
            AddItem(item);

            //AssignItemToSlot(item,i);
            i++;
        }
        
    }

    private void OnItemSelectedEvent(ItemBase sender)
    {
        int nextSlot = _lastAssignedSlot + 1;
        for(int i=0; i < _usableSlots.Length;++i)
        {
            if ( _usableSlots[i] == null)
            {
                nextSlot = i;
                break;
            }
        }
        if (nextSlot >= _usableSlotsNumber)
        {
            nextSlot = 0;
        }
        AssignItemToSlot(sender.itemData,nextSlot);
    }

    public void AddItem(ItemSO item)
    {
        if (_itemInstances.ContainsKey(item))
        {
            _itemInstances[item].IncreaseAmount();
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
            itemBase.DecreaseAmount();
            if (itemBase.CurrentAmount <= 0)
            {
                _itemInstances.Remove(item);
                
                _uiInventoryManager.RemoveItem(itemBase); //TODO Make with event but careful with destruction
                
                Destroy(itemBase.gameObject);
                
            }
        }
    }

    public void AssignItemToSlot(ItemSO item, int slot)
    {
        if (_usableSlots.Contains(item))
        {
            int oldIndex = Array.IndexOf(_usableSlots, item);
            _usableSlots[oldIndex] = null;
            _uiInventoryManager.AssignItemToSlot(null, oldIndex); //TODO make with event
            return;
        }
        _usableSlots[slot] = item;
        
        _uiInventoryManager.AssignItemToSlot(_itemInstances[item], slot); //TODO make with event
        
        _lastAssignedSlot = slot;
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
        if (item == null) return;
        ItemBase itemInstance = _itemInstances[item];
        if (itemInstance is IUsable usable)
        {
            usable.Use();
        }
    }

    private void OnDestroy()
    {
        _uiInventoryManager.ItemSelectedEvent -= OnItemSelectedEvent;
    }

    public void OpenCloseInventory()
    {
        _uiInventoryManager.OpenCloseInventory();
    }
}
