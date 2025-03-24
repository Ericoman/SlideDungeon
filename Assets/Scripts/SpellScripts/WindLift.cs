using System.Collections;
using UnityEngine;

public class WindLift : MonoBehaviour
{
    public LayerMask windableLayer; // The layer mask for detecting specific objects

    private void Start()
    {
        // Perform an initial overlap check for objects already inside the trigger
        PerformInitialDetection();
    }

    private void PerformInitialDetection()
    {
        // Get the SphereCollider attached to the GameObject
        SphereCollider sphereCollider = GetComponent<SphereCollider>();

        if (sphereCollider != null)
        {
            Vector3 position = transform.position + sphereCollider.center; // Account for collider center offset if any
            float radius = sphereCollider.radius * Mathf.Max(transform.lossyScale.x, transform.lossyScale.y, transform.lossyScale.z); // Handle scaling

            // Detect objects already inside the collider volume
            Collider[] hitColliders = Physics.OverlapSphere(position, radius, windableLayer);

            foreach (Collider col in hitColliders)
            {
                HandleCollision(col);
            }
        }
        else
        {
            Debug.LogWarning("No SphereCollider found on this object!");
        }
    }
    
    private void OnTriggerEnter(Collider other)
    {
        HandleCollision(other); // Handle objects entering the trigger normally
    }

    private void HandleCollision(Collider other)
    {
        // Check if the other object is in the windable layer
        if (((1 << other.gameObject.layer) & windableLayer) != 0)
        {
            // Check if the object has the WindSpellCast component
            WindSpellCast windSpellCast = other.GetComponent<WindSpellCast>();

            if (windSpellCast != null)
            {
                Debug.Log($"{other.gameObject.name} has the WindSpellCast component.");
                StartCoroutine(LiftAndLowerObject(other.gameObject));
            }
            else
            {
                Debug.Log($"{other.gameObject.name} does not have the WindSpellCast component.");
                StartCoroutine(LiftAndLowerObject(other.gameObject));
            }
        }
    }

    private IEnumerator LiftAndLowerObject(GameObject obj)
    {
        float originalY = obj.transform.position.y; // Store only the original Y position of the object
        float targetY = originalY + 5f; // Set the target Y-axis height (e.g., 5 units above)

        float duration = 1.5f; // Duration for lifting and lowering
        float elapsed = 0f;

        // Phase 1: Lift the object (modify only Y-axis)
        while (elapsed < duration)
        {
            float newY = Mathf.Lerp(originalY, targetY, elapsed / duration); // Interpolate Y-axis
            obj.transform.position = new Vector3(obj.transform.position.x, newY, obj.transform.position.z); // Update Y, keep X and Z dynamic
            elapsed += Time.deltaTime;
            yield return null;
        }

        // Ensure the object reaches the exact target Y position after the lift
        obj.transform.position = new Vector3(obj.transform.position.x, targetY, obj.transform.position.z);

        // Optional: Pause at the top
        yield return new WaitForSeconds(0.5f);

        // Reset elapsed time for lowering phase
        elapsed = 0f;

        // Phase 2: Lower the object back down (modify only Y-axis)
        while (elapsed < duration)
        {
            float newY = Mathf.Lerp(targetY, originalY, elapsed / duration); // Interpolate Y-axis back
            obj.transform.position = new Vector3(obj.transform.position.x, newY, obj.transform.position.z); // Update Y, keep X and Z dynamic
            elapsed += Time.deltaTime;
            yield return null;
        }

        // Ensure the object reaches back to the exact original Y position
        obj.transform.position = new Vector3(obj.transform.position.x, originalY, obj.transform.position.z);
        
        Destroy(gameObject);
    }
}
