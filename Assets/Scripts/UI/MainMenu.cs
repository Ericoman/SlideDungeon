using System;
using GlobalControllers;
using SavingSystem;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

namespace UI
{
    public class MainMenu : MonoBehaviour
    {
        [SerializeField]
        private ShowablePanel settingsPanel;
        [SerializeField]
        private ShowablePanel creditsPanel;

        [SerializeField] 
        private GameObject continueGameButton;
        [SerializeField]
        private CanvasGroup buttonsCanvasGroup;

        [SerializeField] 
        private GameObject firstSelectedUIElement;
        SavingSystemManager savingSystemManager;
        private void Start()
        {
            GameObject savingManager = GameObject.FindGameObjectWithTag("SavingSystemManager");
            if (savingManager != null)
            {
                savingSystemManager = savingManager.GetComponent<SavingSystemManager>();
            }
            else
            {
                Debug.LogError("SavingSystemManager not found");
            }
            
            continueGameButton.SetActive(savingSystemManager.IsThereAnySavedData());

            settingsPanel.OnShow.AddListener(()=>EnableInteraction(false));
            settingsPanel.OnHide.AddListener(() => EnableInteraction(true));

            EnableInteraction(true);
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
        public void ContinueGame()
        {
            SceneController.Instance.StartGame();
        }

        public void NewGame()
        {
            SceneController.Instance.StartGame(true);
        }

        public void QuitGame()
        {
            SceneController.Instance.QuitGame();
        }

        public void ShowSettings()
        {
            settingsPanel.Show(true);
        }

        public void ShowCredits()
        {
            creditsPanel.Show(true);
        }
    }
}
