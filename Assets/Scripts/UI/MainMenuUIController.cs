using DodoRun.Data;
using DodoRun.Sound;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace DodoRun.UI
{
    public sealed class MainMenuUIController : MonoBehaviour
    {
        [SerializeField] private GameObject settingMenu;
        [SerializeField] private TextMeshProUGUI coinText;
        [SerializeField] private TextMeshProUGUI bestScoreText;
        [SerializeField] private Button playButton;
        [SerializeField] private Button quitButton;
        [SerializeField] private Button settingButton;

        [Header("Scene")]
        [SerializeField] private string gameplaySceneName = "Gameplay";

        private void Awake()
        {
            playButton.onClick.AddListener(OnPlayButtonClicked);
            quitButton.onClick.AddListener(OnQuitButtonClicked);
            settingButton.onClick.AddListener(OnSettingButtonClicked);
        }

        private void OnEnable()
        {
            UpdateUI();
        }

        private void UpdateUI()
        {
            coinText.text = PlayerDataService.TotalCoins.ToString();
            bestScoreText.text = PlayerDataService.HighScore.ToString();
        }

        private void OnPlayButtonClicked()
        {
            AudioManager.Instance.PlayEffect(SoundType.ButtonClick);
            SceneManager.LoadScene(gameplaySceneName);
        }

        private void OnQuitButtonClicked()
        {
            AudioManager.Instance.PlayEffect(SoundType.ButtonClick);
            Application.Quit();
        }

        private void OnSettingButtonClicked()
        {
            AudioManager.Instance.PlayEffect(SoundType.ButtonClick);
            settingMenu.SetActive(true);
        }
    }
}
