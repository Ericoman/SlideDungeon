using System;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Settings
{
    [RequireComponent(typeof(ShowablePanel))]
    public class SettingsMenu : MonoBehaviour
    {
        [SerializeField] 
        private ToggleGroup toggleGroup;
        [SerializeField]
        private ShowablePanel[] settingsPanels;
        
        
        private ShowablePanel _showablePanel;
        private Toggle[] _toggles;
        private int _currentIndex;

        private void Awake()
        {
            _showablePanel = GetComponent<ShowablePanel>();
            _showablePanel.OnHide.AddListener(ResetSettingsMenu);
            
            _toggles = toggleGroup.transform.GetComponentsInChildren<Toggle>();
        }

        private void ResetSettingsMenu()
        {
            _toggles[0].isOn = true;
            _toggles[0].onValueChanged.Invoke(true);
        }

        public void ToggleSettingsMenu(int menuIndex)
        {
            if (menuIndex < settingsPanels.Length)
            {
                settingsPanels[_currentIndex].Show(false);
                settingsPanels[menuIndex].Show(true);
                _currentIndex = menuIndex;
            }
        }

        public void Close()
        {
            if (_showablePanel.IsShown)
            {
                _showablePanel.Show(false);
            }
        }
    }
}
