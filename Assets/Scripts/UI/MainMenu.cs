using GlobalControllers;
using UnityEngine;

namespace UI
{
    public class MainMenu : MonoBehaviour
    {
        [SerializeField]
        private ShowablePanel settingsPanel;
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
