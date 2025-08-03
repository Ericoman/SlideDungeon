using System;
using System.Collections;
using UnityEngine;

public class HealthComponent : MonoBehaviour
{

    [SerializeField]
    protected float maxHealth = 1;
    protected float currentHealth;

    [SerializeField]
    bool doesRespawn = false;
    protected Vector3 respawnPotition = Vector3.zero;

    [SerializeField]
    protected float damageCooldown = 1;
    protected bool _canTakeDamage = true;


    void Start()
    {
        currentHealth = maxHealth;
        respawnPotition = gameObject.transform.position;
    }

    virtual public void ReceiveDamage(float damage)
    {
        if (!_canTakeDamage)
            return;

        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            if (!doesRespawn)
            {
                Destroy(gameObject);
            }
            else
            {
                gameObject.transform.position = respawnPotition;
                currentHealth = maxHealth;
            }
        }

        // Start cooldown coroutine
        StartCoroutine(DamageCooldownCoroutine());
    }

    private IEnumerator DamageCooldownCoroutine()
    {
        _canTakeDamage = false;
        yield return new WaitForSeconds(damageCooldown);
        _canTakeDamage = true;
    }


    virtual public void ReceiveHealth(float health) 
    {
        currentHealth = Math.Min(maxHealth, currentHealth + health);
    }

    public void SetRespawnPosition(Vector3 respawn) 
    {
        respawnPotition = respawn;
    }

}