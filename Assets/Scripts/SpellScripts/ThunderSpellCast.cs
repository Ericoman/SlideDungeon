using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThunderSpellCast : SpellBase
{
    public GameObject thunderLinkPrefab; // Assign the ThunderLink prefab in the inspector
    public Transform targetTransform;   // Assign the selectable Transform in the inspector
    
    public Transform castOrigin;        // Transform from which the raycast will originate
    public float raycastRange = 100f;   // Maximum range for the raycast (adjust as needed)
    public float sphereMoveSpeed = 10f; // Speed at which the sphere will move to the target
    
    private ThunderLink currentThunderLink; // Reference to the current ThunderLink instance
    private bool isFirstCast = true;        // Tracks whether it's the first or second cast
    private bool isCasting = false; // Lock to prevent multiple casts
    
    public float maxAttachmentDistance = 20f; // Maximum distance allowed for attachment
    private bool isSphere2Attached = false; // Tracks whether sphere2 is attached to a TeslaCoil or MirrorReflection

    private Queue<GameObject> thunderLinkQueue = new Queue<GameObject>(); // Store instantiated prefabs
    public int maxThunderLinks = 3; // Maximum number of ThunderLinks to store in the queue

    private Transform originalSphere2Parent;
    
    private TeslaCoil currentAttachedCoil; // Store reference to the currently attached coil, if any
    private MirrorReflection currentMirrorReflection; // Store reference to the currently attached mirror reflection, if any
    private Vector3 GetPlayerAdjustedForward()
    {
        // Correct the player's forward direction by rotating the local forward (Z-axis) to match logical forward
        return transform.rotation * Vector3.forward;
    }
    private Vector3 GetClosestAllowedDirection(Vector3 playerFacingDirection)
    {
        // Lock to X-Z plane by zeroing out the Y-axis
        playerFacingDirection.y = 0;

        // Check for zero input
        if (playerFacingDirection.sqrMagnitude == 0)
        {
            Debug.LogWarning("Player facing direction magnitude is zero! Defaulting to World Forward.");
            return Vector3.forward; // Default to forward if input is invalid
        }

        // Normalize the player's facing direction
        playerFacingDirection = playerFacingDirection.normalized;

        // Define allowed directions in the X-Z plane
        Vector3[] allowedDirections = new Vector3[]
        {
            Vector3.forward,                             // Forward (0, 0, 1)
            (Vector3.forward + Vector3.right).normalized, // Forward-Right
            Vector3.right,                              // Right (1, 0, 0)
            (Vector3.back + Vector3.right).normalized,  // Back-Right
            Vector3.back,                               // Back (0, 0, -1)
            (Vector3.back + Vector3.left).normalized,   // Back-Left
            Vector3.left,                               // Left (-1, 0, 0)
            (Vector3.forward + Vector3.left).normalized // Forward-Left
        };

        // Find the closest allowed direction based on a dot product comparison
        float maxDot = -1f;
        Vector3 bestDirection = Vector3.zero;

        foreach (Vector3 direction in allowedDirections)
        {
            float dot = Vector3.Dot(playerFacingDirection, direction);
            if (dot > maxDot)
            {
                maxDot = dot;
                bestDirection = direction;
            }
        }

        return bestDirection;
    }
    
    
    public void Awake()
    {
        GameObject rayCastLaunchPoint = GameObject.Find("RayCastLaunchPoint");
        GameObject targetTransformObject = GameObject.Find("Crystal");

        if (targetTransformObject != null)
        {
            targetTransform = targetTransformObject.transform;
        }
        else
        {
            Debug.LogError("ThunderSpellCast: 'Crystal' GameObject not found! Assign it manually or ensure the GameObject exists.");
        }
        
        if (rayCastLaunchPoint != null)
        {
            castOrigin = rayCastLaunchPoint.transform;
        }
        else
        {
            Debug.LogError("ThunderSpellCast: 'RayCastLaunchPoint' GameObject not found! Assign it manually or ensure the GameObject exists.");
        }
    }

    public override void CastSpell() // Directly callable by SpellHudManager
    {
        Debug.Log("ThunderSpellCast: OnCast triggered."); // Debug

        if (castOrigin == null)
        {
            Debug.LogError("ThunderSpellCast: CastOrigin is not assigned.");
            return;
        }
        
        // Prevent casting if already in progress
        if (isCasting)
        {
            Debug.LogWarning("ThunderSpellCast: Cast is already in progress!");
            return;
        }

        if (isFirstCast)
        {
            FirstCast(); // Perform the logic for the first cast
        }
        else
        {
            SecondCast(); // Perform the logic for the second cast
        }
        if (GetComponent<AudioComponent>())
        {
            GetComponent<AudioComponent>().Play();
        }
       
    }
    
    private void FirstCast()
    {
        if (thunderLinkPrefab == null)
        {
            Debug.LogError("ThunderSpellCast: ThunderLinkPrefab is not assigned.");
            return;
        }

        if (isCasting)
        {
            Debug.LogWarning("ThunderSpellCast: FirstCast already in progress.");
            return;
        }
        
        // Unsubscribe from the old ThunderLink (if any)
        if (currentThunderLink != null)
        {
            currentThunderLink.OnDestructionRequested -= HandleDestructionRequested;
        }

        isCasting = true; // Start the casting lock

        // Get player's logical forward direction
        Vector3 playerFacingDirection = GetPlayerAdjustedForward();
        Debug.DrawRay(transform.position, playerFacingDirection * 2f, Color.blue, 1f); // Debug player's adjusted forward

        // Lock direction to the nearest 8 allowed directions
        Vector3 desiredDirection = GetClosestAllowedDirection(playerFacingDirection);
        Debug.DrawRay(transform.position, desiredDirection * raycastRange, Color.green, 1f); // Debug direction snapped to 8 degrees

        // Instantiate the ThunderLink prefab
        GameObject thunderLinkInstance = Instantiate(thunderLinkPrefab, transform.position, Quaternion.identity);

        ThunderLink thunderLink = thunderLinkInstance.GetComponent<ThunderLink>();
        if (thunderLink != null)
        {
            thunderLink.Initialize(this); // Pass ThunderSpellCast instance
        }
        // Get the ThunderLink script on the prefab
        currentThunderLink = thunderLinkInstance.GetComponent<ThunderLink>();
        if (currentThunderLink == null)
        {
            Debug.LogError("ThunderSpellCast: ThunderLink script is missing or not configured properly!");
            isCasting = false; // Release the lock
            return;
        }

        
        
        // Subscribe to the destruction event
        currentThunderLink.OnDestructionRequested += HandleDestructionRequested;

        // Move sphere1 (first sphere) to the origin
        if (currentThunderLink.sphere1 != null)
        {
            currentThunderLink.sphere1.transform.position = castOrigin.position;

            // Perform raycast
            Ray ray = new Ray(castOrigin.position, desiredDirection); // Raycast from the cast origin
            if (Physics.Raycast(ray, out RaycastHit hitInfo, raycastRange))
            {
                float distanceToHit = Vector3.Distance(castOrigin.position, hitInfo.point); // Calculate distance
                Debug.Log($"ThunderSpellCast: Raycast hit {hitInfo.collider.name} at distance {distanceToHit}.");

                // Check if the distance is within the allowable range
                if (distanceToHit > maxAttachmentDistance)
                {
                    Debug.LogWarning($"ThunderSpellCast: Hit point is too far (distance: {distanceToHit}, max allowed: {maxAttachmentDistance}). Destroying the spell.");

                    // Move sphere1 to the point and destroy afterward
                    currentThunderLink.StartSphere1Movement(currentThunderLink.sphere1.transform, hitInfo.point, sphereMoveSpeed);
                    //currentThunderLink.StartCoroutine(DestroyWhenReached(currentThunderLink.sphere1.transform, hitInfo.point, currentThunderLink.gameObject, currentThunderLink.sphere2 != null ? currentThunderLink.sphere2.gameObject : null));
                }
                else
                {
                    // Check if the hit object has the "TeslaCoil" tag
                    if (hitInfo.collider.CompareTag("TeslaCoil"))
                    {
                        // Attach to the object
                        currentThunderLink.StartSphere1Movement(currentThunderLink.sphere1.transform, hitInfo.point, sphereMoveSpeed);
                        isFirstCast = false; // Mark first cast as complete

                        // Monitor the distance in real-time
                        StartCoroutine(MonitorDistance(currentThunderLink.sphere1.transform));

                        // Release the cast lock after sphere1 movement is complete
                        currentThunderLink.StartCoroutine(ReleaseLockWhenSphere1MovementComplete());
                    }
                    else
                    {
                        // If the hit object does not have the "TeslaCoil" tag
                        Debug.Log($"ThunderSpellCast: Hit object '{hitInfo.collider.name}' has no 'TeslaCoil' tag.");

                        // Move sphere1 to the hit point and destroy it afterward
                        currentThunderLink.StartSphere1Movement(currentThunderLink.sphere1.transform, hitInfo.point, sphereMoveSpeed);
                        currentThunderLink.StartCoroutine(DestroyWhenReached(currentThunderLink.sphere1.transform, hitInfo.point, currentThunderLink.gameObject, currentThunderLink.sphere2 != null ? currentThunderLink.sphere2.gameObject : null));
                    }
                }
            }
            else
            {
                // Raycast did not hit anything
                Vector3 endPoint = castOrigin.position + (desiredDirection * raycastRange);
                float distanceToEndPoint = Vector3.Distance(castOrigin.position, endPoint); // Calculate distance
                Debug.Log($"ThunderSpellCast: Raycast didn't hit anything. Moving to endpoint at distance {distanceToEndPoint}.");

                // If no hit, calculate the endpoint of the raycast and move sphere1 there
                currentThunderLink.StartSphere1Movement(currentThunderLink.sphere1.transform, endPoint, sphereMoveSpeed);

                // Schedule destruction of ThunderLink after reaching the endpoint
                currentThunderLink.StartCoroutine(DestroyWhenReached(currentThunderLink.sphere1.transform, endPoint, currentThunderLink.gameObject, currentThunderLink.sphere2 != null ? currentThunderLink.sphere2.gameObject : null));
            }
        }
    
        // Parent sphere2 (second sphere) to the specified Transform or default behavior
        if (currentThunderLink.sphere2 != null && targetTransform != null)
        {
            currentThunderLink.sphere2.transform.position = targetTransform.position;


            originalSphere2Parent = currentThunderLink.sphere2.transform.parent;


            currentThunderLink.sphere2.transform.SetParent(targetTransform, true); // Make it a child
        }

    }

    private void SecondCast()
    {
        if (currentThunderLink != null)
        {
            // Unsubscribe from the destruction event
            currentThunderLink.OnDestructionRequested -= HandleDestructionRequested;

            // Destroy the ThunderLink
            //Destroy(currentThunderLink.gameObject);
            //currentThunderLink = null;
        }
        
        if (currentThunderLink == null || currentThunderLink.sphere2 == null)
        {
            if (currentThunderLink == null)
            {
                Debug.LogError("ThunderSpellCast: Second cast failed. 'currentThunderLink' is null!");
            }
            else if (currentThunderLink.sphere2 == null)
            {
                Debug.LogError("ThunderSpellCast: Second cast failed. 'currentThunderLink.sphere2' is null!");
            }

            return;
        }
        
        // Ensure the first sphere's movement has been completed
        if (!currentThunderLink.isSphere1MovementComplete)
        {
            Debug.LogWarning("ThunderSpellCast: Cannot perform the second cast until the first sphere's movement is completed.");
            return; // Exit the method if the coroutine is not finished
        }

        Debug.Log("ThunderSpellCast: Second cast triggered.");

        // Release sphere2 from the parent Transform
        currentThunderLink.sphere2.transform.SetParent(originalSphere2Parent, true); // Restore the original parent;

        // -- START BACKUP: Raycast and move sphere2 logic commented out --
        /*
        // Perform raycast with restricted direction
        Vector3 desiredDirection = GetClosestAllowedDirection(castOrigin.forward); // 
        
        // Perform raycast for the second sphere
        Ray ray = new Ray(castOrigin.position, desiredDirection); // Raycast from the cast origin
        if (Physics.Raycast(ray, out RaycastHit hitInfo, raycastRange))
        {
            Debug.Log($"ThunderSpellCast: Raycast hit {hitInfo.collider.name} (Second Cast).");

            // Move sphere2 to the raycast hit point
            currentThunderLink.StartSphere2Movement(currentThunderLink.sphere2.transform, hitInfo.point, sphereMoveSpeed);
        }
        else
        {
            Debug.Log("ThunderSpellCast: Second cast raycast did not hit anything.");
        }
        */
        // -- END BACKUP --
        
        
        // If `sphere2` exists, destroy it explicitly
        if (currentThunderLink.sphere2 != null)
        {
            if (currentAttachedCoil != null)
            {
                Debug.Log($"ThunderSpellCast: Detaching sphere2 from Tesla Coil '{currentAttachedCoil.name}' before destroying it.");
                currentAttachedCoil.SetActiveWithSphere(null); // Clear TeslaCoil's reference
                currentAttachedCoil = null; // Clear the ThunderSpellCast reference
            }

            if (currentMirrorReflection != null)
            {
                Debug.Log($"ThunderSpellCast: Detaching sphere2 from Mirror '{currentMirrorReflection.name}' before destroying it.");
                currentMirrorReflection.SetActiveWithSphere(null); // Clear Mirror's reference
                currentMirrorReflection = null; // Clear the ThunderSpellCast reference
            }
            Destroy(currentThunderLink.sphere2.gameObject);
            Debug.Log("ThunderSpellCast: Destroyed sphere2.");
        }
        else
        {
            Debug.LogWarning("ThunderSpellCast: sphere2 was already null, skipping destruction.");
        }
        // DESTROY ThunderLink object
        Destroy(currentThunderLink.gameObject);
        Debug.Log("ThunderSpellCast: Destroyed current ThunderLink.");
        
        // Reset to handle new casts later
        isFirstCast = true; // Reset to allow the first cast logic to run again
        currentThunderLink = null; // Clear the reference to the current ThunderLink
        isCasting = false; // Release the lock
        // Reset state when destroying sphere2 or the ThunderLink
        isSphere2Attached = false;
    }
    
    /*private void ManagePrefabQueue(GameObject thunderLinkInstance)
    {
        thunderLinkQueue.Enqueue(thunderLinkInstance);

        if (thunderLinkQueue.Count > maxThunderLinks)
        {
            GameObject oldestLinkObject = thunderLinkQueue.Dequeue();

            // Ensure the ThunderLink cleans up its coroutines and resources before destruction
            ThunderLink oldestLink = oldestLinkObject.GetComponent<ThunderLink>();
            if (oldestLink != null)
            {
                oldestLink.OnIsBeingDestroyed(); // Stop coroutines safely
            }

            Destroy(oldestLinkObject); // Destroy the object
        }
    }*/
    
    /*private System.Collections.IEnumerator MoveSphereToPoint(Transform sphereTransform, Vector3 targetPoint)
    {
        while (Vector3.Distance(sphereTransform.position, targetPoint) > 0.1f)
        {
            // Move the sphere towards the target point
            sphereTransform.position = Vector3.MoveTowards(sphereTransform.position, targetPoint, sphereMoveSpeed * Time.deltaTime);
            yield return null;
        }

        // Ensure the sphere snaps exactly to the target position at the end of the movement
        sphereTransform.position = targetPoint;
    }*/
    
    private IEnumerator DestroyWhenReached(Transform sphereTransform, Vector3 targetPoint, GameObject objectToDestroy, GameObject sphere2 = null)
    {
        while (sphereTransform != null && Vector3.Distance(sphereTransform.position, targetPoint) > 0.1f)
        {
            yield return null; // Wait until the sphere reaches the target
        }
        
        // Reset any attachment flags
        isSphere2Attached = false;

        // Destroy the object after movement is complete
        Destroy(objectToDestroy);
        Debug.Log("ThunderSpellCast: ThunderLink destroyed after reaching the endpoint.");
        
        // Destroy sphere2 if provided
        if (sphere2 != null)
        {
            Destroy(sphere2);
            Debug.Log("ThunderSpellCast: sphere2 also destroyed after reaching the endpoint.");
        }
        
        isFirstCast = true; // Reset to allow the first cast logic to run again
        isCasting = false; // Release the lock
    }
    
    private IEnumerator ReleaseLockWhenSphere1MovementComplete()
    {
        while (!currentThunderLink.isSphere1MovementComplete)
        {
            yield return null; // Wait until the movement is complete
        }

        // Release the lock after completion
        isCasting = false;
        Debug.Log("ThunderSpellCast: First cast complete, lock released.");
    }
    
    private IEnumerator MonitorDistance(Transform attachedSphere)
    {
        while (attachedSphere != null)
        {
            if (isSphere2Attached)
            {
                yield return new WaitForSeconds(0.1f); // Check distance periodically (every 0.1 second)
                continue; // Skip the rest of the logic if sphere2 is attached
            }
            
            float currentDistance = Vector3.Distance(castOrigin.position, attachedSphere.position);

            // Check if the sphere has exceeded the maxAttachmentDistance
            if (currentDistance > maxAttachmentDistance)
            {
                Debug.LogWarning($"ThunderSpellCast: Sphere moved out of range (distance: {currentDistance}, max allowed: {maxAttachmentDistance}). Destroying the link.");

                // Trigger destruction when out of range
                currentThunderLink.StartCoroutine(DestroyWhenReached(attachedSphere, attachedSphere.position, currentThunderLink.gameObject, currentThunderLink.sphere2 != null ? currentThunderLink.sphere2.gameObject : null));
                yield break; // Stop monitoring after destruction
            }

            yield return new WaitForSeconds(0.1f); // Check distance periodically (every 0.1 second)
        }
    }
    
    private void HandleDestructionRequested(GameObject sphere1, GameObject sphere2, Vector3 position)
    {
        StartCoroutine(DestroyWhenReached(sphere1.transform, position, currentThunderLink.gameObject, currentThunderLink.sphere2 != null ? currentThunderLink.sphere2.gameObject : null));
    }
    
    private void OnDestroy()
    {
        if (currentThunderLink != null)
        {
            currentThunderLink.OnDestructionRequested -= HandleDestructionRequested;
        }
    }
    
    public void OnSphere2TriggerEnter(Collider other)
    {
        if (other.CompareTag("TeslaCoil"))
        {
            // Attach sphere2 to the Tesla Coil if it’s not attached yet
            TeslaCoil teslaCoil = other.GetComponent<TeslaCoil>();

            if (teslaCoil != null && currentThunderLink != null && currentThunderLink.sphere2 != null && teslaCoil.IsActive == false)
            {
                AttachSphere2ToTeslaCoil(teslaCoil, currentThunderLink.sphere2);
            }
        }

        if (other.CompareTag("Mirror"))
        {
            MirrorReflection mirror = other.GetComponent<MirrorReflection>();

            if (mirror != null && currentThunderLink != null && currentThunderLink.sphere2 != null &&
                mirror.IsActive == false)
            {
                AttachSphere2ToMirror(mirror, currentThunderLink.sphere2);
            }
        }
    }

    private void AttachSphere2ToTeslaCoil(TeslaCoil coil, GameObject sphere2)
    {
        if (currentAttachedCoil == coil)
        {
            Debug.LogWarning("ThunderSpellCast: sphere2 is already attached to the Tesla Coil.");
            return;
        }

        // Detach previous Tesla Coil, if any
        if (currentAttachedCoil != null)
        {
            Debug.Log($"ThunderSpellCast: Detaching from Coil {currentAttachedCoil.name}");
        }

        // Update the reference and parent sphere2 to the Tesla Coil
        currentAttachedCoil = coil;
        sphere2.transform.SetParent(coil.transform, true);
        sphere2.transform.position = coil.transform.position + new Vector3(0, 0.5f, 0);
        Debug.Log($"ThunderSpellCast: sphere2 is now attached to Tesla Coil '{coil.name}'.");
        
        // Activate the Tesla Coil
        coil.SetActiveWithSphere(sphere2);
        // Mark sphere2 as attached
        isSphere2Attached = true;
    }

    private void AttachSphere2ToMirror(MirrorReflection mirror, GameObject sphere2)
    {
        if (currentMirrorReflection == mirror)
        {
            Debug.LogWarning("ThunderSpellCast: sphere2 is already attached to the Mirror.");
            return;
        }
        // Detach previous Mirror, if any
        if (currentMirrorReflection != null)
        {
            Debug.Log($"ThunderSpellCast: Detaching from Mirror {currentAttachedCoil.name}");
        }
        
        currentMirrorReflection = mirror;
        sphere2.transform.SetParent(mirror.transform, true);
        sphere2.transform.position = mirror.transform.position + new Vector3(0, 0.5f, 0);
        Debug.Log($"ThunderSpellCast: sphere2 is now attached to Mirror '{mirror.name}'.");
        // Activate the Mirror
        mirror.SetActiveWithSphere(sphere2);
        // Mark sphere2 as attached
        isSphere2Attached = true;
    }
}
