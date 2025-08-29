using UnityEngine;
using TMPro;
using System.Collections;
using System;

public class PlayersHealthComponent : HealthComponent
{
    //public TextMeshProUGUI heartsText;
    //public TextMeshProUGUI extraHeartsText;

    public HeartsManager heartsManagerUI;

    [SerializeField]
    protected int maxExtraHealth = 2;
    protected int currentExtraHealth = 0;

    public float updateFallPositionCooldown = 2;
    private float _checkDistance = 1; // Distance below object to check for ground
    private Vector3 _fallPosition;

    void Start()
    {
        currentHealth = maxHealth;
        if(heartsManagerUI) heartsManagerUI.SetFullHealth();

        StartCoroutine(FallPositionCoroutine());
    }

    private IEnumerator FallPositionCoroutine()
    {
        yield return new WaitForSeconds(0.25f);

        respawnPotition = gameObject.transform.position;
        _fallPosition = gameObject.transform.position;

        while (true)
        {
            if (IsGrounded())
            { 
                _fallPosition = gameObject.transform.position;
            }
            yield return new WaitForSeconds(updateFallPositionCooldown);
        }

    }
    bool IsGrounded()
    {
        return Physics.Raycast(transform.position, Vector3.down, _checkDistance);
    }

    //healthComponent 
    override public void ReceiveHealth(int health)
    {
        base.ReceiveHealth(health);
        SetText();
    }
    override public bool ReceiveDamage(int damage)
    {
        if (currentExtraHealth == 0) 
        { 
            base.ReceiveDamage(damage);
            SetText();
        }
        else 
        {
            if (!_canTakeDamage)
                return false;

            if (damage - currentExtraHealth > 0) 
            { 
                base.ReceiveDamage(damage - currentExtraHealth);
                SetText();
            }
            ReceiveExtraDamage(damage);

        }
       return true;
    }
    override public void ReceiveDamageByFall(int damage) 
    {
        if (currentExtraHealth == 0)
        {
            base.ReceiveDamageByFall(damage);
        }
        else
        {

            if (damage - currentExtraHealth > 0)
            {
                base.ReceiveDamage(damage - currentExtraHealth);
            }
            ReceiveExtraDamage(damage);
            RespawnFall();
        }
    }

    //healthComponent respawn
    override protected void RespawnDeath()
    {
        base.RespawnDeath();
        SetText();
    }
    override protected void RespawnFall()
    {
        gameObject.transform.position = _fallPosition;
        SetText();
    }

    //extra healht
    public bool ReceiveExtraHealth(int health)
    {
        if (currentExtraHealth >= maxExtraHealth) 
        { 
            return false;
        } 
        currentExtraHealth = Math.Min(maxExtraHealth, currentExtraHealth + health);
        SetTextExtra();
        return true;
    }
    public void ReceiveExtraDamage(int damage)
    {
        currentExtraHealth -= damage;
        if (currentExtraHealth < 0) currentExtraHealth = 0;
        SetTextExtra();
    }

    void SetText() 
    {
        if (heartsManagerUI) 
            heartsManagerUI.SetHearts(currentHealth);
    }
    void SetTextExtra() 
    {
        if (heartsManagerUI) 
            heartsManagerUI.SetExtraHeart(currentExtraHealth);
    }

}