using DodoRun.Main;
using TMPro;
using UnityEngine;

namespace DodoRun.UI
{
    public class CoinUIController : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI coinText;

        private void Start()
        {
            coinText.text = "0";
            GameService.Instance.EventService.OnCoinCollected.AddListner(OnCoinCollected);
        }

        private void OnDestroy()
        {
            if (GameService.Instance != null)
                GameService.Instance.EventService.OnCoinCollected.RemoveListner(OnCoinCollected);
        }

        private void OnCoinCollected(int amount)
        {
            int total = GameService.Instance.ScoreService.CollectedCoins;
            coinText.text = total.ToString();
        }
    }
}
