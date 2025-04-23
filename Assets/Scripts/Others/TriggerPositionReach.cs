using UnityEngine;

public class TriggerPositionReach : MonoBehaviour
{
    [SerializeField] GameObject objectAppear;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) 
        {
            objectAppear.SetActive(true);
        }
    }
}
