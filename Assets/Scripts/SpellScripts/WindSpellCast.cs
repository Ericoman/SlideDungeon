using UnityEngine;
using System.Collections;

public class WindSpellCast : SpellBase
{
    public Vector3 spellOffset = Vector3.zero; // Offset position for the OverlapSphere
    public GameObject windSpellPrefab; // Prefab for the spell
    public float spellRadius = 3f;     // Radius of the spell
    public LayerMask windableLayer;    // Layers affected by the wind spell

    public override void CastSpell()
    {
        CastWindSpell();
    }
    public void CastWindSpell()
    {
        // Calculate the position of the spell (in front of the player)
        Vector3 spellPosition = transform.position + transform.TransformDirection(spellOffset);

        // Instantiate the wind spell prefab
        GameObject spawnedSpell = Instantiate(windSpellPrefab, spellPosition, Quaternion.identity);

        // Adjust the prefab's collider size to match the spell radius
        SphereCollider spellCollider = spawnedSpell.GetComponent<SphereCollider>();
        if (spellCollider != null)
        {
            spellCollider.radius = spellRadius;
        }
        
    }
    
    private void OnDrawGizmos()
    {
        // Set the color for the Gizmos
        Gizmos.color = Color.blue;

        // Draw the spell area at the correct position
        Vector3 spellPosition = transform.position + transform.TransformDirection(spellOffset);
        Gizmos.DrawWireSphere(spellPosition, spellRadius);
    }
}
