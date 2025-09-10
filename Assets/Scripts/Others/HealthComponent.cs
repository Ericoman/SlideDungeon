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

    public event Action OnDeath;

    public event System.Action OnDamageTaken;
    void Start()
    {
        currentHealth = maxHealth;
        respawnPotition = gameObject.transform.position;
    }

    virtual public bool ReceiveDamage(int damage)
    {
        if (!_canTakeDamage)
            return false;

        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            OnDeath?.Invoke();
            if (!doesRespawn)
            {
                Destroy(gameObject);
            }
            else
            {
                DieAnimation();
                
            }
        }
        OnDamageTaken?.Invoke();

        // Start cooldown coroutine
        StartCoroutine(DamageCooldownCoroutine());
        return true;
    }

    virtual protected void DieAnimation()
    {
        StartCoroutine(DieCoroutine());
       
    }

    IEnumerator DieCoroutine()
    {
        yield return new WaitForSeconds(2f);
        RespawnDeath();
        

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

    virtual public void ReceiveDamageByFall(int damage)
    {
        currentHealth -= damage;

        if (!doesRespawn)
        {
            OnDeath?.Invoke();
            Destroy(gameObject);
        }
        else
        {
            if (currentHealth <= 0)
            {
                OnDeath?.Invoke();

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

    private void OnDestroy()
    {
        OnDeath = null;
    }
}