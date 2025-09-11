using Assets.Devs.Julia.Scripts;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;

public class SmokeInteract : MonoBehaviour, IInteractable
{
    [SerializeField]
    private int healthRecived = 1;

    public GameObject EffectToActivate;

    public void Interact(GameObject interactor)
    {
        if (interactor.tag == "Player")
        {
            PlayersHealthComponent playersHealth = interactor.gameObject.GetComponentInChildren<PlayersHealthComponent>();
            if (playersHealth.ReceiveExtraHealth(healthRecived)) 
            {
                if (GetComponent<AudioComponent>())
                {
                    GetComponent<AudioComponent>().Play();
                }
                if (EffectToActivate) EffectToActivate.SetActive(true);
                StartCoroutine(EndEffect());
            }
        }
    }

    private IEnumerator EndEffect()
    {
        yield return new WaitForSeconds(1);
        if (EffectToActivate) EffectToActivate.SetActive(false);
    }
}
