using Assets.Devs.Julia.Scripts;
using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class HatGoldInteract : MonoBehaviour, IInteractable
{
    [SerializeField] Material materialGold;
    public void Interact(GameObject interactor)
    {
        if (interactor.tag == "Player")
        {
            GameObject hat = interactor.transform.Find("Hat 02 Brown")?.gameObject;
            if (hat != null)
            {
                MeshRenderer smr = hat.GetComponentInChildren<MeshRenderer>(true);

                if (smr != null)
                {
                    smr.materials = new Material[] { materialGold };
                }

                hat.SetActive(true);
            }
            gameObject.transform.position = new Vector3(0, -800, 0);

            StartCoroutine(EndGame());
        }
    }

    private IEnumerator EndGame()
    {
        yield return new WaitForSeconds(3);
        SceneManager.LoadScene(2);
    }
}