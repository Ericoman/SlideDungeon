using UnityEngine;

namespace Inventory
{
    [System.Serializable]
    public class ItemBase:MonoBehaviour
    {
        public ItemSO itemData;
        private int _currentAmmount = 0;
        
        public int CurrentAmmount => _currentAmmount;

        public void Initialize(ItemSO _itemData)
        {
            this.itemData = _itemData;
            IncreaseAmmount(1);
        }
        public void IncreaseAmmount(int amount=1)
        {
            _currentAmmount += amount;
        }

        public void DecreaseAmmount(int amount=1)
        {
            _currentAmmount -= amount;
        }
    }
}
