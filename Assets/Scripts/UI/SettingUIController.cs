using UnityEngine;
using UnityEngine.UI;
using DodoRun.Main;
using DodoRun.Sound;

namespace ForgottonChambers.UI
{
    public class SettingUIController : MonoBehaviour
    {
        [SerializeField] private Button controlButton;
        [SerializeField] private Button backButton;
        [SerializeField] private GameObject controlPanel;

        [SerializeField] private Slider sfxVolumeSlider;
        [SerializeField] private Slider musicVolumeSlider;

        private void Start()
        {
            controlButton.onClick.AddListener(ControlPanel);
            backButton.onClick.AddListener(BackButton);

            sfxVolumeSlider.onValueChanged.AddListener(SetSFXVolume);
            musicVolumeSlider.onValueChanged.AddListener(SetMusicVolume);

            sfxVolumeSlider.value = AudioManager.Instance.SoundFX.volume;
            musicVolumeSlider.value = AudioManager.Instance.SoundMusic.volume;
        }

        private void ControlPanel()
        {
            AudioManager.Instance.PlayEffect(SoundType.ButtonClick);
            controlPanel.SetActive(true);
        }

        private void BackButton()
        {
            AudioManager.Instance.PlayEffect(SoundType.ButtonClick);
            gameObject.SetActive(false);
        }

        private void SetSFXVolume(float volume)
        {
            AudioManager.Instance.SoundFX.volume = volume;
        }

        private void SetMusicVolume(float volume)
        {
            AudioManager.Instance.SoundMusic.volume = volume;
        }

        public void ButtonClicked()
        {
            AudioManager.Instance.PlayEffect(SoundType.ButtonClick);
        }
    }
}
