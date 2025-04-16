using UnityEngine;

namespace Inventory
{
    [CreateAssetMenu(fileName = "ItemSO", menuName = "Scriptable Objects/Inventory/ItemSO")]
    public class ItemSO : ScriptableObject
    {
        public int Id;
        public string Name;
        public Sprite Sprite;
        public string Description;
        public int MaxAmmount;
        public ItemBase InstancePrefab;
    }
}
