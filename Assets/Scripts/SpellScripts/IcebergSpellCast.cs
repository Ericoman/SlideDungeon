using UnityEngine;

public class IcebergSpellCast : SpellBase
{
    public GameObject icebergPrefab; // Assign the Iceberg prefab in the inspector
    private GameObject currentIceberg; // Keep a reference to the instantiated iceberg
    private PlayerMovement playerMovement;

    void Start()
    {
        // Find the PlayerMovement script on the same GameObject (or explicitly assign it)
        playerMovement = GetComponent<PlayerMovement>();

        if (playerMovement == null)
        {
            Debug.LogError("PlayerMovement script not found on the player.");
        }
    }
    
    public override void CastSpell() // Directly callable by SpellHudManager
    {
        if (currentIceberg == null)
        {
            // First cast: Instantiate iceberg and lock movement
            CastIceberg();
        }
        else
        {
            // Second cast: Destroy iceberg and restore movement
            RemoveIceberg();
        }
    }
    
    private void CastIceberg()
    {
        // Instantiate the iceberg prefab at player's position
        currentIceberg = Instantiate(icebergPrefab, transform.position, Quaternion.identity);

        // Disable player movement
        if (playerMovement != null)
        {
            playerMovement.enabled = false;
        }
    }

    private void RemoveIceberg()
    {
        // Destroy the existing iceberg
        if (currentIceberg != null)
        {
            Destroy(currentIceberg);
            currentIceberg = null;

            // Enable player movement
            if (playerMovement != null)
            {
                playerMovement.enabled = true;
            }
        }
    }
}
