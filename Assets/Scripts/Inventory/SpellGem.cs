using Inventory;
using UnityEngine;

public class SpellGem: ItemBase,IUsable
{
    [SerializeField]
    private SpellBase spell;  
    
    public void Use()
    {
        spell.CastSpell();
    }
}
