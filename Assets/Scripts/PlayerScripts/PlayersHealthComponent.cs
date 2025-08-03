using UnityEngine;
using TMPro;

public class PlayersHealthComponent : HealthComponent
{
    public TextMeshProUGUI heartsText;

    void Start()
    {
        currentHealth = maxHealth;
        respawnPotition = gameObject.transform.position;
        SetText();
    }

    override public void ReceiveDamage(float damage)
    {
        base.ReceiveDamage(damage);//call parent 
        SetText();
    }

    override public void ReceiveHealth(float health)
    {
        base.ReceiveHealth(health);//call parent 
        SetText();
    }

    void SetText() 
    {
        heartsText.text = $"{currentHealth}";
    }
}
