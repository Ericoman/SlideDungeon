using System;
using System.Collections;
using UnityEngine;
[RequireComponent(typeof(LineRenderer))]
public class ThunderLink : MonoBehaviour
{
    public GameObject sphere1; // Reference to the first sphere
    public GameObject sphere2; // Reference to the second sphere

    private Coroutine sphere1MovementCoroutine;
    private Coroutine sphere2MovementCoroutine;

    public bool isSphere1MovementComplete { get; private set; } // Flag for movement completion status
    
    public LineRenderer lineRenderer;
    //private BoxCollider lineCollider; // The collider along the line
    
    void Start()
    {
        // Get the LineRenderer component attached to this GameObject
        lineRenderer = GetComponent<LineRenderer>();

        // Set the number of line points to 2 (one for each sphere)
        lineRenderer.positionCount = 2;

        // Optional: Customize the LineRenderer's appearance
        lineRenderer.startWidth = 0.1f; // Thickness of the line at the start
        lineRenderer.endWidth = 0.1f;   // Thickness of the line at the end
        lineRenderer.material = new Material(Shader.Find("Sprites/Default")); // Basic material
        lineRenderer.startColor = Color.yellow; // Line color at the start
        lineRenderer.endColor = Color.yellow;   // Line color at the end
        
        // Create a new BoxCollider and attach it to this GameObject
        //lineCollider = gameObject.AddComponent<BoxCollider>();
        //lineCollider.isTrigger = true; // Optional: Make it a trigger collider
    }

    public void StartSphere1Movement(Transform target, Vector3 targetPoint, float speed)
    {
        StopSphere1Movement(); // Ensure the old coroutine is stopped
        isSphere1MovementComplete = false; // Reset the movement completion flag
        sphere1MovementCoroutine = StartCoroutine(MoveSphereToPoint(target, targetPoint, speed, onComplete: () => { isSphere1MovementComplete = true; }));
    }

    public void StartSphere2Movement(Transform target, Vector3 targetPoint, float speed)
    {
        StopSphere2Movement(); // Ensure the old coroutine is stopped
        sphere2MovementCoroutine = StartCoroutine(MoveSphereToPoint(target, targetPoint, speed, onComplete: null));
    }

    public void StopSphere1Movement()
    {
        if (sphere1MovementCoroutine != null)
        {
            StopCoroutine(sphere1MovementCoroutine);
            sphere1MovementCoroutine = null;
        }
    }

    public void StopSphere2Movement()
    {
        if (sphere2MovementCoroutine != null)
        {
            StopCoroutine(sphere2MovementCoroutine);
            sphere2MovementCoroutine = null;
        }
    }

    public void OnIsBeingDestroyed()
    {
        // Stop all coroutines for sphere movement
        StopSphere1Movement();
        StopSphere2Movement();
    }

    private IEnumerator MoveSphereToPoint(Transform sphereTransform, Vector3 targetPoint, float moveSpeed,
        Action onComplete)
    {
        while (sphereTransform != null && Vector3.Distance(sphereTransform.position, targetPoint) > 0.1f)
        {
            sphereTransform.position = Vector3.MoveTowards(sphereTransform.position, targetPoint, moveSpeed * Time.deltaTime);
            yield return null;
        }

        if (sphereTransform != null)
        {
            // Snap to target point
            sphereTransform.position = targetPoint;
        }
        
        onComplete?.Invoke(); // Invoke the callback if any
    }

    void OnDestroy()
    {
        OnIsBeingDestroyed(); // Ensure cleanup when the object is destroyed
    }

