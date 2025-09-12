using Assets.Devs.Julia.Scripts;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class MessageInteract : MonoBehaviour, IInteractable
{
    public GameObject messageUIPrefab;
    [TextArea(3, 10)] 
    public string messageText = "Mago fumon mueve habitacion";

    [SerializeField] private Sprite _keboardImg;
    [SerializeField] private Sprite _gamepadImg;

    public void Interact(GameObject interactor)
    {
        if (messageUIPrefab != null)
        {
            GameObject currentMessageUI = Instantiate(messageUIPrefab);

            //Exit image based on current device
            DeviceDetector deviceDetector = interactor.GetComponent<DeviceDetector>();
            if (deviceDetector) 
            {
                Transform exitImage = currentMessageUI.transform.Find("PopUp/Exit/ExitImage");
                if (exitImage != null && exitImage.gameObject) 
                {
                    if (deviceDetector.IsUsingKeyboard()) 
                    {
                        exitImage.gameObject.GetComponent<Image>().sprite = _keboardImg;
                    }
                    else 
                    {
                        exitImage.gameObject.GetComponent<Image>().sprite = _gamepadImg;
                    }
                }              
            }

            //Text
            TextMeshProUGUI tmpText = currentMessageUI.GetComponentInChildren<TextMeshProUGUI>();
            if (tmpText != null)
                tmpText.text = messageText;        
            
            Time.timeScale = 0f;
        }
    }
}