using UnityEngine;
using TMPro;
using System.Collections;
using System;
using UnityEngine.VFX;
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
    public PlayerMovement playermove;
    public Animator anim;

    public AudioClip damageSound;
    public AudioClip dieSound;
    public AudioClip healSound;

    public event System.Action OnDamageTaken;
    [Header("VFX")]
    public VisualEffect healVFX;
    public VisualEffect extraHeartVFX;
    public VisualEffect respawnVFX;

    [SerializeField] private float respawnVFXCooldown = 1.0f; // tiempo mínimo entre efectos
    private float lastRespawnVFXTime = -999f;

    private bool respawnVFXPlayed = false;

    void Start()
    {
        currentHealth = maxHealth;
        if (heartsManagerUI) heartsManagerUI.SetFullHealth();

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
        if (GetComponent<AudioComponent>() && healSound != null)
        {
            GetComponent<AudioComponent>().PlaySound(healSound);
        }
        if (healVFX != null)
        {
            VisualEffect vfx = Instantiate(healVFX, transform.position, Quaternion.identity);
            vfx.Play();
            Destroy(vfx.gameObject, 2f); // destruye la instancia después de 2s
        }
        SetText();


    }
    override public bool ReceiveDamage(int damage)
    {
        if (_canTakeDamage && (currentExtraHealth + currentHealth > damage))
        {
            anim.SetTrigger("Damage");
            if (GetComponent<AudioComponent>() && damageSound != null)
            {
                GetComponent<AudioComponent>().PlaySound(damageSound);
            }
        }


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
                anim.SetTrigger("Damage");
                SetText();
            }


            ReceiveExtraDamage(damage);


        }


        if (currentHealth > 0)
        {

            OnDamageTaken?.Invoke();
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

    override protected void DieAnimation()
    {
        anim.SetTrigger("Die");
        if (GetComponent<AudioComponent>() && dieSound != null)
        {
            GetComponent<AudioComponent>().PlaySound(dieSound);
        }
        base.DieAnimation();
        playermove.SetCanMove(false);


    }



    //healthComponent respawn
    override protected void RespawnDeath()
    {
        anim.SetTrigger("Spawn");
        base.RespawnDeath();
        SetText();
        playermove.SetCanMove(true);

        if (respawnVFX != null && Time.time - lastRespawnVFXTime >= respawnVFXCooldown)
        {
            Vector3 spawnPos = transform.position + Vector3.up * 1.0f;
            VisualEffect vfx = Instantiate(respawnVFX, spawnPos, Quaternion.identity);
            vfx.Play();
            Destroy(vfx.gameObject, 1f);

            lastRespawnVFXTime = Time.time;
        }
    }
    override protected void RespawnFall()
    {
        gameObject.transform.position = _fallPosition;
        SetText();
        if (respawnVFX != null && Time.time - lastRespawnVFXTime >= respawnVFXCooldown)
        {
            Vector3 spawnPos = transform.position + Vector3.up * 1.0f;
            VisualEffect vfx = Instantiate(respawnVFX, spawnPos, Quaternion.identity);
            vfx.Play();
            Destroy(vfx.gameObject, 1f);

            lastRespawnVFXTime = Time.time;
        }
    }

    //extra healht
    public bool ReceiveExtraHealth(int health)
    {
        if (currentExtraHealth >= maxExtraHealth)
        {
            return false;
        }
        if (GetComponent<AudioComponent>() && healSound != null)
        {
            GetComponent<AudioComponent>().PlaySound(healSound);
        }
        currentExtraHealth = Math.Min(maxExtraHealth, currentExtraHealth + health);
        SetTextExtra();
        if (extraHeartVFX != null)
        {
            VisualEffect vfx = Instantiate(extraHeartVFX, transform.position, Quaternion.identity);
            vfx.Play();
            Destroy(vfx.gameObject, 2f);
        }
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