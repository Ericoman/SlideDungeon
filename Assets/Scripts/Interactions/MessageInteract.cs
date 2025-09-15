using Assets.Devs.Julia.Scripts;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class MessageInteract : MonoBehaviour, IInteractable
{
    public GameObject messageUIPrefab;

    public enum TypeOption
    {
        Text,
        Image
    }
    [SerializeField] private TypeOption type;

    [Header("If Text")]
    [TextArea(3, 10)] 
    public string messageText = "Mago fumon mueve habitacion";

    [Header("If Image")]
    [SerializeField] private string imageLocation = "PopUp/Key_Image";
    [SerializeField] private Sprite _keboardInfoImg;
    [SerializeField] private Sprite _gamepadInfoImg;

    [Header("Exit info")]
    [SerializeField] private Sprite _keboardExitImg;
    [SerializeField] private Sprite _gamepadExitImg;

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
                        if(_keboardExitImg) exitImage.gameObject.GetComponent<Image>().sprite = _keboardExitImg;
                    }
                    else 
                    {
                        if (_gamepadExitImg) exitImage.gameObject.GetComponent<Image>().sprite = _gamepadExitImg;
                    }
                }              
            }

            if(type == TypeOption.Text) //Text
            {
                TextMeshProUGUI tmpText = currentMessageUI.GetComponentInChildren<TextMeshProUGUI>();
                if (tmpText != null)
                    tmpText.text = messageText;    
            }
            else  //Image
            {
                //Info image based on current device
                Transform infoImage = currentMessageUI.transform.Find(imageLocation);
                if (infoImage != null && infoImage.gameObject)
                {
                    if (deviceDetector.IsUsingKeyboard())
                    {
                        if (_keboardInfoImg) infoImage.gameObject.GetComponent<Image>().sprite = _keboardInfoImg;
                    }
                    else
                    {
                        if (_gamepadInfoImg) infoImage.gameObject.GetComponent<Image>().sprite = _gamepadInfoImg;
                    }
                }
            }

            Time.timeScale = 0f;
        }
    }
}