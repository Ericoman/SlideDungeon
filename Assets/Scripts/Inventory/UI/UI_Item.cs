using Inventory;
using UnityEngine;
using UnityEngine.UI;

public class UI_Item : MonoBehaviour
{
    [SerializeField]
    private Image image;
    
    private ItemBase _itemData;

    public void Initialize(ItemBase itemData)
    {
        _itemData = itemData;
        InitializeUI();
    }

    void InitializeUI()
    {
        image.sprite = _itemData.itemData.Sprite;
    }
}
