using System;
using System.Collections;
using UnityEngine;

public class MirrorReflection : MonoBehaviour
{
    public bool IsActive
    {
        get => _isActive;
        set
        {
            _isActive = value;
            Activated?.Invoke(_isActive);
        }
    }

    public Transform reflectionDirection;
    
    [SerializeField] private bool _isActive = false;
    
    public float checkInterval = 0.2f; // Interval for checking child objects
    
    public float activeDuration = 5f;
    
    private TeslaCoil lastHitTeslaCoil; // To store the Tesla Coil hit by the raycast

    public float raycastInterval = 0.1f; // Interval for performing raycasts
    private Coroutine raycastCoroutine;
    private Coroutine deactivateCoroutine;
    private bool isBeingHitByRaycast = false; // Tracks if this mirror is being hit by a raycast
    public float deactivateTimeout = 0.5f; // Time before deactivating if no longer hit by a raycast
    
    // Cached reference to `sphere2`
    private GameObject sphere2;
    public event Action<bool> Activated;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Begin periodic child check
        StartCoroutine(CheckForChildSphere());
    }

    // Update is called once per frame
    void Update()
    {
        if (IsActive)
        {
            PerformRaycast();
        }
        
    }

    public void Activate()
    {
        if (!IsActive) // Avoid reactivating the same mirror
        {
            IsActive = true;

            // Start the raycasting coroutine if it's not already running
            if (raycastCoroutine == null)
            {
                raycastCoroutine = StartCoroutine(RaycastRoutine());
            }
        } 
        
        if (deactivateCoroutine != null)
        {
            StopCoroutine(deactivateCoroutine);
        }

        // Mark as being hit by a raycast
        isBeingHitByRaycast = true;
    }
    
    /*private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("ThunderLink"))
        {
            Activate();
        }
        else if (other.gameObject.name == "ThunderSphere_2") // Optional: Check specifically for sphere2
        {
            sphere2 = other.gameObject;
            IsActive = true; // Stay active as sphere2 is present
        }
    }
    
    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("ThunderLink"))
        {
            IsActive = true;
        }
    }*/

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.name == "sphere2")
        {
            sphere2 = null; // Reset reference to sphere2
        }
    }

    private IEnumerator DeactivateAfterTime(float duration)
    {
        yield return new WaitForSeconds(duration);

        // Deactivate only if no `sphere2` is attached
        if (!IsSphere2Attached())
        {
            IsActive = false;
        }
    }
    
    private IEnumerator CheckForChildSphere()
    {
        while (true)
        {
            // Determine if sphere2 exists as a child
            if (IsSphere2Attached())
            {
                IsActive = true;
            }
            else
            {
                // Clear references if sphere2 no longer exists
                Debug.Log("Mirror: sphere2 is no longer attached or has been destroyed.");
                sphere2 = null; 
                IsActive = false;
            }

            yield return new WaitForSeconds(checkInterval); // Re-check periodically
        }
    }

    private bool IsSphere2Attached()
    {
        if (sphere2 == null) return false;

        // Ensure sphere2 is a direct or indirect child of this Mirror
        return sphere2.transform.IsChildOf(transform);
    }
    
    public void SetActiveWithSphere(GameObject sphere)
    {
        if (sphere2 == sphere)
        {
            Debug.LogWarning("Mirror: This sphere2 is already attached.");
            return;
        }
        
        // Handle detachment if sphere is null
        if (sphere == null)
        {
            Debug.Log("Mirror: Detaching sphere2 and deactivating Tesla Coil.");
            sphere2 = null;
            IsActive = false;
            return;
        }

        // Set the reference to the new sphere2
        sphere2 = sphere;

        // Keep the Mirror active while sphere2 is attached
        IsActive = true;

        Debug.Log($"Mirror: Activated with sphere2 '{sphere.name}'.");
        
    }

    private void PerformRaycast()
    {
        if (reflectionDirection == null) return;

        Ray reflectionRay = new Ray(reflectionDirection.position, reflectionDirection.forward);
        Debug.DrawRay(reflectionDirection.position, reflectionDirection.forward * 100f, Color.yellow);
        
        // Create a LayerMask that excludes the Player layer
        int layerMask = ~LayerMask.GetMask("Player"); // Exclude the "Player" layer

        if (Physics.Raycast(reflectionRay, out RaycastHit hitInfo, Mathf.Infinity, layerMask))
        {
            // Check if raycast hits a TeslaCoil
            TeslaCoil teslaCoil = hitInfo.collider.GetComponent<TeslaCoil>();
            if (teslaCoil != null)
            {
                teslaCoil.ActivateByRaycast();
            }

            // Check if raycast hits another mirror
            MirrorReflection otherMirror = hitInfo.collider.GetComponent<MirrorReflection>();
            if (otherMirror != null && !otherMirror.IsActive)
            {
                otherMirror.Activate(); // Activate the other mirror
            }
        }

        // If no raycast hit, mark the mirror as "not being hit"
        isBeingHitByRaycast = false;

        // Start the deactivation timeout if this mirror is no longer hit
        if (deactivateCoroutine == null)
        {
            deactivateCoroutine = StartCoroutine(DeactivateAfterTimeout());
        }
    }
    
    private IEnumerator RaycastRoutine()
    {
        while (IsActive)
        {
            PerformRaycast(); // Perform the raycast to check for TeslaCoils or other mirrors
            yield return new WaitForSeconds(raycastInterval); // Wait for the next cast
        }
    }
    
    public void Deactivate()
    {
        IsActive = false;

        // Stop raycasting when the mirror is deactivated
        if (raycastCoroutine != null)
        {
            StopCoroutine(raycastCoroutine);
            raycastCoroutine = null;
        }

        isBeingHitByRaycast = false; // Reset the hit flag
    }
    
    private IEnumerator DeactivateAfterTimeout()
    {
        yield return new WaitForSeconds(deactivateTimeout);

        // Deactivate if not being hit by a raycast during the timeout
        if (!isBeingHitByRaycast)
        {
            Deactivate();
        }

        deactivateCoroutine = null; // Clear coroutine reference
    }
}
