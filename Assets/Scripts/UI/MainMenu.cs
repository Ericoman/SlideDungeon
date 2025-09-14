using System;
using GlobalControllers;
using SavingSystem;
using UnityEngine;

namespace UI
{
    public class MainMenu : MonoBehaviour
    {
        [SerializeField]
        private ShowablePanel settingsPanel;

        [SerializeField] 
        private GameObject continueGameButton;

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
    }
}
