using System;
using System.Collections;
using UnityEngine;
using UnityEngine.VFX;
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

    [Header("VFX Graph")]
    public VisualEffect vfx;
    public string pos1Property = "Pos1";
    public string pos2Property = "Pos2";
    public string pos3Property = "Pos3";
    public string pos4Property = "Pos4";
    public bool vfxUsesLocalSpace = false;

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
            if (vfx != null) vfx.Play();
        }

        if (deactivateCoroutine != null)
        {
            StopCoroutine(deactivateCoroutine);
        }

        // Mark as being hit by a raycast
        isBeingHitByRaycast = true;
    }

    public void MarkAsHitByRaycast()
    {
        isBeingHitByRaycast = true;

        // reinicia timeout de apagado
        if (deactivateCoroutine != null)
        {
            StopCoroutine(deactivateCoroutine);
            deactivateCoroutine = null;
        }

        if (!IsActive) Activate();
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

        Vector3 origin = reflectionDirection.position;
        Vector3 dir = reflectionDirection.forward;
        int layerMask = ~LayerMask.GetMask("Player", "Room");

        if (Physics.Raycast(origin, dir, out RaycastHit hit, Mathf.Infinity, layerMask))
        {
            // --- actualizar VFX ---
            Vector3 hitPoint = hit.point;
            Vector3 midPoint = (origin + hitPoint) * 0.5f;
            Vector3 direction1 = (hitPoint - origin).normalized;
            Vector3 offset = Vector3.Cross(direction1, Vector3.up) * UnityEngine.Random.Range(-0.5f, 0.5f);
            Vector3 pos2 = Vector3.Lerp(origin, midPoint, 0.5f) + offset;
            Vector3 pos3 = Vector3.Lerp(midPoint, hitPoint, 0.5f) - offset;

            if (vfx != null)
            {
                if (!vfx.enabled) vfx.Play();
                vfx.SetVector3(pos1Property, origin);
                vfx.SetVector3(pos2Property, pos2);
                vfx.SetVector3(pos3Property, pos3);
                vfx.SetVector3(pos4Property, hitPoint);
            }

            // --- lógica puzzle ---
            TeslaCoil coil = hit.collider.GetComponent<TeslaCoil>();
            if (coil != null) coil.ActivateByRaycast();

            MirrorReflection otherMirror = hit.collider.GetComponent<MirrorReflection>();
            if (otherMirror != null)
            {
                otherMirror.MarkAsHitByRaycast(); 
            }

            // este espejo sigue activo porque lanza un rayo válido
            isBeingHitByRaycast = true;
        }
        else
        {
            // If not hit, stop VFX and handle deactivation
            if (vfx != null && vfx.enabled)
            {
                Debug.Log("Stopping VFX");
                vfx.Stop();
            }

            isBeingHitByRaycast = false; // Reset raycast status

            if (deactivateCoroutine == null)
            {
                deactivateCoroutine = StartCoroutine(DeactivateAfterTimeout());
            }
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
        Debug.Log("Mirror: Deactivating.");
        IsActive = false;

        // Stop raycasting when the mirror is deactivated
        if (raycastCoroutine != null)
        {
            StopCoroutine(raycastCoroutine);
            raycastCoroutine = null;
        }

        isBeingHitByRaycast = false; // Reset the hit flag
        if (vfx != null && vfx.enabled)
        {
            Debug.Log("Stopping VFX");
            vfx.Stop(); // Stop the VFX
        }
        else
        {
            Debug.Log("VFX already stopped or null");
        }
    }

    private IEnumerator DeactivateAfterTimeout()
    {
        yield return new WaitForSeconds(deactivateTimeout);

        // Deactivate if not being hit by a raycast during the timeout
        if (!isBeingHitByRaycast)
        {
            Deactivate();
        }
        else
        {
            // If another ray hit resets the timer, we shouldn't deactivate
            deactivateCoroutine = null;
        }

        deactivateCoroutine = null; // Clear coroutine reference
    }
}
