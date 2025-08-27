using Assets.Devs.Julia.Scripts;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class MessageInteract : MonoBehaviour, IInteractable
{
    public GameObject messageUIPrefab;
    [TextArea(3, 10)] 
    public string messageText = "Mago fumon mueve habitacion";


    public void Interact(GameObject interactor)
    {
        if (messageUIPrefab != null)
        {
            GameObject currentMessageUI = Instantiate(messageUIPrefab);

            TextMeshProUGUI tmpText = currentMessageUI.GetComponentInChildren<TextMeshProUGUI>();
            if (tmpText != null)
                tmpText.text = messageText;        
            
            Time.timeScale = 0f;
        }
    }
}