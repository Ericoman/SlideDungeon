using System;
using System.Collections;
using UnityEngine;

public class HealthComponent : MonoBehaviour
{

    [SerializeField]
    protected int maxHealth = 1;
    protected int currentHealth;

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

    virtual public void ReceiveDamage(int damage)
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
                RespawnDeath();
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


    virtual public void ReceiveHealth(int health) 
    {
        currentHealth = Math.Min(maxHealth, currentHealth + health);
    }

    public void ReceiveDamageByFall(int damage)
    {
        currentHealth -= damage;

        if (!doesRespawn)
        {
            Destroy(gameObject);
        }
        else
        {
            if (currentHealth <= 0)
            {
                RespawnDeath();
            }
            else
            {
                RespawnFall();
            }
        }
    }

    public void SetRespawnPosition(Vector3 respawn) 
    {
        respawnPotition = respawn;
    }

    virtual protected void RespawnDeath() 
    {
        gameObject.transform.position = respawnPotition;
        currentHealth = maxHealth;
    }

    virtual protected void RespawnFall()
    {
        Destroy(gameObject);
    }

}