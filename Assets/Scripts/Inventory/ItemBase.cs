using UnityEngine;

namespace Inventory
{
    [System.Serializable]
    public class ItemBase:MonoBehaviour
    {
        public ItemSO itemData;
        private int _currentAmount = 0;
        
        public delegate void AmmountChangedEventHandler();
        public event AmmountChangedEventHandler AmountChangedEvent;
        
        public int CurrentAmount => _currentAmount;

        public void Initialize(ItemSO _itemData)
        {
            this.itemData = _itemData;
            IncreaseAmount(1);
        }
        public void IncreaseAmount(int amount=1)
        {
            _currentAmount += amount;
            AmountChangedEvent?.Invoke();
        }

        public void DecreaseAmount(int amount=1)
        {
            _currentAmount -= amount;
            AmountChangedEvent?.Invoke();
        }
    }
}
