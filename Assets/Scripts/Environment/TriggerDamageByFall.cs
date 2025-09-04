using UnityEngine;

public class TriggerDamageByFall : MonoBehaviour
{
    [SerializeField]
    private int damageProduced = 1;

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        { 
            HealthComponent health = other.GetComponentInChildren<HealthComponent>();
            if (health)
            {
                health.ReceiveDamageByFall(damageProduced);
            }
        }
        else //if not player destroy
        {
            Destroy(other.gameObject, 0.5f);
        }   
    }
}
