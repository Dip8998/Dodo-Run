using DodoRun.Data;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace DodoRun.UI
{
    public sealed class MainMenuUIController : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI coinText;
        [SerializeField] private TextMeshProUGUI bestScoreText;
        [SerializeField] private Button playButton;
        [SerializeField] private Button quitButton;

        [Header("Scene")]
        [SerializeField] private string gameplaySceneName = "Gameplay";

        private void Awake()
        {
            playButton.onClick.AddListener(OnPlayButtonClicked);
            quitButton.onClick.AddListener(OnQuitButtonClicked);
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
            SceneManager.LoadScene(gameplaySceneName);
        }

        private void OnQuitButtonClicked()
        {
            Application.Quit();
        }
    }
}
