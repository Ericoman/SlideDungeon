using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UI.PauseMenu
{
    public class PauseMenu : MonoBehaviour
    {
        [SerializeField]
        private ShowablePanel settingsPanel;
        [SerializeField]
        private CanvasGroup buttonsCanvasGroup;

        [SerializeField] 
        private GameObject firstSelectedUIElement;

        private void Start()
        {
            GetComponent<ShowablePanel>().OnHide.AddListener(()=> ShowSettingsPanel(false));
            settingsPanel.OnShow.AddListener(()=>EnableInteraction(false));
            settingsPanel.OnHide.AddListener(() => EnableInteraction(true));
        }

        private void EnableInteraction(bool enable)
        {
            buttonsCanvasGroup.interactable = enable;
            buttonsCanvasGroup.blocksRaycasts = enable;
            if (enable)
            {
                EventSystem.current.SetSelectedGameObject(firstSelectedUIElement);
            }
        }
        public void ShowSettingsPanel(bool show)
        {
            settingsPanel.Show(show);
        }

        public void Resume()
        {
            GameManager.Instance.Pause(false);
        }

        public void QuitGame()
        {
            GameManager.Instance.QuitGame();
        }
        
    }
}
