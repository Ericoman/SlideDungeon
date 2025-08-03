using System;
using System.Collections;
using UnityEngine;

public class HealthComponent : MonoBehaviour
{

    [SerializeField]
    private float maxHealth = 1;
    private float currentHealth;

    [SerializeField]
    bool doesRespawn = false;
    private Vector3 respawnPotition = Vector3.zero;

    [SerializeField]
    private float damageCooldown = 1;
    private bool _canTakeDamage = true;


    void Start()
    {
        currentHealth = maxHealth;
        respawnPotition = gameObject.transform.position;
    }

    public void ReceiveDamage(float damage)
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


    public void ReceiveHealth(float health) 
    {
        currentHealth = Math.Min(maxHealth, currentHealth + health);
    }

    public void SetRespawnPosition(Vector3 respawn) 
    {
        respawnPotition = respawn;
    }

}