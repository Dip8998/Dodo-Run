using TMPro;
using DodoRun.Event;
using DodoRun.Main;

namespace DodoRun.UI.Controllers
{
    public sealed class CoinUIController
    {
        private readonly TextMeshProUGUI text;
        private readonly EventService events;

        public CoinUIController(TextMeshProUGUI text, EventService events)
        {
            this.text = text;
            this.events = events;

            text.text = "0";
            events.OnCoinCollected.AddListner(OnCollected);
        }

        private void OnCollected(int _)
        {
            text.text =
                GameService.Instance.ScoreService.CollectedCoins.ToString();
        }

        public void Dispose()
        {
            events.OnCoinCollected.RemoveListner(OnCollected);
        }
    }
}
