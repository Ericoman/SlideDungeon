using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class TeslaCoil : MonoBehaviour
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

    private bool _isActive = false;
    
    public GameObject thunderDetector;
    
    public float checkInterval = 0.2f; // Interval for checking child objects
    
    public float activeDuration = 5f;
    
    private bool isHitByRaycast = false;
    private float raycastActiveTimeout = 1f;
    
    private Coroutine raycastDeactivationCoroutine;

    // Cached reference to `sphere2`
    private GameObject sphere2;
    public event Action<bool> Activated;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        thunderDetector.SetActive(false);
        
        // Begin periodic child check
        StartCoroutine(CheckForChildSphere());
    }

    // Update is called once per frame
    void Update()
    {

        // Update thunder detector visibility based on IsActive
        thunderDetector.SetActive(IsActive);
        
    }

    public void Activate()
    {
        IsActive = true;
        StartCoroutine(DeactivateAfterTime(activeDuration)); 
    }
    
    private void OnTriggerEnter(Collider other)
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
    }

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
                Debug.Log("TeslaCoil: sphere2 is no longer attached or has been destroyed.");
                sphere2 = null; 
                IsActive = false;
            }

            yield return new WaitForSeconds(checkInterval); // Re-check periodically
        }
    }

    private bool IsSphere2Attached()
    {
        if (sphere2 == null) return false;

        // Ensure sphere2 is a direct or indirect child of this TeslaCoil
        return sphere2.transform.IsChildOf(transform);
    }
    
    public void SetActiveWithSphere(GameObject sphere)
    {
        if (sphere2 == sphere)
        {
            Debug.LogWarning("TeslaCoil: This sphere2 is already attached.");
            return;
        }
        
        // Handle detachment if sphere is null
        if (sphere == null)
        {
            Debug.Log("TeslaCoil: Detaching sphere2 and deactivating Tesla Coil.");
            sphere2 = null;
            IsActive = false;
            return;
        }

        // Set the reference to the new sphere2
        sphere2 = sphere;

        // Keep the Tesla Coil active while sphere2 is attached
        IsActive = true;

        Debug.Log($"TeslaCoil: Activated with sphere2 '{sphere.name}'.");
    }
    
    public void ActivateByRaycast()
    {
        isHitByRaycast = true; // Set raycast hit flag
        if (!IsActive)
        {
            IsActive = true; // Activate the coil if not already active
        }

        // Restart the raycast timeout coroutine to extend active state
        if (raycastDeactivationCoroutine != null)
        {
            StopCoroutine(raycastDeactivationCoroutine);
        }

        raycastDeactivationCoroutine = StartCoroutine(RaycastDeactivationTimeout());
    }

    private IEnumerator RaycastDeactivationTimeout()
    {
        yield return new WaitForSeconds(raycastActiveTimeout);
        isHitByRaycast = false;

        // Deactivate if `sphere2` is not attached
        if (!IsSphere2Attached())
        {
            IsActive = false;
        }
    }
}
