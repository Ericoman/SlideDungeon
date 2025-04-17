using Inventory;
using TMPro;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;

public class UI_Item : MonoBehaviour
{
    [SerializeField]
    private Image image;

    [SerializeField] 
    private TextMeshProUGUI ammount;
    
    private ItemBase _itemData;
    private Button _button;
    
    public ItemBase ItemData => _itemData;
    
    public delegate void ItemSelectedEventHandler(UI_Item sender);
    public event ItemSelectedEventHandler ItemSelectedEvent;

    public void Initialize(ItemBase itemData)
    {
        if (itemData == null)
        {
            Clear();
        }
        else if(_itemData != itemData)
        {
            _itemData = itemData;
            itemData.AmountChangedEvent += UpdateAmount;
            InitializeUI();
        }
        
        _button = GetComponent<Button>();
        _button.onClick.AddListener(Select);
    }

    public void Clear()
    {
        if (_itemData != null)
        {
            _itemData.AmountChangedEvent -= UpdateAmount;
        }
        _itemData = null;
        image.sprite = null;
        ammount.text = "";
    }
    void InitializeUI()
    {
        image.sprite = _itemData.itemData.Sprite;
        UpdateAmount();
    }

    void UpdateAmount()
    {
        ammount.text = _itemData.CurrentAmount.ToString();
    }

    public void Select()
    {
        ItemSelectedEvent?.Invoke(this);
    }
}
