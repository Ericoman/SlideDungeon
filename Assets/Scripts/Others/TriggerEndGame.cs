using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TriggerEndGame : MonoBehaviour
{
    [SerializeField] private float WaitTime = 2;
    [SerializeField] private int EndSceneID = 2;

    public Image targetImage;

    public float duration = 0.5f;

    public void Start()
    {
        targetImage = GameManager.Instance.targetImage;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
             StartCoroutine(EndGame());
            FadeIn();
        }
    }

    private IEnumerator EndGame()
    {
        yield return new WaitForSeconds(WaitTime);
        SceneManager.LoadScene(EndSceneID);
    }

    public void FadeIn()
    {
        StartCoroutine(FadeToOpaque());
    }

    private IEnumerator FadeToOpaque()
    {
        Color color = targetImage.color;
        float startAlpha = color.a; // alpha actual
        float endAlpha = 1f;        // opaco
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);

            color.a = Mathf.Lerp(startAlpha, endAlpha, t);
            targetImage.color = color;

            yield return null;
        }
    }
}
