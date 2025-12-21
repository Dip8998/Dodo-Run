using UnityEngine;
using TMPro;
using DodoRun.Main;
using DodoRun.UI.Controllers;

namespace DodoRun.UI
{
    public sealed class GameplayUIView : MonoBehaviour
    {
        [Header("Texts")]
        [SerializeField] private TextMeshProUGUI coinText;
        [SerializeField] private TextMeshProUGUI scoreText;
        [SerializeField] private TextMeshProUGUI multiplierText;

        [Header("Powerup Bars")]
        [SerializeField] private PowerupUIRefs powerupRefs;

        private CoinUIController coinUI;
        private ScoreUIController scoreUI;
        private PowerupUIController powerupUI;

        private void Awake()
        {
            var game = GameService.Instance;

            coinUI = new CoinUIController(coinText, game.EventService);
            scoreUI = new ScoreUIController(scoreText, multiplierText, game.ScoreService);
            powerupUI = new PowerupUIController(powerupRefs, game.EventService);
        }

        private void OnDestroy()
        {
            coinUI.Dispose();
            scoreUI.Dispose();
            powerupUI.Dispose();
        }

        private void Update()
        {
            powerupUI.Update(Time.unscaledDeltaTime);
        }
    }

    [System.Serializable]
    public struct PowerupUIRefs
    {
        public UnityEngine.UI.Slider magnet;
        public UnityEngine.UI.Slider shield;
        public UnityEngine.UI.Slider doubleScore;
    }
}
