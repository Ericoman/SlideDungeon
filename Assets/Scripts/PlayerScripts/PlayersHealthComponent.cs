using UnityEngine;
using TMPro;
using System.Collections;

public class PlayersHealthComponent : HealthComponent
{
    public TextMeshProUGUI heartsText;

    public float updateFallPositionCooldown = 2;
    private float _checkDistance = 1; // Distance below object to check for ground
    private Vector3 _fallPosition;

    void Start()
    {
        currentHealth = maxHealth;
        respawnPotition = gameObject.transform.position;
        _fallPosition = gameObject.transform.position;
        SetText();

        StartCoroutine(FallPositionCoroutine());

    }

    private IEnumerator FallPositionCoroutine()
    {
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

    override protected void RespawnDeath()
    {
        base.RespawnDeath();//call parent 
        SetText();
    }
    override public void ReceiveHealth(int health)
    {
        base.ReceiveHealth(health);//call parent 
        SetText();
    }

    override public void ReceiveDamage(int damage)
    {
        base.ReceiveDamage(damage);//call parent 
        SetText();
    }

    override protected void RespawnFall()
    {
        gameObject.transform.position = _fallPosition;
        SetText();
    }

    void SetText() 
    {
        if(heartsText != null ) heartsText.text = $"{currentHealth}";
    }

   
}