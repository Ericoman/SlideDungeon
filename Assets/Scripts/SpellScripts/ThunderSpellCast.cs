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

    private Queue<GameObject> thunderLinkQueue = new Queue<GameObject>(); // Store instantiated prefabs
    public int maxThunderLinks = 3; // Maximum number of ThunderLinks to store in the queue

    private Transform originalSphere2Parent;
    
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

        isCasting = true; // Start the casting lock

        // Get player's logical forward direction
        Vector3 playerFacingDirection = GetPlayerAdjustedForward();
        Debug.DrawRay(transform.position, playerFacingDirection * 2f, Color.blue, 1f); // Debug player's adjusted forward

        // Lock direction to the nearest 8 allowed directions
        Vector3 desiredDirection = GetClosestAllowedDirection(playerFacingDirection);
        Debug.DrawRay(transform.position, desiredDirection * raycastRange, Color.green, 1f); // Debug direction snapped to 8 degrees

        // Instantiate the ThunderLink prefab
        GameObject thunderLinkInstance = Instantiate(thunderLinkPrefab, transform.position, Quaternion.identity);

        // Get the ThunderLink script on the prefab
        currentThunderLink = thunderLinkInstance.GetComponent<ThunderLink>();
        if (currentThunderLink == null)
        {
            Debug.LogError("ThunderSpellCast: ThunderLink script is missing or not configured properly!");
            isCasting = false; // Release the lock
            return;
        }

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
                    currentThunderLink.StartCoroutine(DestroyWhenReached(currentThunderLink.sphere1.transform, hitInfo.point, currentThunderLink.gameObject));
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
                        currentThunderLink.StartCoroutine(DestroyWhenReached(currentThunderLink.sphere1.transform, hitInfo.point, currentThunderLink.gameObject));
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
                currentThunderLink.StartCoroutine(DestroyWhenReached(currentThunderLink.sphere1.transform, endPoint, currentThunderLink.gameObject));
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
        if (currentThunderLink == null || currentThunderLink.sphere2 == null)
        {
            Debug.LogError("ThunderSpellCast: Second cast failed. ThunderLink or sphere2 is not properly initialized!");
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
        
        // DESTROY ThunderLink object
        Destroy(currentThunderLink.gameObject);
        Debug.Log("ThunderSpellCast: Destroyed current ThunderLink.");
        
        // Reset to handle new casts later
        isFirstCast = true; // Reset to allow the first cast logic to run again
        currentThunderLink = null; // Clear the reference to the current ThunderLink
        isCasting = false; // Release the lock
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
    
    private IEnumerator DestroyWhenReached(Transform sphereTransform, Vector3 targetPoint, GameObject objectToDestroy)
    {
        while (sphereTransform != null && Vector3.Distance(sphereTransform.position, targetPoint) > 0.1f)
        {
            yield return null; // Wait until the sphere reaches the target
        }

        // Destroy the object after movement is complete
        Destroy(objectToDestroy);
        Debug.Log("ThunderSpellCast: ThunderLink destroyed after reaching the endpoint.");
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
            float currentDistance = Vector3.Distance(castOrigin.position, attachedSphere.position);

            // Check if the sphere has exceeded the maxAttachmentDistance
            if (currentDistance > maxAttachmentDistance)
            {
                Debug.LogWarning($"ThunderSpellCast: Sphere moved out of range (distance: {currentDistance}, max allowed: {maxAttachmentDistance}). Destroying the link.");

                // Trigger destruction when out of range
                currentThunderLink.StartCoroutine(DestroyWhenReached(attachedSphere, attachedSphere.position, currentThunderLink.gameObject));
                yield break; // Stop monitoring after destruction
            }

            yield return new WaitForSeconds(0.1f); // Check distance periodically (every 0.1 second)
        }
    }
}
