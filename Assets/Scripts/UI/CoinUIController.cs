using TMPro;
using UnityEngine;
using DodoRun.Main;

namespace DodoRun.UI
{
    public sealed class CoinUIController : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI coinText;

        private void Start()
        {
            coinText.text = "0";
            GameService.Instance.EventService.OnCoinCollected.AddListner(UpdateCoins);
        }

        private void OnDestroy()
        {
            if (GameService.Instance == null) return;
            GameService.Instance.EventService.OnCoinCollected.RemoveListner(UpdateCoins);
        }

        private void UpdateCoins(int _)
        {
            coinText.text =
                GameService.Instance.ScoreService.CollectedCoins.ToString();
        }
    }
}
