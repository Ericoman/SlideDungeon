using UnityEngine;

public class DisableAfterTime : MonoBehaviour
{
    public float disableDelay = 3f; // Time in seconds before disabling components

    private void Start()
    {
        // Start the countdown as soon as the object is instantiated
        Invoke(nameof(DisableComponents), disableDelay);
    }

    private void DisableComponents()
    {
        // Disable all child MeshRenderers
        foreach (MeshRenderer meshRenderer in GetComponentsInChildren<MeshRenderer>())
        {
            meshRenderer.enabled = false;
        }

        // Disable its own Collider (if any)
        Collider collider = GetComponent<Collider>();
        if (collider != null)
        {
            collider.enabled = false;
        }
    }
}