using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed = 10f;
    public float lifetime = 5f;
    public int damage = 1;

    void Start()
    {
        Destroy(gameObject, lifetime); // destroy after x seconds
    }

    void Update()
    {
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            HealthComponent health = other.gameObject.GetComponentInChildren<HealthComponent>();
            if (health)
            {
                health.ReceiveDamage(damage);
            }

        }
        Destroy(gameObject);
    }
}
