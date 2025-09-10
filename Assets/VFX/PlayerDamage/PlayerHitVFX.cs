using UnityEngine;
using UnityEngine.VFX;

public class PlayerHitVFX : MonoBehaviour
{
    [Header("References")]
    public PlayersHealthComponent health;
    public VisualEffect hitVFX;    
    public Transform spawnPoint;
    public Transform visualRoot; // el objeto gráfico (malla/rig), no el collider

    [Header("Flash Settings")]
    public Renderer[] renderers;   
    public Color flashColor = Color.red;
    public float flashDuration = 0.1f;

    [Header("Scale Feedback")]
    public float scaleUpFactor = 1.2f;   // cuánto crece
    public float scaleDuration = 0.15f;  // cuánto tarda en volver

    [Header("VFX Settings")]
    public float cooldown = 0.2f;
    private float _lastEffectTime;

    private Vector3 _originalScale;

    void Awake()
    {
        if (health == null)
            health = GetComponent<PlayersHealthComponent>();
        if (spawnPoint == null)
            spawnPoint = transform;
        if (visualRoot == null)
            visualRoot = transform; // fallback al propio Player

        _originalScale = visualRoot.localScale;
    }

    void OnEnable()
    {
        health.OnDamageTaken += HandleDamageFeedback;
    }

    void OnDisable()
    {
        health.OnDamageTaken -= HandleDamageFeedback;
    }

    private void HandleDamageFeedback()
    {
        if (Time.time - _lastEffectTime >= cooldown)
        {
            PlayHitEffect();
            _lastEffectTime = Time.time;
        }

        Flash();
        ScaleFeedback();
    }

    private void PlayHitEffect()
    {
        if (hitVFX == null || spawnPoint == null) return;
        VisualEffect vfx = Instantiate(hitVFX, spawnPoint.position, Quaternion.identity);
        vfx.Play();
        Destroy(vfx.gameObject, 2f);
    }

    private void Flash()
    {
        if (renderers == null || renderers.Length == 0) return;
        StopCoroutine(nameof(FlashRoutine));
        StartCoroutine(nameof(FlashRoutine));
    }

    private System.Collections.IEnumerator FlashRoutine()
    {
        MaterialPropertyBlock block = new MaterialPropertyBlock();

        foreach (var r in renderers)
        {
            if (r == null) continue;
            r.GetPropertyBlock(block);
            if (r.sharedMaterial != null && r.sharedMaterial.HasProperty("_BaseColor"))
                block.SetColor("_BaseColor", flashColor);
            r.SetPropertyBlock(block);
        }

        yield return new WaitForSeconds(flashDuration);

        foreach (var r in renderers)
        {
            if (r == null) continue;
            r.GetPropertyBlock(block);
            if (r.sharedMaterial != null && r.sharedMaterial.HasProperty("_BaseColor"))
                block.SetColor("_BaseColor", Color.white);
            r.SetPropertyBlock(block);
        }
    }

    private void ScaleFeedback()
    {
        StopCoroutine(nameof(ScaleRoutine));
        StartCoroutine(nameof(ScaleRoutine));
    }

    private System.Collections.IEnumerator ScaleRoutine()
    {
        // crecer rápido
        Vector3 targetScale = _originalScale * scaleUpFactor;
        float t = 0f;
        while (t < scaleDuration * 0.3f)
        {
            t += Time.deltaTime;
            visualRoot.localScale = Vector3.Lerp(_originalScale, targetScale, t / (scaleDuration * 0.3f));
            yield return null;
        }

        // volver a normal
        t = 0f;
        while (t < scaleDuration * 0.7f)
        {
            t += Time.deltaTime;
            visualRoot.localScale = Vector3.Lerp(targetScale, _originalScale, t / (scaleDuration * 0.7f));
            yield return null;
        }

        visualRoot.localScale = _originalScale;
    }
}
