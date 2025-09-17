using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

namespace UI
{
    public class EndMenu : MonoBehaviour
    {
        [SerializeField] 
        private GameObject firstSelectedElement;
        [SerializeField]
        private ShowablePanel creditsPanel;
        [SerializeField]
        private CanvasGroup buttonsCanvasGroup;
        private void Start()
        {
            creditsPanel.OnShow?.AddListener(()=>EnableInteraction(false));
            creditsPanel.OnHide?.AddListener(()=>EnableInteraction(true));
            EnableInteraction(true);
        }
        private void EnableInteraction(bool enable)
        {
            buttonsCanvasGroup.interactable = enable;
            buttonsCanvasGroup.blocksRaycasts = enable;
            if (enable)
            {
                EventSystem.current.SetSelectedGameObject(firstSelectedElement);
            }
        }

        public void ShowCredits()
        {
            creditsPanel.Show(true);
        }
        public void RestartGame()
        {
            SceneManager.LoadScene(1);
        }

        public void MainMenu()
        {
            SceneManager.LoadScene(0);
        }

    }
}
