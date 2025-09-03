using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TriggerEndGame : MonoBehaviour
{
    [SerializeField] private int WaitTime = 2;
    [SerializeField] private int EndSceneID = 2;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
             StartCoroutine(EndGame());
        }
    }

    private IEnumerator EndGame()
    {
        yield return new WaitForSeconds(WaitTime);
        SceneManager.LoadScene(EndSceneID);
    }
}
