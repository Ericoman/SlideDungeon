using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

namespace UI.Settings
{
    [RequireComponent(typeof(ShowablePanel))]
    public class AudioSettingsMenu : MonoBehaviour
    {
        [SerializeField]
        private AudioMixer audioMixer;
        [SerializeField]
        private Slider masterVolumeSlider;
        [SerializeField]
        private Slider musicVolumeSlider;
        [SerializeField]
        private Slider soundFXVolumeSlider;
        [SerializeField]
        private Toggle muteToggle;

        private ShowablePanel _showablePanel;
        private float _masterLevel;
        private float _musicLevel;
        private float _soundFXLevel;
        private bool _bMute = false;
        
        private void Awake()
        {
            _showablePanel = GetComponent<ShowablePanel>();
            _showablePanel.OnHide.AddListener(ResetValues);
            
            masterVolumeSlider.onValueChanged.AddListener(SetMasterVolume);
            musicVolumeSlider.onValueChanged.AddListener(SetMusicVolume);
            soundFXVolumeSlider.onValueChanged.AddListener(SetSoundFXVolume);
            muteToggle.onValueChanged.AddListener(SetMute);
            ResetValues();
        }
        
        private void SetMasterVolume(float volume)
        {   
            volume = Mathf.Max(0.0001f, volume);
            audioMixer.SetFloat("MasterVolume", Mathf.Log10(volume) * 20);
            _masterLevel = volume;
        }
        
        private void SetMusicVolume(float volume)
        {   
            volume = Mathf.Max(0.0001f, volume);
            audioMixer.SetFloat("MusicVolume", Mathf.Log10(volume) * 20);
            _musicLevel = volume;
        }
        
        private void SetSoundFXVolume(float volume)
        {   
            volume = Mathf.Max(0.0001f, volume);
            audioMixer.SetFloat("SoundFXVolume", Mathf.Log10(volume) * 20);
            _soundFXLevel = volume;
        }

        private void SetMute(bool mute)
        {
            float volume = mute ? 0.0001f : _masterLevel;
            audioMixer.SetFloat("MasterVolume", Mathf.Log10(volume) * 20);
            _bMute = mute;
        }

        private void ResetValues()
        {
            _masterLevel = PlayerPrefs.GetFloat("MasterVolume", masterVolumeSlider.maxValue);
            masterVolumeSlider.value = _masterLevel;
            _musicLevel = PlayerPrefs.GetFloat("MusicVolume", musicVolumeSlider.maxValue);
            musicVolumeSlider.value = _musicLevel;
            _soundFXLevel = PlayerPrefs.GetFloat("SoundFXVolume", soundFXVolumeSlider.maxValue);
            soundFXVolumeSlider.value = _soundFXLevel;
            _bMute = false;
            bool.TryParse(PlayerPrefs.GetString("Mute"), out _bMute);
            
            muteToggle.isOn = _bMute;
        }

        public void Save()
        {
            PlayerPrefs.SetFloat("MasterVolume", _masterLevel);
            PlayerPrefs.SetFloat("MusicVolume", _musicLevel);
            PlayerPrefs.SetFloat("SoundFXVolume", _soundFXLevel);
            PlayerPrefs.SetString("Mute", _bMute.ToString());
            PlayerPrefs.Save();
        }

        public void Cancel()
        {
            _showablePanel.Show(false);
        }
        
    }
}
