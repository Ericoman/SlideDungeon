using System;
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

    private Queue<GameObject> thunderLinkQueue = new Queue<GameObject>(); // Store instantiated prefabs
    public int maxThunderLinks = 3; // Maximum number of ThunderLinks to store in the queue

    private Transform originalSphere2Parent;
    
    
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

        // Perform raycast
        Ray ray = new Ray(castOrigin.position, castOrigin.forward); // Raycast from the cast origin
        if (Physics.Raycast(ray, out RaycastHit hitInfo, raycastRange))
        {
            Debug.Log($"ThunderSpellCast: Raycast hit {hitInfo.collider.name}.");

            // Instantiate the ThunderLink prefab
            GameObject thunderLinkInstance = Instantiate(thunderLinkPrefab, transform.position, Quaternion.identity);
            
            ManagePrefabQueue(thunderLinkInstance); // Manage the prefab queue;
            
            // Get the ThunderLink script on the prefab
            currentThunderLink = thunderLinkInstance.GetComponent<ThunderLink>();
            if (currentThunderLink != null)
            {
                // Move sphere1 (first sphere) to the origin and start moving it towards the hit point
                if (currentThunderLink.sphere1 != null)
                {
                    currentThunderLink.sphere1.transform.position = castOrigin.position;
                    currentThunderLink.StartSphere1Movement(currentThunderLink.sphere1.transform, hitInfo.point, sphereMoveSpeed);
                }

                // Parent sphere2 (second sphere) to the specified Transform
                if (currentThunderLink.sphere2 != null && targetTransform != null)
                {
                    currentThunderLink.sphere2.transform.position = targetTransform.position;
                    
                    originalSphere2Parent = currentThunderLink.sphere2.transform.parent;
                    
                    currentThunderLink.sphere2.transform.SetParent(targetTransform, true); // Make it a child
                }

                // First cast is complete
                isFirstCast = false;
            }
            else
            {
                Debug.LogError("ThunderSpellCast: ThunderLink script is missing or not configured properly!");
            }
        }
        else
        {
            Debug.Log("ThunderSpellCast: Raycast did not hit anything.");
        }
    }

    private void SecondCast()
    {
        if (currentThunderLink == null || currentThunderLink.sphere2 == null)
        {
            Debug.LogError("ThunderSpellCast: Second cast failed. ThunderLink or sphere2 is not properly initialized!");
            return;
        }

        Debug.Log("ThunderSpellCast: Second cast triggered.");

        // Release sphere2 from the parent Transform
        currentThunderLink.sphere2.transform.SetParent(originalSphere2Parent, true); // Restore the original parent;

        // Perform raycast for the second sphere
        Ray ray = new Ray(castOrigin.position, castOrigin.forward); // Raycast from the cast origin
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
        
        
        // Reset to handle new casts later
        isFirstCast = true; // Reset to allow the first cast logic to run again
        currentThunderLink = null; // Clear the reference to the current ThunderLink
    }
    
    private void ManagePrefabQueue(GameObject thunderLinkInstance)
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
    }
    
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
    
}
