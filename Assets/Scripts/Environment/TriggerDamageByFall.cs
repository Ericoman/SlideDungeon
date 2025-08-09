using UnityEngine;

public class TriggerDamageByFall : MonoBehaviour
{
    [SerializeField]
    private int damageProduced = 1;

    void OnTriggerEnter(Collider other)
    {
        HealthComponent health = other.GetComponentInChildren<HealthComponent>();
        if (health)
        {
            health.ReceiveDamageByFall(damageProduced);
        }
    }
}
