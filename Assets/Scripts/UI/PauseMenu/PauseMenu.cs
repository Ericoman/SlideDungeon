using UnityEngine;

namespace UI.PauseMenu
{
    public class PauseMenu : MonoBehaviour
    {
        [SerializeField]
        private ShowablePanel settingsPanel;

        public void ShowSettingsPanel()
        {
            settingsPanel.Show(true);
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
