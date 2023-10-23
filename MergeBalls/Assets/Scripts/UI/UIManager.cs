using System.Threading.Tasks;
using UnityEngine;

namespace BiniGames.UI {
    public class UIManager : MonoBehaviour {
        [SerializeField] LoadingScreenWindow loadingScreenWindow;
        [SerializeField] VictoryScreenWindow winScreenWindow;

        private void Awake() {
            loadingScreenWindow.Show();
            winScreenWindow.Hide();
        }

        public void HideLoadingScreen() {
            loadingScreenWindow.Hide();
        }

        public async Task ShowWinScreen() {
            await Task.Delay(50);
            winScreenWindow.Hide();
            winScreenWindow.Show();
        }
    }
}
