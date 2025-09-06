using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


namespace UI.Settings
{
    [RequireComponent(typeof(ShowablePanel))]
    public class VideoSettingsMenu : MonoBehaviour
    {
        [SerializeField]
        private TMP_Dropdown resolutionDropdown;
        [SerializeField]
        private Toggle fullscreenToggle;

        private ShowablePanel _showablePanel;
        private Resolution[] _resolutions;

        bool _bFullscreen;
        int _currentResolutionIndex;

        private void Awake()
        {
            _showablePanel = GetComponent<ShowablePanel>();
            _showablePanel.OnHide.AddListener(ResetValues);
            
            _resolutions = Screen.resolutions;
            resolutionDropdown.ClearOptions();

            List<string> options = new List<string>();
            int currentResolutionIndex = 0;
            for (int i = 0; i < _resolutions.Length; i++)
            {
                string option = _resolutions[i].width + " x " + _resolutions[i].height;
                options.Add(option);

                if (_resolutions[i].width == Screen.currentResolution.width &&
                    _resolutions[i].height == Screen.currentResolution.height)
                {
                    currentResolutionIndex = i;
                }
            }

            _currentResolutionIndex = currentResolutionIndex;
            resolutionDropdown.AddOptions(options);
            resolutionDropdown.value = currentResolutionIndex;
            resolutionDropdown.RefreshShownValue();
            
            resolutionDropdown.onValueChanged.AddListener(SetResolution);
            
            fullscreenToggle.onValueChanged.AddListener(SetFullScreen);
            
            ResetValues();
        }

        public void SetResolution(int resolutionIndex)
        {
            _currentResolutionIndex = resolutionIndex;
        }

        public void SetFullScreen(bool isFullScreen)
        {
            _bFullscreen = isFullScreen;
        }

        private void ResetValues()
        {
            int currentResolutionWidth= PlayerPrefs.GetInt("ResolutionWidth", Screen.currentResolution.width);
            int currentResolutionHeight = PlayerPrefs.GetInt("ResolutionHeight", Screen.currentResolution.height);
            
            _bFullscreen = Screen.fullScreen;
            bool.TryParse(PlayerPrefs.GetString("Fullscreen"), out  _bFullscreen); 
            fullscreenToggle.isOn = _bFullscreen;

            for (int i = 0; i < _resolutions.Length; i++)
            {
                if (_resolutions[i].width == currentResolutionWidth &&
                    _resolutions[i].height == currentResolutionHeight)
                {
                    _currentResolutionIndex = i;
                }
            }
            resolutionDropdown.value = _currentResolutionIndex;
            resolutionDropdown.RefreshShownValue();
            
            Apply();
        }

        public void Apply()
        {
            Resolution resolution = _resolutions[_currentResolutionIndex];
            Screen.SetResolution(resolution.width, resolution.height, _bFullscreen ? FullScreenMode.FullScreenWindow : FullScreenMode.Windowed);
        }
        
        public void Save()
        {
            Apply();
            
            PlayerPrefs.SetInt("ResolutionWidth", _resolutions[_currentResolutionIndex].width);
            PlayerPrefs.SetInt("ResolutionHeight", _resolutions[_currentResolutionIndex].height);
            PlayerPrefs.SetString("Fullscreen", _bFullscreen.ToString());
            PlayerPrefs.Save();
        }

        public void Cancel()
        {
           _showablePanel.Show(false); 
        }
    }
}
