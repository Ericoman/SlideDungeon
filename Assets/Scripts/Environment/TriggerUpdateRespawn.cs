using UnityEngine;

public class TriggerUpdateRespawn : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            HealthComponent health = other.GetComponentInChildren<HealthComponent>();
            if (health)
            {
                health.SetRespawnPosition(other.transform.position);
            }
        } 
    }
}