    void Update()
    {
        if (sphere1 != null && sphere2 != null)
        {
            // Update the positions of the LineRenderer's endpoints
            Vector3 sphere1Position = sphere1.transform.position;
            Vector3 sphere2Position = sphere2.transform.position;

            lineRenderer.SetPosition(0, sphere1Position); // Set start position
            lineRenderer.SetPosition(1, sphere2Position); // Set end position
            
            // Perform a raycast between sphere1 and sphere2
            RaycastHit hit;
            Vector3 direction = sphere2Position - sphere1Position; // Direction from sphere1 to sphere2
            float distance = direction.magnitude; // Distance between the spheres

            // Check if the ray hits any object
            if (Physics.Raycast(sphere1Position, direction.normalized, out hit, distance))
            {
                // Check if the object hit has the "Enemy" tag
                if (hit.collider.CompareTag("Enemy"))
                {
                    // Destroy the enemy object
                    Destroy(hit.collider.gameObject);
                }
                
                // Check if we hit an iceberg
                if (hit.collider.CompareTag("Iceberg"))
                {
                        // Get the normal of the surface at the impact point
                    Vector3 hitNormal = hit.normal;

                    // Calculate the two perpendicular directions
                    Vector3 perpendicular1 = Vector3.Cross(hitNormal, Vector3.up).normalized;
                    if (perpendicular1 == Vector3.zero) // Handle parallel cases
                    {
                        perpendicular1 = Vector3.Cross(hitNormal, Vector3.forward).normalized;
                    }
                    Vector3 perpendicular2 = -perpendicular1; // Opposite direction

                    // Calculate the points for the perpendicular rays
                    Vector3 hitPoint = hit.point;
                    Vector3 endPoint1;
                    Vector3 endPoint2;

                    // Perpendicular ray 1 (cast in perpendicular1 direction)
                    if (Physics.Raycast(hitPoint, perpendicular1, out RaycastHit hit1, Mathf.Infinity))
                    {
                        endPoint1 = hit1.point; // Stop at the collision point
                        
                        TeslaCoil coil1 = hit1.collider.gameObject.GetComponent<TeslaCoil>();
                        if (coil1 != null)
                        {
                            coil1.Activate(); // Activate TeslaCoil 1
                        }
                        
                    }
                    else
                    {
                        endPoint1 = hitPoint + perpendicular1 * 10f; // Extend 10 units if no collision
                    }

                    // Perpendicular ray 2 (cast in perpendicular2 direction)
                    if (Physics.Raycast(hitPoint, perpendicular2, out RaycastHit hit2, Mathf.Infinity))
                    {
                        endPoint2 = hit2.point; // Stop at the collision point
                        
                        // Check if the object hit is a TeslaCoil
                        TeslaCoil coil2 = hit2.collider.gameObject.GetComponent<TeslaCoil>();
                        if (coil2 != null)
                        {
                            coil2.Activate(); // Activate TeslaCoil 2
                        }
                    }
                    else
                    {
                        endPoint2 = hitPoint + perpendicular2 * 10f; // Extend 10 units if no collision
                    }

                    // Update the LineRenderer to include perpendicular lines
                    lineRenderer.positionCount = 5;
                    lineRenderer.SetPosition(1, hitPoint);        // Point of impact
                    lineRenderer.SetPosition(2, endPoint1);      // First perpendicular ray
                    lineRenderer.SetPosition(3, hitPoint);       // Back to the hit point
                    lineRenderer.SetPosition(4, endPoint2);      // Second perpendicular ray

                    // Debugging (Optional - Visualize the rays in the Scene View)
                    Debug.DrawRay(hitPoint, perpendicular1 * 10f, Color.green, 0.1f);
                    Debug.DrawRay(hitPoint, perpendicular2 * 10f, Color.blue, 0.1f);
            
                }
                else
                {
                    // If the raycast no longer hits an iceberg, reset to only original ray
                    lineRenderer.positionCount = 2; // Reset to only showing the original ray
                    lineRenderer.SetPosition(0, sphere1Position);
                    lineRenderer.SetPosition(1, sphere2Position);
                }
                
            }
            // Update the BoxCollider
            //UpdateLineCollider(sphere1Position, sphere2Position);
        }
    }
    
    /*private void UpdateLineCollider(Vector3 startPoint, Vector3 endPoint)
    {
        // Position the collider at the midpoint of the line
        Vector3 midPoint = (startPoint + endPoint) / 2f;
        lineCollider.transform.position = midPoint;

        // Adjust collider size to match the line
        float lineLength = Vector3.Distance(startPoint, endPoint);
        lineCollider.size = new Vector3(lineLength, lineRenderer.startWidth, lineRenderer.startWidth);

        // Rotate the collider to align with the line
        Vector3 direction = (endPoint - startPoint).normalized;
        lineCollider.transform.rotation = Quaternion.LookRotation(direction);
    }*/
    
    /*private void OnTriggerEnter(Collider other)
    {
        Debug.Log($"Object {other.name} entered the line trigger!");

        if (other.gameObject.CompareTag("Enemy"))
        {
            Destroy(other.gameObject);
        }
    }*/
}
